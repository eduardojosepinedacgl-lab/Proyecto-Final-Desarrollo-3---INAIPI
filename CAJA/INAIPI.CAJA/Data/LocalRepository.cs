using INAIPI.CAJA.Models;
using Newtonsoft.Json; // Requiere instalar el paquete NuGet Newtonsoft.Json
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
            // Lee la cadena de conexión desde el App.config
            _connectionString = ConfigurationManager.ConnectionStrings["CnCajaLocal"].ConnectionString;
        }

        public void GuardarTransaccionContingencia(TransaccionCajaDTO transaccion)
        {
            // Serializamos el objeto completo para la columna PayloadJson del Outbox
            string payload = JsonConvert.SerializeObject(transaccion);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.ppInsertarTransaccionContingencia", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros tipados para evitar Inyección SQL
                    cmd.Parameters.AddWithValue("@TransaccionGuid", transaccion.TransaccionGuid);
                    cmd.Parameters.AddWithValue("@BeneficiarioId", transaccion.BeneficiarioId);
                    cmd.Parameters.AddWithValue("@DescripcionServicio", transaccion.DescripcionServicio);
                    cmd.Parameters.AddWithValue("@Importe", transaccion.Importe);
                    cmd.Parameters.AddWithValue("@Fecha", transaccion.Fecha);
                    cmd.Parameters.AddWithValue("@PayloadJson", payload);

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
                // Consulta que une la transacción con su estado en la cola Outbox
                string query = @"
                    SELECT 
                        t.Id, 
                        t.TransaccionGuid, 
                        t.BeneficiarioId, 
                        t.DescripcionServicio, 
                        t.Importe, 
                        t.Fecha,
                        o.Sincronizado
                    FROM dbo.tblTransaccionesLocales t
                    LEFT JOIN dbo.tblOutboxTransacciones o ON t.TransaccionGuid = o.TransaccionGuid
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
                string query = "SELECT TransaccionGuid, PayloadJson, FechaCreacion FROM dbo.tblOutboxTransacciones WHERE Sincronizado = 0 ORDER BY FechaCreacion ASC";
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
                // Actualizamos la bandera a 1 y registramos la fecha exacta de subida al SND
                string query = "UPDATE dbo.tblOutboxTransacciones SET Sincronizado = 1, FechaSincronizacion = GETDATE() WHERE TransaccionGuid = @Guid";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Guid", transaccionGuid);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}