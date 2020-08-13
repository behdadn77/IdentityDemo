using IdentityDemo.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityDemo.Services
{
    public class ReCaptchav3Service
    {
        private readonly IOptions<ReCaptchav3Options> options;

        public ReCaptchav3Service(IOptions<ReCaptchav3Options> options)
        {
            this.options = options;
        }
        public virtual async Task<GoogleResponse> Verify(string token)
        {
            GoogleReCaptchaData reCaptchaData = new GoogleReCaptchaData()
            {
                Response = token,
                Secret = options.Value.SecretKey
            };
            HttpClient client = new HttpClient();
            var response = 
                await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaData.Secret}&response={reCaptchaData.Response}");
            var capresp = JsonConvert.DeserializeObject<GoogleResponse>(response);
            return capresp;
        }
    }
    public class GoogleReCaptchaData
    {
        public string Response { get; set; }
        public string  Secret { get; set; }
    }

    public class GoogleResponse
    {
        public bool Success { get; set; }
        public Double Score { get; set; }
        public string Action { get; set; }
        public DateTime Challenge_ts { get; set; }
        public string Hostname { get; set; }
    }
}
