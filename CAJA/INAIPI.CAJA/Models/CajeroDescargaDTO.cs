namespace INAIPI.CAJA.Models
{
    public class CajeroDescargaDTO
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; }
        public string NombreCompleto { get; set; }
        public string PasswordHash { get; set; }
        public int CentroId { get; set; }
    }
}