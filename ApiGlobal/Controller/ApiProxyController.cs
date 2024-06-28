using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiGlobal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiProxyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiProxyController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> ProxyRequest([FromBody] ProxyRequest request)
        {
            var apiServers = _configuration.GetSection("ApiServers").Get<Dictionary<string, string>>();
            if (!apiServers.TryGetValue(request.Servidor, out var baseUrl))
            {
                return BadRequest("Servidor no válido");
            }

            var client = _httpClientFactory.CreateClient();
            var url = ConstructUrl(baseUrl, request.Opcion, request.Parametros);

            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Metodo), url);

            if (request.Parametros != null && (request.Metodo.ToUpper() == "POST" || request.Metodo.ToUpper() == "PUT"))
            {
                // Codificar los parámetros según ISO-8859-1 si se solicita
                if (request.CorregirISO8859)
                {
                    var postData = EncodeParametersToIso88591(request.Parametros);
                    httpRequest.Content = new ByteArrayContent(postData);
                    httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
                else
                {
                    // Usar la codificación por defecto (UTF-8)
                    httpRequest.Content = new StringContent(JsonSerializer.Serialize(request.Parametros), Encoding.UTF8, "application/json");
                }
            }

            var response = await client.SendAsync(httpRequest);
            var content = await response.Content.ReadAsByteArrayAsync();

            // Determinar el tipo de contenido esperado y retornar adecuadamente
            switch (request.Body.ToLower())
            {
                case "xml":
                    // Convertir la respuesta a XML si es necesario
                    var xmlContent = request.CorregirISO8859 ? Encoding.GetEncoding("ISO-8859-1").GetString(content) : Encoding.UTF8.GetString(content);
                    var xmlResponse = XElement.Parse(xmlContent);
                    return new ContentResult
                    {
                        Content = xmlResponse.ToString(),
                        ContentType = "application/xml",
                        StatusCode = (int)response.StatusCode
                    };

                case "json":
                    // No es necesario convertir la respuesta si ya es JSON
                    return new ContentResult
                    {
                        Content = Encoding.UTF8.GetString(content),
                        ContentType = "application/json",
                        StatusCode = (int)response.StatusCode
                    };

                case "text":
                    // Retornar la respuesta como texto plano
                    return new ContentResult
                    {
                        Content = request.CorregirISO8859 ? Encoding.GetEncoding("ISO-8859-1").GetString(content) : Encoding.UTF8.GetString(content),
                        ContentType = "text/plain",
                        StatusCode = (int)response.StatusCode
                    };

                case "html":
                    // Retornar la respuesta como HTML
                    return new ContentResult
                    {
                        Content = request.CorregirISO8859 ? Encoding.GetEncoding("ISO-8859-1").GetString(content) : Encoding.UTF8.GetString(content),
                        ContentType = "text/html",
                        StatusCode = (int)response.StatusCode
                    };

                case "binary":
                    // Retornar la respuesta binaria
                    return new FileContentResult(content, response.Content.Headers.ContentType.MediaType)
                    {
                        FileDownloadName = $"response_{DateTime.Now:yyyyMMddHHmmss}"
                    };

                default:
                    return BadRequest("Tipo de cuerpo no válido. Debe ser 'xml', 'json', 'text', 'html' o 'binary'.");
            }
        }

        private string ConstructUrl(string baseUrl, string opcion, Dictionary<string, string> parametros)
        {
            var url = $"{baseUrl}{opcion}";

            if (parametros != null && parametros.Count > 0)
            {
                var queryString = string.Join("&", parametros.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                url = $"{url}?{queryString}";
            }

            return url;
        }

        private byte[] EncodeParametersToIso88591(Dictionary<string, string> parametros)
        {
            var encodedParams = new List<string>();
            foreach (var kvp in parametros)
            {
                var encodedKey = EncodeToIso88591(kvp.Key);
                var encodedValue = EncodeToIso88591(kvp.Value);
                encodedParams.Add($"{encodedKey}={encodedValue}");
            }

            var postData = string.Join("&", encodedParams);
            return Encoding.GetEncoding("ISO-8859-1").GetBytes(postData);
        }

        private string EncodeToIso88591(string value)
        {
            var isoLatin1 = Encoding.GetEncoding("ISO-8859-1");
            var utfBytes = Encoding.UTF8.GetBytes(value);
            var isoBytes = Encoding.Convert(Encoding.UTF8, isoLatin1, utfBytes);
            return isoLatin1.GetString(isoBytes);
        }
    }

    public class ProxyRequest
    {
        public string Servidor { get; set; }
        public string Metodo { get; set; }
        public string Opcion { get; set; }
        public string Body { get; set; } // Nuevo campo para el tipo de respuesta
        public bool CorregirISO8859 { get; set; } // Nuevo campo para indicar corrección ISO-8859-1 opcional
        public Dictionary<string, string> Parametros { get; set; }
    }
}
