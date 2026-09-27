using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using INAIPI.Core.Services.Models;
using static INAIPI.Core.Services.Models.EntidadesDTO;

namespace INAIPI.Core.Services.Data
{
    public class CoreRepository
    {
        public UsuarioAutenticadoDTO ValidarCredenciales(string username, string passwordHash)
        {
            using (SqlConnection conexion = AccesoDatos.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("dbo.ppValidarUsuario", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username });
                comando.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 256) { Value = passwordHash });

                using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (lector.Read())
                    {
                        return new UsuarioAutenticadoDTO
                        {
                            UsuarioId = lector.GetInt32(lector.GetOrdinal("UsuarioId")),
                            Username = lector.GetString(lector.GetOrdinal("Username")),
                            Nombres = lector.GetString(lector.GetOrdinal("Nombres")),
                            Apellidos = lector.GetString(lector.GetOrdinal("Apellidos")),
                            Email = lector.GetString(lector.GetOrdinal("Email")),
                            RolId = lector.GetInt32(lector.GetOrdinal("RolId")),
                            NombreRol = lector.GetString(lector.GetOrdinal("NombreRol")),
                            CentroId = lector.IsDBNull(lector.GetOrdinal("CentroId")) ? (int?)null : lector.GetInt32(lector.GetOrdinal("CentroId")),
                            NombreCentro = lector.IsDBNull(lector.GetOrdinal("NombreCentro")) ? null : lector.GetString(lector.GetOrdinal("NombreCentro"))
                        };
                    }
                }
            }
            return null;
        }

        public RespuestaServicio InsertarBeneficiario(BeneficiarioDTO dto, string nombresTutor, string apellidosTutor, string telefonoTutor, string direccionTutor)
        {
            using (SqlConnection conexion = AccesoDatos.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("dbo.ppInsertarBeneficiario", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = dto.Nombres });
                comando.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = dto.Apellidos });
                comando.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
                comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = dto.CentroId });
                comando.Parameters.Add(new SqlParameter("@DocumentoTutor", SqlDbType.NVarChar, 20) { Value = dto.DocumentoTutor });
                comando.Parameters.Add(new SqlParameter("@NombresTutor", SqlDbType.NVarChar, 100) { Value = nombresTutor });
                comando.Parameters.Add(new SqlParameter("@ApellidosTutor", SqlDbType.NVarChar, 100) { Value = apellidosTutor });
                comando.Parameters.Add(new SqlParameter("@TelefonoTutor", SqlDbType.NVarChar, 20) { Value = telefonoTutor });
                comando.Parameters.Add(new SqlParameter("@DireccionTutor", SqlDbType.NVarChar, 250) { Value = direccionTutor });
                comando.Parameters.Add(new SqlParameter("@FotografiaUrl", SqlDbType.NVarChar, 500) { Value = (object)dto.FotografiaUrl ?? DBNull.Value });

                using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (lector.Read())
                    {
                        return new RespuestaServicio
                        {
                            Codigo = lector.GetInt32(lector.GetOrdinal("Codigo")),
                            Mensaje = lector.GetString(lector.GetOrdinal("Mensaje")),
                            DetalleTecnico = lector.IsDBNull(lector.GetOrdinal("DetalleTecnico")) ? null : lector.GetString(lector.GetOrdinal("DetalleTecnico"))
                        };
                    }
                }
            }

            return new RespuestaServicio
            {
                Codigo = 500,
                Mensaje = "No se obtuvo respuesta del procedimiento almacenado.",
                DetalleTecnico = "Procedimiento dbo.ppInsertarBeneficiario finalizó sin retornar registro."
            };
        }

        public RespuestaServicio ActualizarBeneficiario(BeneficiarioDTO dto)
        {
            using (SqlConnection conexion = AccesoDatos.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("dbo.ppActualizarBeneficiario", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@BeneficiarioId", SqlDbType.Int) { Value = dto.BeneficiarioId });
                comando.Parameters.Add(new SqlParameter("@Nombres", SqlDbType.NVarChar, 100) { Value = dto.Nombres });
                comando.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = dto.Apellidos });
                comando.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
                comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = dto.CentroId });
                comando.Parameters.Add(new SqlParameter("@TutorId", SqlDbType.Int) { Value = dto.TutorId });
                comando.Parameters.Add(new SqlParameter("@FotografiaUrl", SqlDbType.NVarChar, 500) { Value = (object)dto.FotografiaUrl ?? DBNull.Value });
                comando.Parameters.Add(new SqlParameter("@VersionOriginal", SqlDbType.Int) { Value = dto.Version });

                using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (lector.Read())
                    {
                        return new RespuestaServicio
                        {
                            Codigo = lector.GetInt32(lector.GetOrdinal("Codigo")),
                            Mensaje = lector.GetString(lector.GetOrdinal("Mensaje")),
                            DetalleTecnico = lector.IsDBNull(lector.GetOrdinal("DetalleTecnico")) ? null : lector.GetString(lector.GetOrdinal("DetalleTecnico"))
                        };
                    }
                }
            }

            return new RespuestaServicio
            {
                Codigo = 500,
                Mensaje = "No se obtuvo respuesta del procedimiento de actualización.",
                DetalleTecnico = "Procedimiento dbo.ppActualizarBeneficiario no retornó datos."
            };
        }

        public RespuestaServicio RegistrarAtencion(SolicitudAtencionDTO dto)
        {
            using (SqlConnection conexion = AccesoDatos.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("dbo.ppRegistrarAtencion", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@TransaccionGuid", SqlDbType.UniqueIdentifier) { Value = dto.TransaccionGuid });
                comando.Parameters.Add(new SqlParameter("@BeneficiarioId", SqlDbType.Int) { Value = dto.BeneficiarioId });
                comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = dto.CentroId });
                comando.Parameters.Add(new SqlParameter("@Monto", SqlDbType.Decimal) { Value = dto.Monto, Precision = 18, Scale = 2 });
                comando.Parameters.Add(new SqlParameter("@DescripcionServicio", SqlDbType.NVarChar, 500) { Value = dto.DescripcionServicio });
                comando.Parameters.Add(new SqlParameter("@FechaRegistro", SqlDbType.DateTime) { Value = dto.FechaRegistro });

                using (SqlDataReader lector = comando.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (lector.Read())
                    {
                        return new RespuestaServicio
                        {
                            Codigo = lector.GetInt32(lector.GetOrdinal("Codigo")),
                            Mensaje = lector.GetString(lector.GetOrdinal("Mensaje")),
                            DetalleTecnico = lector.IsDBNull(lector.GetOrdinal("DetalleTecnico")) ? null : lector.GetString(lector.GetOrdinal("DetalleTecnico"))
                        };
                    }
                }
            }

            return new RespuestaServicio
            {
                Codigo = 500,
                Mensaje = "Fallo al asentar la atención.",
                DetalleTecnico = "Procedimiento dbo.ppRegistrarAtencion no retornó datos."
            };
        }

        public List<BeneficiarioConsultaDTO> ConsultarBeneficiariosPorCentro(int? centroId, DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<BeneficiarioConsultaDTO> lista = new List<BeneficiarioConsultaDTO>();

            using (SqlConnection conexion = AccesoDatos.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("dbo.ppGetBeneficiariosPorCentro", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add(new SqlParameter("@CentroId", SqlDbType.Int) { Value = (object)centroId ?? DBNull.Value });
                comando.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date) { Value = (object)fechaInicio ?? DBNull.Value });
                comando.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date) { Value = (object)fechaFin ?? DBNull.Value });

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new BeneficiarioConsultaDTO
                        {
                            BeneficiarioId = lector.GetInt32(lector.GetOrdinal("BeneficiarioId")),
                            NombreBeneficiario = lector.GetString(lector.GetOrdinal("NombreBeneficiario")),
                            ApellidoBeneficiario = lector.GetString(lector.GetOrdinal("ApellidoBeneficiario")),
                            FechaNacimientoStr = lector.GetString(lector.GetOrdinal("FechaNacimientoStr")),
                            EdadAnios = lector.GetInt32(lector.GetOrdinal("EdadAnios")),
                            FotografiaUrl = lector.IsDBNull(lector.GetOrdinal("FotografiaUrl")) ? null : lector.GetString(lector.GetOrdinal("FotografiaUrl")),
                            VersionRegistro = lector.GetInt32(lector.GetOrdinal("VersionRegistro")),
                            CentroId = lector.GetInt32(lector.GetOrdinal("CentroId")),
                            CodigoCentro = lector.GetString(lector.GetOrdinal("CodigoCentro")),
                            NombreCentro = lector.GetString(lector.GetOrdinal("NombreCentro")),
                            TipoCentro = lector.GetString(lector.GetOrdinal("TipoCentro")),
                            TutorId = lector.GetInt32(lector.GetOrdinal("TutorId")),
                            DocumentoTutor = lector.GetString(lector.GetOrdinal("DocumentoTutor")),
                            NombreCompletoTutor = lector.GetString(lector.GetOrdinal("NombreCompletoTutor")),
                            TelefonoTutor = lector.GetString(lector.GetOrdinal("TelefonoTutor")),
                            DireccionTutor = lector.GetString(lector.GetOrdinal("DireccionTutor")),
                            FechaIngresoStr = lector.GetString(lector.GetOrdinal("FechaIngresoStr"))
                        });
                    }
                }
            }

            return lista;
        }
    }
}