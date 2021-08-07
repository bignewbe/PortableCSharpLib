using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortableCSharpLib.Interace
{
    public interface IHttpRequestCommon
    {
        Task<HttpResponseMessage> GetResponseWithHeaderReturnedAsync(string url, int timeout = 5000);
        Task<HttpResponseMessage> GetAsync(string url, int timeout = 5000);
        Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> queryParam, int timeout = 5000);
        Task<HttpResponseMessage> GetWithParamAsync(string url, string paramName, string paramValue, int timeout = 5000);
        Task<HttpResponseMessage> PostAsync(string url, string postData, int timeout = 5000);
        Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> formData, int timeout = 5000);
        void AddHeader(string key, string value); 
    }
}