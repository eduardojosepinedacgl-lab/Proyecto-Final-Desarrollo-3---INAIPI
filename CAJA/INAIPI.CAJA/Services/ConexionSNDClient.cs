using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Helpers;
using INAIPI.CAJA.Models;
using Newtonsoft.Json;

namespace INAIPI.CAJA.Services
{
    public class ConexionSNDClient
    {
        private static readonly HttpClient _httpClient;
        private readonly string _sndBaseUrl;
        private readonly string _coreBaseUrl;

        static ConexionSNDClient()
        {
            // Permitir certificados autofirmados de desarrollo local en IIS Express (HTTPS localhost)
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
        }

        public ConexionSNDClient()
        {
            _sndBaseUrl = ConfigurationManager.AppSettings["SndBaseUrl"] ?? "http://localhost:59969";
            _sndBaseUrl = _sndBaseUrl.TrimEnd('/');

            _coreBaseUrl = ConfigurationManager.AppSettings["CoreBaseUrl"] ?? "https://localhost:44333";
            _coreBaseUrl = _coreBaseUrl.TrimEnd('/');
        }

        public bool VerificarConexionPing()
        {
            try
            {
                var response = _httpClient.GetAsync($"{_sndBaseUrl}/api/telemetria/ping").GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SincronizarCajerosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_coreBaseUrl}/api/seguridad/cajeros");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var cajerosActivos = JsonConvert.DeserializeObject<List<CajeroDescargaDTO>>(json);

                    if (cajerosActivos != null && cajerosActivos.Count > 0)
                    {
                        var repo = new LocalRepository();
                        repo.ActualizarPadrónCajeros(cajerosActivos);

                        CajaLogger.Info($"Sincronización de cajeros exitosa. {cajerosActivos.Count} operadores descargados.");
                        return true;
                    }
                }
                else
                {
                    CajaLogger.Error($"El servidor CORE respondió con código {(int)response.StatusCode} al consultar cajeros.");
                }
                return false;
            }
            catch (Exception ex)
            {
                CajaLogger.Error("Fallo de red al intentar descargar cajeros desde el CORE: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> SincronizarPadronBeneficiariosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_coreBaseUrl}/api/beneficiarios/padron");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var padron = JsonConvert.DeserializeObject<List<BeneficiarioPadronDTO>>(json);

                    if (padron != null && padron.Count > 0)
                    {
                        var repo = new LocalRepository();
                        repo.ActualizarPadronBeneficiarios(padron);

                        CajaLogger.Info($"Sincronización de padrón exitosa. {padron.Count} niños descargados.");
                        return true;
                    }
                }
                else
                {
                    CajaLogger.Error($"El servidor CORE respondió con código {(int)response.StatusCode} al consultar el padrón de niños.");
                }
                return false;
            }
            catch (Exception ex)
            {
                CajaLogger.Error("Fallo de red al intentar descargar el padrón de beneficiarios: " + ex.Message);
                return false;
            }
        }

        public bool EnviarTransaccion(TransaccionCajaDTO transaccion)
        {
            try
            {
                var lote = new LoteSincronizacionCajaDTO
                {
                    TerminalId = ConfigurationManager.AppSettings["TerminalId"] ?? Environment.MachineName,
                    Transacciones = new List<TransaccionCajaDTO> { transaccion }
                };

                string json = JsonConvert.SerializeObject(lote);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // El envío transaccional sigue yendo hacia Integración
                var response = _httpClient.PostAsync($"{_sndBaseUrl}/api/caja/sincronizar", content).GetAwaiter().GetResult();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                CajaLogger.Error("Error al enviar lote a SND: " + ex.Message);
                return false;
            }
        }
    }
}