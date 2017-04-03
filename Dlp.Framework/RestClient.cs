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
        Json,

        /// <summary>
        /// Defines that the rest communication should be made using plain text.
        /// </summary>
        PlainText
    }

    /// <summary>
    /// Represents the response for a HttpWebRequest.
    /// </summary>
    public class WebResponse {

        /// <summary>
        /// Initializes a new instance of the WebResponse class.
        /// </summary>
        public WebResponse() { }

        /// <summary>
        /// Gets the returned HttpStatusCode.
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Gets the flag that indicates whether the StatusCode represents a successful operation.
        /// </summary>
        public bool IsSuccessStatusCode { get; internal set; }

        /// <summary>
        /// Gets a dynamic object containing the returned object.
        /// </summary>
        public dynamic ResponseData { get; internal set; }

        /// <summary>
        /// Gets the raw string returned by the service.
        /// </summary>
        public string RawData { get; set; }
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
        /// Gets the flag that indicates whether the StatusCode represents a successful operation.
        /// </summary>
        public bool IsSuccessStatusCode { get; internal set; }

        /// <summary>
        /// Gets the returned data.
        /// </summary>
        public T ResponseData { get; internal set; }

        /// <summary>
        /// Gets the raw string returned by the service.
        /// </summary>
        public string RawData { get; set; }
    }

    public interface IRestClient {

        /// <summary>
        /// Sends an Http request to the specified endpoint.
        /// </summary>
        /// <param name="dataToSend">Object containing the data to be sent in the request.</param>
        /// <param name="httpVerb">HTTP verb to be using when sending the data.</param>
        /// <param name="destinationEndPoint">Endpoint where the request will be sent to.</param>
        /// <param name="headerCollection">Custom data to be added to the request header.</param>
        /// <param name="allowInvalidCertificate">When set to true, allows the request to be done even if the destination certificate is not valid.</param>
        /// <param name="timeoutInSeconds">Request timeout in seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        WebResponse Send(object dataToSend, HttpVerb httpVerb, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90);

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
        /// <param name="timeoutInSeconds">Request timeout in seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        WebResponse<T> Send<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) where T : class;
    }

    /// <summary>
    /// REST utility for HTTP communication.
    /// </summary>
    public sealed class RestClient : IRestClient {

        /// <summary>
        /// Sends an Http request to the specified endpoint.
        /// </summary>
        /// <param name="dataToSend">Object containing the data to be sent in the request.</param>
        /// <param name="httpVerb">HTTP verb to be using when sending the data.</param>
        /// <param name="destinationEndPoint">Endpoint where the request will be sent to.</param>
        /// <param name="headerCollection">Custom data to be added to the request header.</param>
        /// <param name="allowInvalidCertificate">When set to true, allows the request to be done even if the destination certificate is not valid.</param>
        /// <param name="timeoutInSeconds">Request timeout in seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public WebResponse Send(object dataToSend, HttpVerb httpVerb, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) {

            return RestClient.SendHttpWebRequest(dataToSend, httpVerb, destinationEndPoint, headerCollection, allowInvalidCertificate, timeoutInSeconds);
        }

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
        /// <param name="timeoutInSeconds">Request timeout in seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public WebResponse<T> Send<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) where T : class {

            return RestClient.SendHttpWebRequest<T>(dataToSend, httpVerb, httpContentType, destinationEndPoint, headerCollection, allowInvalidCertificate, timeoutInSeconds);
        }

        /// <summary>
        /// Sends an Http request to the specified endpoint.
        /// </summary>
        /// <param name="dataToSend">Object containing the data to be sent in the request.</param>
        /// <param name="httpVerb">HTTP verb to be using when sending the data.</param>
        /// <param name="destinationEndPoint">Endpoint where the request will be sent to.</param>
        /// <param name="headerCollection">Custom data to be added to the request header.</param>
        /// <param name="allowInvalidCertificate">When set to true, allows the request to be done even if the destination certificate is not valid.</param>
        /// <param name="timeoutInSeconds">Request timeout in seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public static WebResponse SendHttpWebRequest(object dataToSend, HttpVerb httpVerb, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) {

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
            bool isSuccessStatusCode = false;

            using (var httpClient = new HttpClient()) {

                string contentType = "application/json";

                // Define o formato das mensagens.
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", contentType);

                // Define o timeout da operação.
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);

                // Verifica se deverão ser enviados dados no header.
                if (headerCollection != null && headerCollection.Count > 0) {

                    // Insere cada chave no header da requisição.
                    foreach (string key in headerCollection.Keys) { httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, headerCollection[key].ToString()); }
                }

                StringContent content = null;

                // Verifica se foi especificada a informação a ser enviada.
                if (dataToSend != null) {

                    // Serializa o objeto para o formato especificado.
                    string serializedData = Serializer.NewtonsoftSerialize(dataToSend);

                    // Prepara os dados a serem enviados.
                    content = new StringContent(serializedData, System.Text.Encoding.UTF8, contentType);
                }

                HttpResponseMessage httpResponseMessage = null;

                using (HttpRequestMessage request = new HttpRequestMessage() { Method = new HttpMethod(httpVerb.ToString().ToUpperInvariant()), RequestUri = destinationUri, Content = content }) {

                    httpResponseMessage = httpClient.SendAsync(request).Result;
                }

                responseStatusCode = httpResponseMessage.StatusCode;
                returnString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                isSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            }

            dynamic returnData = null;

            try {
                // Executa a deserialização.
                returnData = Serializer.DynamicDeserialize(returnString);
            }
            catch (Exception ex) {
                returnData = null;
            }

            // Cria o objeto contendo o resultado da requisição.
            return new WebResponse() { StatusCode = responseStatusCode, IsSuccessStatusCode = isSuccessStatusCode, ResponseData = returnData, RawData = returnString };
        }

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
        /// <param name="timeoutInSeconds">Operation timeout. Default 90 seconds.</param>
        /// <returns>Returns an WebResponse as a Task, containing the result of the request.</returns>
        public static WebResponse<T> SendHttpWebRequest<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) where T : class {

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
            bool isSuccessStatusCode = false;

            using (var httpClient = new HttpClient()) {

                string contentType = "text/plain";

                if (httpContentType == HttpContentType.Json) { contentType = "application/json"; }
                else if (httpContentType == HttpContentType.Xml) { contentType = "application/xml"; };

                // Define o formato das mensagens.
                //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", contentType);

                // Define o timeout da operação.
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);

                // Verifica se deverão ser enviados dados no header.
                if (headerCollection != null && headerCollection.Count > 0) {

                    // Insere cada chave no header da requisição.
                    foreach (string key in headerCollection.Keys) { httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, headerCollection[key].ToString()); }
                }

                StringContent content = null;

                // Verifica se foi especificada a informação a ser enviada.
                if (dataToSend != null) {

                    // Serializa o objeto para o formato especificado.
                    string serializedData = (httpContentType == HttpContentType.Xml) ? Serializer.XmlSerialize(dataToSend) : Serializer.NewtonsoftSerialize(dataToSend);

                    // Prepara os dados a serem enviados.
                    content = new StringContent(serializedData, System.Text.Encoding.UTF8, contentType);
                }

                HttpResponseMessage httpResponseMessage = null;

                using (HttpRequestMessage request = new HttpRequestMessage() { Method = new HttpMethod(httpVerb.ToString().ToUpperInvariant()), RequestUri = destinationUri, Content = content }) {
                    
                    httpResponseMessage = httpClient.SendAsync(request).Result;
                }

                responseStatusCode = httpResponseMessage.StatusCode;
                returnString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                isSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
            }

            T returnValue = null;

            // Caso o tipo a ser retornado seja uma string, não executa a deserialização.
            if (typeof(T) == typeof(string)) { returnValue = returnString as T; }
            else {
                try {
                    // Executa a deserialização adequada.
                    returnValue = (httpContentType == HttpContentType.Json) ? Serializer.NewtonsoftDeserialize<T>(returnString) : Serializer.XmlDeserialize<T>(returnString);
                }
                catch (Exception ex) {
                    returnValue = null;
                }
            }

            // Cria o objeto contendo o resultado da requisição.
            return new WebResponse<T>() { StatusCode = responseStatusCode, IsSuccessStatusCode = isSuccessStatusCode, ResponseData = returnValue, RawData = returnString };
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
        public static async Task<WebResponse<T>> SendHttpWebRequestAsync<T>(object dataToSend, HttpVerb httpVerb, HttpContentType httpContentType, string destinationEndPoint, NameValueCollection headerCollection, bool allowInvalidCertificate = false, int timeoutInSeconds = 90) where T : class {

            // Verifica se o endpoint para onde a requisição será enviada foi especificada.
            if (string.IsNullOrWhiteSpace(destinationEndPoint)) { throw new ArgumentNullException("serviceEndpoint", "The serviceEndPoint parameter must not be null."); }

            return await Task.Run<WebResponse<T>>(() => {

                return SendHttpWebRequest<T>(dataToSend, httpVerb, httpContentType, destinationEndPoint, headerCollection, allowInvalidCertificate);
            });
        }
    }
}
