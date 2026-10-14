using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using INAIPI.CAJA.Models;
using Newtonsoft.Json;

namespace INAIPI.CAJA.Services
{
    public class ConexionSNDClient
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        private readonly string _baseUrl;

        public ConexionSNDClient()
        {
            _baseUrl = ConfigurationManager.AppSettings["SndBaseUrl"] ?? "http://localhost:59969";
            _baseUrl = _baseUrl.TrimEnd('/');
        }

        public bool VerificarConexionPing()
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/api/telemetria/ping").GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public bool EnviarTransaccion(TransaccionCajaDTO transaccion)
        {
            try
            {
                // Empaquetamos la transacción dentro del LoteSincronizacionDTO que exige el SND
                var lote = new LoteSincronizacionCajaDTO
                {
                    TerminalId = "CAJA-PRINCIPAL",
                    Transacciones = new List<TransaccionCajaDTO> { transaccion }
                };

                string json = JsonConvert.SerializeObject(lote);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync($"{_baseUrl}/api/caja/sincronizar", content).GetAwaiter().GetResult();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                INAIPI.CAJA.Helpers.CajaLogger.Error("Error al enviar lote a SND: " + ex.Message);
                return false;
            }
        }
    }
}