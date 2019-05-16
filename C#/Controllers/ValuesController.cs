using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace test_oauth.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ValuesController : ControllerBase
    {
        //Server Info
        public const string PayPingOauthPath = "https://oauth.payping.ir";
        public const string PayPingAuthorizeUrl = PayPingOauthPath + "/connect/authorize";
        public const string PayPingTokeneURL = "connect/token";

        //Client Info
        private const string ClientId = "[YourClientId]";
        private const string ClientSecret = "[YourClientSecret]";


        private const string Scopes = "[Your Requested Scopes Separated by Space]";

        //you should replace your own RedirectURL
        private const string RedirectUrl = @"http://localhost:5000/api/GetAcessToken";

        //ClientType Info
        private const string ResponseType = "code";
        private const string GrantType = "authorization_code";

        //Security Info (for test purposes its static but it shouldn't be static)
        public static string CodeVerifier;
        public static string CodeChallenge;

        //First Url That gets Called for initiating the token process
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPayPingAccess()
        {
            CodeVerifier = GetCodeVerifier();
            var getAuthorizationUrl = PayPingAuthorizeUrl +
                        "?client_id=" + ClientId +
                        "&response_type=" + ResponseType +
                        "&redirect_uri=" + RedirectUrl +
                        "&scope=" + Scopes +
                        "&code_challenge=" + GetCodeChallenge(CodeVerifier) +
                        "&code_challenge_method=" + "S256" +
                        "&state=" + "[OPTIONAL]";

            return Redirect(getAuthorizationUrl);
        }

        //redirecturl That gets Called after user logged in payping and grants the consent page
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAcessToken()
        {
            // Get Authorization Code from Querystring (use this to get access token)
            var authorizationCode = HttpContext.Request.Query["code"].ToString();

            // [Optional] The Optional State That was sent with the first request to getAuthorizationUrl 
            var state = HttpContext.Request.Query["state"].ToString();


            string json = null;

            //Request should be in format => FormData *NOT application/json*
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(PayPingOauthPath);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", GrantType),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("code_verifier", CodeVerifier),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUrl)
                });
                var httpResponseMessage = await client.PostAsync(PayPingTokeneURL, content);
                json = await httpResponseMessage.Content.ReadAsStringAsync();
            }

            dynamic results = JsonConvert.DeserializeObject<dynamic>(json);
            string accessToken = results.access_token;

            return Ok(accessToken);
        }

        private string GetCodeVerifier()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[32];
            rnd.NextBytes(b);
            CodeVerifier = Base64UrlEncoder.Encode(b);
            return CodeVerifier;
        }

        private string GetCodeChallenge(string codeVerifyer)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(codeVerifyer);
            byte[] hash = sha256.ComputeHash(bytes);
            var codeChallenge = Base64UrlEncoder.Encode(hash);
            return codeChallenge;
        }

    }
}
