using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace dotMailer.Api
{
    public partial class Client
    {
        private readonly HttpClient httpClient;

        public Client(string username, string password)
        {
            httpClient = GetHttpClient(username, password);
        }

        #region Helpers

        private HttpClient GetHttpClient(string username, string password)
        {
            var client = new HttpClient { BaseAddress = new Uri(BaseAddress) };

            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);

            return client;
        }

        private ServiceResult Result(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ? SuccessResult(response) : ErrorResult(response);
        }

        private ServiceResult<T> Result<T>(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ? SuccessResult<T>(response) : ErrorResult<T>(response);
        }

        private ServiceResult SuccessResult(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;
            return new ServiceResult(true, result);
        }

        private ServiceResult<T> SuccessResult<T>(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsAsync<T>(new[] { jsonFormatter }).Result;
            return new ServiceResult<T>(true, result);
        }

        private ServiceResult ErrorResult(HttpResponseMessage response)
        {
            var message = GetErrorMessage(response);
            return new ServiceResult(false, message);
        }

        private ServiceResult<T> ErrorResult<T>(HttpResponseMessage response)
        {
            var message = GetErrorMessage(response);
            return new ServiceResult<T>(false, default(T), message);
        }

        private string GetErrorMessage(HttpResponseMessage response)
        {
            var errorInfo = response.Content.ReadAsAsync<RequestErrorInfo>().Result;
            var message = string.Format("Failed to {0} object (Status Code: {1}, Status Description: {2}, Detail: {3})", response.RequestMessage.Method.Method, (int)response.StatusCode, response.StatusCode, errorInfo.Message);
            return message;
        }

        #endregion

        #region Formatters

        private static MediaTypeFormatter jsonFormatter;
        private static MediaTypeFormatter JsonFormatter
        {
            get
            {
                if (jsonFormatter == null)
                {
                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new StringEnumConverter());
                    jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = settings };
                }
                return jsonFormatter;
            }
        }

        #endregion

        #region Get

        private ServiceResult Get(Request request)
        {
            var response = httpClient.GetAsync(request.Url).Result;
            return Result(response);
        }

        private ServiceResult<T> Get<T>(Request request)
        {
            var response = httpClient.GetAsync(request.Url).Result;
            return Result<T>(response);
        }

        #endregion

        #region Post

        private ServiceResult<T> Post<T>(Request request)
        {
            var response = httpClient.PostAsync(request.Url, string.Empty, JsonFormatter).Result;
            return Result<T>(response);
        }

        private ServiceResult<T> Post<T>(Request request, T data)
        {
            var response = httpClient.PostAsync(request.Url, data, JsonFormatter).Result;
            return Result<T>(response);
        }

        private ServiceResult<TOutput> Post<TOutput, TInput>(Request request, TInput data)
        {
            var response = httpClient.PostAsync(request.Url, data, JsonFormatter).Result;
            return Result<TOutput>(response);
        }

        #endregion

        #region Put

        private ServiceResult<T> Put<T>(Request request, T data)
        {
            var response = httpClient.PutAsync(request.Url, data, JsonFormatter).Result;
            return Result<T>(response);
        }

        #endregion

        #region Delete

        private ServiceResult Delete(Request request)
        {
            var response = httpClient.DeleteAsync(request.Url).Result;
            return Result(response);
        }

        private ServiceResult<T> Delete<T>(Request request)
        {
            var response = httpClient.DeleteAsync(request.Url).Result;
            return Result<T>(response);
        }

        #endregion
    }
}
