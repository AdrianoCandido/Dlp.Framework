using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dlp.Framework {

    /// <summary>
    /// Enumerates the available HTTP verbs for REST communication.
    /// </summary>
    public enum HttpVerb {

        /// <summary>
        /// Defines a HTTP GET method.
        /// </summary>
        Get,

        /// <summary>
        /// Defines a HTTP POST method.
        /// </summary>
        Post,

        /// <summary>
        /// Defines a HTTP PUT method.
        /// </summary>
        Put,

        /// <summary>
        /// Defines a HTTP DELETE method.
        /// </summary>
        Delete,

        /// <summary>
        /// Defines a HTTP PATCH method.
        /// </summary>
        Patch
    }

    /// <summary>
    /// Enumerates the available formats for REST communication.
    /// </summary>
    public enum HttpContentType {

        /// <summary>
        /// Defines that the rest communication should be made with XML format.
        /// </summary>
        Xml,

        /// <summary>
        /// Defines that the rest communication should be made with JSON format.
        /// </summary>
        Json
    }

    /// <summary>
    /// Represents the response for a HttpWebRequest.
    /// </summary>
    /// <typeparam name="T">Type of the response of a HttpWebRequest.</typeparam>
    public sealed class WebResponse<T> {

        /// <summary>
        /// Initializes a new instance of the WebResponse class.
        /// </summary>
        public WebResponse() { }

        /// <summary>
        /// Gets the returned HttpStatusCode.
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Gets the returned data.
        /// </summary>
        public T ResponseData { get; internal set; }

        /// <summary>
        /// Gets the raw string returned by the service.
        /// </summary>
        public string RawData { get; set; }
    }

    /// <summary>
    /// REST utility for HTTP communication.
    /// </summary>
    public static class RestClient {

        /// <summary>
        /// Sends an Http request to the specified endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the expected response. Ignored if http verb GET is used.</typeparam>
        /// <param name="dataToSend">Object containing the data to be sent in the request.</param>
        /// <param name="httpVerb">HTTP verb to be using when sending the data.</param>
        /// <param name="httpContentType">Content type of the transferred data.</param>
        /// <param name="destinationEndPoint">Endpoint where the request will be sent to.</param>
        /// <param name="headerCollection">Custom data to be added to the request header.</param>
        /// <param name="allowInvalidCertificate">When set to true, allows the request to be done even if the destination certificate is not valid.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public static WebResponse<T> SendHttpWebRequest<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false) where T : class {

            // Verifica se o endpoint para onde a requisição será enviada foi especificada.
            if (string.IsNullOrWhiteSpace(destinationEndPoint)) { throw new ArgumentNullException("serviceEndpoint", "The serviceEndPoint parameter must not be null."); }

            // Cria a uri para onde a requisição será enviada.
            Uri destinationUri = new Uri(destinationEndPoint);

            if (allowInvalidCertificate == true) {

                // Verifica se certificados inválidos devem ser aceitos.
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            }

            // Inicializa o código de status http a ser retornado.
            HttpStatusCode responseStatusCode = HttpStatusCode.OK;

            // Variável que armazenará o resultado da requisição.
            string returnString = string.Empty;

            using (var httpClient = new HttpClient()) {

                string contentType = (httpContentType == HttpContentType.Json) ? "application/json" : "application/xml";

                // Define o formato das mensagens.
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", contentType);

                // Verifica se deverão ser enviados dados no header.
                if (headerCollection != null && headerCollection.Count > 0) {

                    // Insere cada chave no header da requisição.
                    foreach (string key in headerCollection.Keys) { httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, headerCollection[key].ToString()); }
                }

                StringContent content = null;

                // Verifica se foi especificada a informação a ser enviada.
                if (dataToSend != null) {

                    // Serializa o objeto para o formato especificado.
                    string serializedData = (httpContentType == HttpContentType.Json) ? Serializer.JavasScriptSerialize(dataToSend) : Serializer.XmlSerialize(dataToSend);

                    // Prepara os dados a serem enviados.
                    content = new StringContent(serializedData, System.Text.Encoding.UTF8, contentType);
                }

                HttpResponseMessage httpResponseMessage = null;

                using (HttpRequestMessage request = new HttpRequestMessage() { Method = new HttpMethod(httpVerb.ToString().ToUpperInvariant()), RequestUri = destinationUri, Content = content }) {

                    httpResponseMessage = httpClient.SendAsync(request).Result;
                }

                responseStatusCode = httpResponseMessage.StatusCode;
                returnString = httpResponseMessage.Content.ReadAsStringAsync().Result;
            }

            T returnValue = null;

            // Caso o tipo a ser retornado seja uma string, não executa a deserialização.
            if (typeof(T) == typeof(string)) { returnValue = returnString as T; }
            else {
                // Executa a deserialização adequada.
                returnValue = (httpContentType == HttpContentType.Json) ? Serializer.JavaScriptDeserialize<T>(returnString) : Serializer.XmlDeserialize<T>(returnString);
            }

            // Cria o objeto contendo o resultado da requisição.
            return new WebResponse<T>() { StatusCode = responseStatusCode, ResponseData = returnValue, RawData = returnString };
        }

        /// <summary>
        /// Sends an Http request to the specified endpoint asyncrounously.
        /// </summary>
        /// <typeparam name="T">Type of the expected response.</typeparam>
        /// <param name="dataToSend">Object containing the data to be sent in the request. Ignored if http verb GET is used.</param>
        /// <param name="httpVerb">HTTP verb to be using when sending the data.</param>
        /// <param name="httpContentType">Content type of the transferred data.</param>
        /// <param name="destinationEndPoint">Endpoint where the request will be sent to.</param>
        /// <param name="headerCollection">Custom data to be added to the request header.</param>
        /// <param name="allowInvalidCertificate">When set to true, allows the request to be done even if the destination certificate is not valid.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public static async Task<WebResponse<T>> SendHttpWebRequestAsync<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false) where T : class {

            // Verifica se o endpoint para onde a requisição será enviada foi especificada.
            if (string.IsNullOrWhiteSpace(destinationEndPoint)) { throw new ArgumentNullException("serviceEndpoint", "The serviceEndPoint parameter must not be null."); }

            return await Task.Run<WebResponse<T>>(() => {

                return SendHttpWebRequest<T>(dataToSend, httpVerb, httpContentType, destinationEndPoint, headerCollection, allowInvalidCertificate);
            });
        }
    }
}
