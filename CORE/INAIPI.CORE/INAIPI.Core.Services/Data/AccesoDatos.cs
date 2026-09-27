using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace INAIPI.Core.Services.Data
{
    public class AccesoDatos
    {
        private static readonly string CadenaConexion;

        static AccesoDatos()
        {
            ConnectionStringSettings config = ConfigurationManager.ConnectionStrings["CnCoreDb"];
            if (config == null || string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'CnCoreDb' no está definida en el archivo Web.config.");
            }
            CadenaConexion = config.ConnectionString;
        }

        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(CadenaConexion);
            if (conexion.State != ConnectionState.Open)
            {
                conexion.Open();
            }
            return conexion;
        }
    }
}