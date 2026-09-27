using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INAIPI.Core.Services.Models
{
    public class EntidadesDTO
    {
        public class RespuestaServicio
        {
            public int Codigo { get; set; }
            public string Mensaje { get; set; }
            public string DetalleTecnico { get; set; }
        }

        public class BeneficiarioDTO
        {
            public int BeneficiarioId { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public int CentroId { get; set; }
            public int TutorId { get; set; }
            public string DocumentoTutor { get; set; }
            public string FotografiaUrl { get; set; }
            public int Version { get; set; }
        }

        public class SolicitudRegistroBeneficiarioDTO
        {
            public BeneficiarioDTO Beneficiario { get; set; }
            public string NombresTutor { get; set; }
            public string ApellidosTutor { get; set; }
            public string TelefonoTutor { get; set; }
            public string DireccionTutor { get; set; }
        }

        public class SolicitudAtencionDTO
        {
            public Guid TransaccionGuid { get; set; }
            public int BeneficiarioId { get; set; }
            public int CentroId { get; set; }
            public decimal Monto { get; set; }
            public string DescripcionServicio { get; set; }
            public DateTime FechaRegistro { get; set; }
        }

        public class LoginRequestDTO
        {
            public string Username { get; set; }
            public string PasswordHash { get; set; }
        }

        public class UsuarioAutenticadoDTO
        {
            public int UsuarioId { get; set; }
            public string Username { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public int RolId { get; set; }
            public string NombreRol { get; set; }
            public int? CentroId { get; set; }
            public string NombreCentro { get; set; }
        }

        public class BeneficiarioConsultaDTO
        {
            public int BeneficiarioId { get; set; }
            public string NombreBeneficiario { get; set; }
            public string ApellidoBeneficiario { get; set; }
            public string FechaNacimientoStr { get; set; }
            public int EdadAnios { get; set; }
            public string FotografiaUrl { get; set; }
            public int VersionRegistro { get; set; }
            public int CentroId { get; set; }
            public string CodigoCentro { get; set; }
            public string NombreCentro { get; set; }
            public string TipoCentro { get; set; }
            public int TutorId { get; set; }
            public string DocumentoTutor { get; set; }
            public string NombreCompletoTutor { get; set; }
            public string TelefonoTutor { get; set; }
            public string DireccionTutor { get; set; }
            public string FechaIngresoStr { get; set; }
        }
    }
}