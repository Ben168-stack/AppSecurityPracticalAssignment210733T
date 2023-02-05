using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace AppSecurityPracticalAssignment210733T.Services
{
    public class GoogleCaptchaService
    {

        private readonly IOptionsMonitor<GoogleCaptchaConfig> _config;
        public GoogleCaptchaService(IOptionsMonitor<GoogleCaptchaConfig> config)
        {
            _config = config;
        }

        public async Task<bool> VerifyToken(string token)
        {
            try
            {
                var url = $"https://www.google.com/recaptcha/api/siteverify?secret={_config.CurrentValue.SecretKey}&response={token}";
                using(var client = new HttpClient())
                {
                    var httpResult = await client.GetAsync(url);
                    if(httpResult.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    var responseString = await httpResult.Content.ReadAsStringAsync();    

                    var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);

                    //Check google valid anti bot score.
                    return googleResult.success && googleResult.score >= 0.5;
                }

            }catch(Exception e)
            {
                Console.WriteLine("Error Occured Verifying Token ", e);
                return false;
            }
        }

    }
}
