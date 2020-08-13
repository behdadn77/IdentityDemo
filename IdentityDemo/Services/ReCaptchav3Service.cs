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
        public virtual async Task<GoogleRespo> RecVer(string token)
        {
            GoogleReCaptchaData reCaptchaData = new GoogleReCaptchaData()
            {
                response = token,
                secret = options.Value.SecretKey
            };
            HttpClient client = new HttpClient();
            var response = 
                await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaData.secret}&response={reCaptchaData.response}");
            var capresp = JsonConvert.DeserializeObject<GoogleRespo>(response);
            return capresp;
        }
    }
    public class GoogleReCaptchaData
    {
        public string response { get; set; }
        public string  secret { get; set; }
    }

    public class GoogleRespo
    {
        public string success { get; set; }
        public string score { get; set; }
        public string action { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
    }
}
