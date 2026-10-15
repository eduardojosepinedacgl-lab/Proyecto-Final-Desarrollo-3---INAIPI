using INAIPI.CAJA.Helpers;
using INAIPI.CAJA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace INAIPI.CAJA.Data
{
    public class LocalRepository
    {
        private readonly string _connectionString;

        public LocalRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["CnCajaLocal"].ConnectionString;
        }

        public List<ServicioCatalogoDTO> ObtenerCatalogoServicios()
        {
            var servicios = new List<ServicioCatalogoDTO>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    const string query = @"
                        SELECT ComponenteId, Codigo, Nombre, PrecioFijo
                        FROM dbo.tblCatalogoServiciosLocal
                        WHERE Activo = 1
                        ORDER BY ComponenteId ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                servicios.Add(new ServicioCatalogoDTO
                                {
                                    ComponenteId = reader.GetInt32(0),
                                    Codigo = reader.GetString(1),
                                    Nombre = reader.GetString(2),
                                    PrecioFijo = reader.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CajaLogger.Error("No se pudo leer tblCatalogoServiciosLocal; se usará el catálogo oficial de respaldo en memoria.", ex);
            }

            if (servicios.Count == 0)
            {
                servicios = ObtenerCatalogoRespaldo();
            }

            return servicios;
        }

        private static List<ServicioCatalogoDTO> ObtenerCatalogoRespaldo()
        {
            return new List<ServicioCatalogoDTO>
            {
                new ServicioCatalogoDTO { ComponenteId = 1, Codigo = "CMP-EDU", Nombre = "Estimulación Oportuna y Educación Inicial", PrecioFijo = 150.00m },
                new ServicioCatalogoDTO { ComponenteId = 2, Codigo = "CMP-SAL", Nombre = "Salud y Nutrición", PrecioFijo = 300.00m },
                new ServicioCatalogoDTO { ComponenteId = 3, Codigo = "CMP-NEE", Nombre = "Atención a Necesidades Educativas Especiales", PrecioFijo = 250.00m },
                new ServicioCatalogoDTO { ComponenteId = 4, Codigo = "CMP-PRO", Nombre = "Protección Contra la Violencia", PrecioFijo = 100.00m },
                new ServicioCatalogoDTO { ComponenteId = 5, Codigo = "CMP-REG", Nombre = "Registro de Nacimiento e Identidad", PrecioFijo = 200.00m },
                new ServicioCatalogoDTO { ComponenteId = 6, Codigo = "CMP-FAM", Nombre = "Familia y Comunidad", PrecioFijo = 100.00m }
            };
        }

        public void GuardarTransaccionContingencia(TransaccionCajaDTO transaccion)
        {
            if (transaccion.Subtotal <= 0m && transaccion.Monto > 0m)
            {
                transaccion.Subtotal = Math.Round(transaccion.Monto / (1m + ReglasNegocio.TasaItbis), 2, MidpointRounding.AwayFromZero);
                transaccion.Itbis = ReglasNegocio.CalcularItbis(transaccion.Subtotal);
            }

            string payload = JsonConvert.SerializeObject(transaccion);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.ppInsertarTransaccionContingencia", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@TransaccionGuid", SqlDbType.UniqueIdentifier).Value = transaccion.TransaccionGuid;
                    cmd.Parameters.Add("@BeneficiarioId", SqlDbType.Int).Value = transaccion.BeneficiarioId;
                    cmd.Parameters.Add("@CentroId", SqlDbType.Int).Value = transaccion.CentroId > 0 ? transaccion.CentroId : ReglasNegocio.ObtenerCentroIdActual();
                    cmd.Parameters.Add("@ComponenteId", SqlDbType.Int).Value = transaccion.ComponenteId;
                    cmd.Parameters.Add("@UsuarioRegistroId", SqlDbType.Int).Value = transaccion.UsuarioRegistroId;
                    cmd.Parameters.Add("@DescripcionServicio", SqlDbType.NVarChar, 255).Value = transaccion.DescripcionServicio;
                    cmd.Parameters.Add("@Subtotal", SqlDbType.Decimal).Value = transaccion.Subtotal;
                    cmd.Parameters.Add("@Itbis", SqlDbType.Decimal).Value = transaccion.Itbis;
                    cmd.Parameters.Add("@Importe", SqlDbType.Decimal).Value = transaccion.Monto;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = transaccion.FechaRegistro;
                    cmd.Parameters.Add("@PayloadJson", SqlDbType.NVarChar, -1).Value = payload;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable ObtenerHistorialTransacciones()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = @"
                    SELECT 
                        t.Id, 
                        t.TransaccionGuid, 
                        t.BeneficiarioId, 
                        t.ComponenteId,
                        t.DescripcionServicio, 
                        t.CentroId,
                        t.UsuarioRegistroId,
                        ISNULL(c.NombreCompleto, 'Cajero #' + CAST(t.UsuarioRegistroId AS NVARCHAR(10))) AS NombreCajero,
                        t.Subtotal,
                        t.Itbis,
                        t.Importe, 
                        t.Fecha,
                        ISNULL(o.Sincronizado, 0) AS Sincronizado
                    FROM dbo.tblTransaccionesLocales t
                    LEFT JOIN dbo.tblOutboxTransacciones o ON t.TransaccionGuid = o.TransaccionGuid
                    LEFT JOIN dbo.tblCajerosLocal c ON t.UsuarioRegistroId = c.EmpleadoId
                    ORDER BY t.Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public List<OutboxItem> ObtenerTransaccionesPendientes()
        {
            var pendientes = new List<OutboxItem>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT TransaccionGuid, PayloadJson, FechaCreacion FROM dbo.tblOutboxTransacciones WHERE Sincronizado = 0 ORDER BY FechaCreacion ASC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendientes.Add(new OutboxItem
                            {
                                TransaccionGuid = reader.GetGuid(0),
                                PayloadJson = reader.GetString(1),
                                FechaCreacion = reader.GetDateTime(2),
                                Sincronizado = false
                            });
                        }
                    }
                }
            }
            return pendientes;
        }

        public void MarcarComoSincronizado(Guid transaccionGuid)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = "UPDATE dbo.tblOutboxTransacciones SET Sincronizado = 1, FechaSincronizacion = GETDATE() WHERE TransaccionGuid = @Guid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier).Value = transaccionGuid;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void ActualizarPadrónCajeros(List<CajeroDescargaDTO> cajeros)
        {
            if (cajeros == null || cajeros.Count == 0) return;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Desactivar todos preventivamente (si no vienen en la lista del CORE es que fueron dados de baja)
                using (SqlCommand cmdDesc = new SqlCommand("UPDATE dbo.tblCajerosLocal SET Activo = 0", con))
                {
                    cmdDesc.ExecuteNonQuery();
                }

                const string queryUpsert = @"
                    MERGE dbo.tblCajerosLocal AS Target
                    USING (SELECT @UsuarioId AS EmpleadoId, @Username AS Username) AS Source
                    ON Target.EmpleadoId = Source.EmpleadoId OR Target.Username = Source.Username
                    WHEN MATCHED THEN
                        UPDATE SET 
                            NombreCompleto = @NombreCompleto, 
                            PasswordHash = @PasswordHash, 
                            CentroId = @CentroId, 
                            Activo = 1, 
                            UltimaSincronizacion = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (EmpleadoId, Username, NombreCompleto, PasswordHash, CentroId, Activo)
                        VALUES (@UsuarioId, @Username, @NombreCompleto, @PasswordHash, @CentroId, 1);";

                foreach (var c in cajeros)
                {
                    using (SqlCommand cmdUpsert = new SqlCommand(queryUpsert, con))
                    {
                        cmdUpsert.Parameters.AddWithValue("@UsuarioId", c.UsuarioId);
                        cmdUpsert.Parameters.AddWithValue("@Username", c.Username);
                        cmdUpsert.Parameters.AddWithValue("@NombreCompleto", c.NombreCompleto);
                        cmdUpsert.Parameters.AddWithValue("@PasswordHash", c.PasswordHash);
                        cmdUpsert.Parameters.AddWithValue("@CentroId", c.CentroId);
                        cmdUpsert.ExecuteNonQuery();
                    }
                }
            }
        }

        public DataRow ObtenerCajeroPorUsername(string username)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = "SELECT EmpleadoId, Username, NombreCompleto, PasswordHash, Activo, IntentosFallidos, BloqueadoHasta FROM dbo.tblCajerosLocal WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0) return dt.Rows[0];
                    }
                }
            }
            return null;
        }

        public void RegistrarIntentoFallidoLogin(string username, int nuevoIntento, DateTime? bloqueo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = "UPDATE dbo.tblCajerosLocal SET IntentosFallidos = @Intentos, BloqueadoHasta = @Bloqueo WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Intentos", nuevoIntento);
                    cmd.Parameters.AddWithValue("@Bloqueo", (object)bloqueo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", username);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LimpiarIntentosLogin(string username)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = "UPDATE dbo.tblCajerosLocal SET IntentosFallidos = 0, BloqueadoHasta = NULL WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable ObtenerConsolidacionPorRango(DateTime fechaInicio, DateTime fechaFin)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = @"
                    SELECT 
                        t.ComponenteId,
                        t.DescripcionServicio,
                        COUNT(1) AS CantidadTransacciones,
                        SUM(t.Subtotal) AS TotalSubtotal,
                        SUM(t.Itbis) AS TotalItbis,
                        SUM(t.Importe) AS TotalRecaudado,
                        SUM(CASE WHEN ISNULL(o.Sincronizado, 0) = 1 THEN 1 ELSE 0 END) AS CantidadSincronizadas,
                        SUM(CASE WHEN ISNULL(o.Sincronizado, 0) = 0 THEN 1 ELSE 0 END) AS CantidadPendientes
                    FROM dbo.tblTransaccionesLocales t
                    LEFT JOIN dbo.tblOutboxTransacciones o ON t.TransaccionGuid = o.TransaccionGuid
                    WHERE CAST(t.Fecha AS DATE) >= @FechaInicio 
                      AND CAST(t.Fecha AS DATE) <= @FechaFin
                    GROUP BY t.ComponenteId, t.DescripcionServicio
                    ORDER BY t.ComponenteId ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaInicio.Date;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin.Date;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable ObtenerCuadreDeTurno(int usuarioId, DateTime fechaApertura, DateTime fechaCierre)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                const string query = @"
                    SELECT 
                        t.ComponenteId,
                        t.DescripcionServicio,
                        COUNT(1) AS CantidadTransacciones,
                        SUM(t.Subtotal) AS TotalSubtotal,
                        SUM(t.Itbis) AS TotalItbis,
                        SUM(t.Importe) AS TotalRecaudado
                    FROM dbo.tblTransaccionesLocales t
                    WHERE t.UsuarioRegistroId = @UsuarioId 
                      AND t.Fecha >= @Apertura 
                      AND t.Fecha <= @Cierre
                    GROUP BY t.ComponenteId, t.DescripcionServicio
                    ORDER BY t.ComponenteId ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                    cmd.Parameters.Add("@Apertura", SqlDbType.DateTime).Value = fechaApertura;
                    cmd.Parameters.Add("@Cierre", SqlDbType.DateTime).Value = fechaCierre;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        public DataTable BuscarBeneficiariosPorCedulaTutor(string cedulaTutor)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Filtra por la cédula del tutor y devuelve el ID y el nombre concatenado del niño
                const string query = @"
                    SELECT 
                        BeneficiarioId, 
                        Nombres + ' ' + Apellidos AS NombreNiño
                    FROM dbo.tblPadronBeneficiariosLocal
                    WHERE CedulaTutor = @CedulaTutor AND Activo = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CedulaTutor", cedulaTutor.Trim());
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public void ActualizarPadronBeneficiarios(List<BeneficiarioPadronDTO> padron)
        {
            if (padron == null || padron.Count == 0) return;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Desactivar todos preventivamente (si no vienen en la lista del CORE es que fueron dados de baja o graduados)
                using (SqlCommand cmdDesc = new SqlCommand("UPDATE dbo.tblPadronBeneficiariosLocal SET Activo = 0", con))
                {
                    cmdDesc.ExecuteNonQuery();
                }

                const string queryUpsert = @"
                    MERGE dbo.tblPadronBeneficiariosLocal AS Target
                    USING (SELECT @BeneficiarioId AS BeneficiarioId) AS Source
                    ON Target.BeneficiarioId = Source.BeneficiarioId
                    WHEN MATCHED THEN
                        UPDATE SET 
                            Nombres = @Nombres, 
                            Apellidos = @Apellidos, 
                            CedulaTutor = @CedulaTutor, 
                            Activo = 1, 
                            FechaUltimaSincronizacion = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (BeneficiarioId, Nombres, Apellidos, CedulaTutor, Activo)
                        VALUES (@BeneficiarioId, @Nombres, @Apellidos, @CedulaTutor, 1);";

                foreach (var b in padron)
                {
                    using (SqlCommand cmdUpsert = new SqlCommand(queryUpsert, con))
                    {
                        cmdUpsert.Parameters.AddWithValue("@BeneficiarioId", b.BeneficiarioId);
                        cmdUpsert.Parameters.AddWithValue("@Nombres", b.Nombres);
                        cmdUpsert.Parameters.AddWithValue("@Apellidos", b.Apellidos);
                        cmdUpsert.Parameters.AddWithValue("@CedulaTutor", b.CedulaTutor);
                        cmdUpsert.ExecuteNonQuery();
                    }
                }
            }
        }

        public string ObtenerNombreCajero(int empleadoId)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "SELECT NombreCompleto FROM dbo.tblCajerosLocal WHERE EmpleadoId = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", empleadoId);
                con.Open();
                object r = cmd.ExecuteScalar();
                return (r == null || r == DBNull.Value) ? null : r.ToString();
            }
        }

        public bool ObtenerDatosBeneficiarioLocal(int beneficiarioId, out string nombreNino, out string cedulaTutor)
        {
            nombreNino = null;
            cedulaTutor = null;

            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "SELECT Nombres + ' ' + Apellidos, CedulaTutor FROM dbo.tblPadronBeneficiariosLocal WHERE BeneficiarioId = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", beneficiarioId);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return false;
                    nombreNino = r.GetString(0);
                    cedulaTutor = r.GetString(1);
                    return true;
                }
            }
        }
    }
}