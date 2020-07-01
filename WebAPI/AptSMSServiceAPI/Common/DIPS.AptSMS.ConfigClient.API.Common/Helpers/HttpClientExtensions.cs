using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DIPS.AptSMS.ConfigClient.API.Common.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ContentAs<T>(this HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            JsonConvert.DeserializeObject<T>(data);
        }
    }
}
