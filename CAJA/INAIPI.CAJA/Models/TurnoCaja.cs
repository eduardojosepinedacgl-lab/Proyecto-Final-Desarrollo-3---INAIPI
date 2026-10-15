using System;

namespace INAIPI.CAJA.Models
{
    public static class TurnoCaja
    {
        public static int CajeroId { get; set; } = 0;
        public static string NombreCajero { get; set; } = string.Empty;
        public static string Username { get; set; } = string.Empty;
        public static decimal MontoInicial { get; set; } = 0m;
        public static bool TurnoAbierto { get; set; } = false;
        public static DateTime FechaApertura { get; set; }
    }
}