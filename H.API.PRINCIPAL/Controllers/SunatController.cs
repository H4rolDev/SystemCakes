using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SunatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string TOKEN = "sk_2255.0mXBpSYJMlE3inRkIl6X3x9amQdjkvnY";
        private const string BASE_URL = "https://api.decolecta.com/v1";

        public SunatController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<JsonElement> ConsultarApi(string endpoint)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return JsonSerializer.Deserialize<JsonElement>(content);
        }

        [HttpGet("dni")]
        public async Task<IActionResult> ConsultarDni(string numero)
        {
            if (string.IsNullOrEmpty(numero) || numero.Length != 8 || !numero.All(char.IsDigit))
            {
                return BadRequest(new { success = false, mensaje = "DNI debe tener 8 digitos" });
            }

            try
            {
                var endpoint = $"{BASE_URL}/reniec/dni?numero={numero}";
                var result = await ConsultarApi(endpoint);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, mensaje = "Error al consultar DNI", detalle = ex.Message });
            }
        }

        [HttpGet("ruc")]
        public async Task<IActionResult> ConsultarRuc(string numero, bool extendida = false)
        {
            if (string.IsNullOrEmpty(numero) || numero.Length != 11 || !numero.All(char.IsDigit))
            {
                return BadRequest(new { success = false, mensaje = "RUC debe tener 11 digitos" });
            }

            try
            {
                var endpoint = extendida ? $"{BASE_URL}/sunat/ruc/full?numero={numero}" : $"{BASE_URL}/sunat/ruc?numero={numero}";
                var result = await ConsultarApi(endpoint);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, mensaje = "Error al consultar RUC", detalle = ex.Message });
            }
        }
    }
}