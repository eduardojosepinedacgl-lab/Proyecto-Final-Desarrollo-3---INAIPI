using System;

namespace INAIPI.CAJA.Models
{
    public static class TurnoCaja
    {
        public static int CajeroId { get; set; } = 0;
        public static decimal MontoInicial { get; set; } = 0m;
        public static bool TurnoAbierto { get; set; } = false;
        public static DateTime FechaApertura { get; set; }
    }
}