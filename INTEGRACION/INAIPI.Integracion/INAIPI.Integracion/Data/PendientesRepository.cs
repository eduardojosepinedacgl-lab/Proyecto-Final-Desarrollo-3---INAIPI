using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using INAIPI.Integracion.Models;

namespace INAIPI.Integracion.Data
{
    public class PendientesRepository
    {
        private readonly string connectionString;
        public PendientesRepository()
        {
            var c = ConfigurationManager.ConnectionStrings["CnIntegracionDb"];
            if (c == null || string.IsNullOrWhiteSpace(c.ConnectionString))
                throw new ConfigurationErrorsException("Falta CnIntegracionDb en Web.config.");
            connectionString = c.ConnectionString;
        }
        public async Task GuardarLoteAsync(LoteSincronizacionDTO lote)
        {
            using (var c = new SqlConnection(connectionString))
            {
                await c.OpenAsync();
                using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (var t in lote.Transacciones)
                        {
                            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.TransaccionesPendientes WITH (UPDLOCK, HOLDLOCK) WHERE TransaccionGuid=@Guid)
INSERT INTO dbo.TransaccionesPendientes
(TransaccionGuid,TerminalId,BeneficiarioId,CentroId,Monto,Descripcion,FechaRegistro,Estado,Intentos,ProximoIntentoUtc)
VALUES (@Guid,@Terminal,@Beneficiario,@Centro,@Monto,@Descripcion,@Fecha,'Pendiente',0,SYSUTCDATETIME());", c, tx))
                            {
                                cmd.Parameters.Add("@Guid",SqlDbType.UniqueIdentifier).Value=t.TransaccionGuid;
                                cmd.Parameters.Add("@Terminal",SqlDbType.NVarChar,100).Value=lote.TerminalId;
                                cmd.Parameters.Add("@Beneficiario",SqlDbType.Int).Value=t.BeneficiarioId;
                                cmd.Parameters.Add("@Centro",SqlDbType.Int).Value=t.CentroId;
                                var monto=cmd.Parameters.Add("@Monto",SqlDbType.Decimal);
                                monto.Precision=18; monto.Scale=2; monto.Value=t.Monto;
                                cmd.Parameters.Add("@Descripcion",SqlDbType.NVarChar,-1).Value=(object)t.Descripcion ?? DBNull.Value;
                                cmd.Parameters.Add("@Fecha",SqlDbType.DateTime2).Value=t.FechaRegistro.ToUniversalTime();
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        tx.Commit();
                    }
                    catch { tx.Rollback(); throw; }
                }
            }
        }
        public async Task<List<Guid>> ObtenerPendientesAsync()
        {
            var lista=new List<Guid>();
            using(var c=new SqlConnection(connectionString))
            using(var cmd=new SqlCommand(@"SELECT TOP (20) TransaccionGuid FROM dbo.TransaccionesPendientes
WHERE Estado='Pendiente' AND (ProximoIntentoUtc IS NULL OR ProximoIntentoUtc<=SYSUTCDATETIME())
ORDER BY FechaRecepcionUtc,TransaccionGuid;",c))
            {
                await c.OpenAsync();
                using(var r=await cmd.ExecuteReaderAsync())
                    while(await r.ReadAsync()) lista.Add(r.GetGuid(0));
            }
            return lista;
        }
        public async Task<string> ObtenerEstadoAsync(Guid guid)
        {
            using(var c=new SqlConnection(connectionString))
            using(var cmd=new SqlCommand("SELECT Estado FROM dbo.TransaccionesPendientes WHERE TransaccionGuid=@Guid;",c))
            {
                cmd.Parameters.Add("@Guid",SqlDbType.UniqueIdentifier).Value=guid;
                await c.OpenAsync();
                var value=await cmd.ExecuteScalarAsync();
                return value==null || value==DBNull.Value ? null : Convert.ToString(value);
            }
        }
        public async Task<SolicitudAtencionCoreDTO> ObtenerSolicitudAsync(Guid guid)
        {
            using(var c=new SqlConnection(connectionString))
            using(var cmd=new SqlCommand(@"SELECT TransaccionGuid,BeneficiarioId,CentroId,Monto,Descripcion,FechaRegistro
FROM dbo.TransaccionesPendientes WHERE TransaccionGuid=@Guid;",c))
            {
                cmd.Parameters.Add("@Guid",SqlDbType.UniqueIdentifier).Value=guid;
                await c.OpenAsync();
                using(var r=await cmd.ExecuteReaderAsync())
                {
                    if(!await r.ReadAsync()) throw new InvalidOperationException("Transaccion no encontrada.");
                    return new SolicitudAtencionCoreDTO
                    {
                        TransaccionGuid=r.GetGuid(0), BeneficiarioId=r.GetInt32(1), CentroId=r.GetInt32(2),
                        Monto=r.GetDecimal(3), DescripcionServicio=r.IsDBNull(4)?null:r.GetString(4),
                        FechaRegistro=DateTime.SpecifyKind(r.GetDateTime(5),DateTimeKind.Utc)
                    };
                }
            }
        }
        public async Task ActualizarResultadoAsync(Guid guid,string estado,string error=null)
        {
            if(estado!="Sincronizada" && estado!="Pendiente" && estado!="Rechazada")
                throw new ArgumentException("Estado no permitido.");
            using(var c=new SqlConnection(connectionString))
            using(var cmd=new SqlCommand(@"UPDATE dbo.TransaccionesPendientes
SET Estado=@Estado,Intentos=Intentos+1,UltimoError=@Error,
FechaSincronizacionUtc=CASE WHEN @Estado='Sincronizada' THEN SYSUTCDATETIME() ELSE NULL END,
ProximoIntentoUtc=CASE WHEN @Estado='Pendiente' THEN DATEADD(SECOND,30,SYSUTCDATETIME()) ELSE NULL END
WHERE TransaccionGuid=@Guid AND Estado<>'Sincronizada';",c))
            {
                cmd.Parameters.Add("@Guid",SqlDbType.UniqueIdentifier).Value=guid;
                cmd.Parameters.Add("@Estado",SqlDbType.VarChar,20).Value=estado;
                if(error!=null && error.Length>2000) error=error.Substring(0,2000);
                cmd.Parameters.Add("@Error",SqlDbType.NVarChar,2000).Value=(object)error ?? DBNull.Value;
                await c.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
