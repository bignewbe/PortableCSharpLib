using PortableCSharpLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PortableCSharpLib.Model
{
    public class HttpRequestCommon : IHttpRequestCommon
    {
        //protected HttpClientHandler _handler;
        //protected CookieContainer _cookieContainer;
        protected HttpClient _client;

        public HttpRequestCommon(bool isByPassSslCertificate=false, bool isAddCookieContainer=false)
        {
            var _handler = new HttpClientHandler();
            
            if (isByPassSslCertificate)
                _handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            if (isAddCookieContainer)
            {
                var _cookieContainer = new CookieContainer();
                _handler.CookieContainer = _cookieContainer;
            }

            _client = new HttpClient(_handler);
        }

        public void AddHeader(string key, string value)
        {
            //throw new NotImplementedException();
            _client.DefaultRequestHeaders.Add(key, value);
        }

        public async Task<HttpResponseMessage> GetResponseWithHeaderReturnedAsync(string url, int timeout = 5000)
        {
            var task1 = _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (await Task.WhenAny(task1, Task.Delay(timeout)) == task1)
            {
                var response = task1.Result;
                response.EnsureSuccessStatusCode();
                return response;
            }
            else
            {
                throw new TimeoutException("Timeout occur in GetResponseWithHeaderReturnedAsync");
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url, int timeout = 5000)
        {
            var task = _client.GetAsync(url);
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task) {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                return response;
            }
            else {
                throw new TimeoutException("Timeout occur in GetAsync");
            }
        }

        public async Task<HttpResponseMessage> GetWithParamAsync(string url, string paramName, string paramValue, int timeout = 5000)
        {
            url = string.Format("{0}?{1}={2}", url, paramName, paramValue);
            var task = _client.GetAsync(url);
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                return response;
            }
            else
            {
                throw new TimeoutException("Timeout occur in GetWithParamAsync");
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string url, string postData, int timeout = 5000)
        {
            var task = _client.PostAsync(url, new StringContent(postData, Encoding.UTF8, "application/json"));
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task) {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                return response;
            }
            else {
                throw new TimeoutException("Timeout occur in PostAsync");
            }
        }

        public async Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> formData, int timeout = 5000)
        {
            var task = _client.PostAsync(url, new FormUrlEncodedContent(formData));
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                return response;
            }
            else
            {
                throw new TimeoutException("Timeout occur in PostFormAsync");
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> queryParam, int timeout = 5000)
        {
            var encodedContent = new FormUrlEncodedContent(queryParam);
            var queryStr = await encodedContent.ReadAsStringAsync();           
            if (!string.IsNullOrEmpty(queryStr))
                url = $"{url}?{queryStr}";

            return await this.GetAsync(url, timeout);
        }
    }
}
