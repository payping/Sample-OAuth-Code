using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PayPing.Ipg.Models;

namespace PayPing.Ipg.Controllers
{

    [RoutePrefix("v1/[controller]/[action]")]
    public class UserController : Controller
    {
        #region PayPing AuthorizationCode-With PKCE Flow

        //مشخصات سرور پی پینگ
        public const string PayPingOauthPath = "https://oauth.payping.ir";
        public const string PayPingAuthorizeUrl = PayPingOauthPath + "/connect/authorize";
        public const string PayPingTokeneURL = "connect/token";

        //مشخصات کلاینت
        private const string ClientId = "------------------------";
        private const string ClientSecret = "------------------------";

        //دسترسی های درخواستی کلاینت
        private const string Scopes =
            "profile openid email pay:write pay:read";

        //آدرس بازگشت از پی پینگ
        private const string RedirectUrl = @"http://localhost:5015/v1/user/GetAcessToken";

        //پروتکل ها
        private const string ResponseType = "code";
        private const string GrantType = "authorization_code";

        //پارامتر های امنیتی ( برای سمپل کد این مقادیر به صورت استاتیک تعیین شده اند)
        public static List<PKCEViewModel> PKCEs = new List<PKCEViewModel>();

        public ActionResult Index()
        {
            return View();
        }


        //اکشن اول که کاربر با زدن دکمه ورود با پی پینگ صدا زده میشود
        //http://localhost:5015/v1/user/GetPayPingAccess
        [HttpGet]
        public ActionResult GetPayPingAccess()
        {
            //ساخت کد وریفایر و کد چلنج و ذخیره آنها
            var tempVerifier = GetCodeVerifier();
            var tPKCE = new PKCEViewModel()
            {
                Code = new Random(100000).Next().ToString(),
                CodeVerifier = tempVerifier,
                CodeChallenge = GetCodeChallenge(tempVerifier)
            };
            PKCEs.Add(tPKCE);

            var getAuthorizationUrl = PayPingAuthorizeUrl +
                                      "?client_id=" + ClientId +
                                      "&response_type=" + ResponseType +
                                      "&redirect_uri=" + RedirectUrl +
                                      "&scope=" + Scopes +
                                      "&code_challenge=" + tPKCE.CodeChallenge +
                                      "&code_challenge_method=" + "S256" +
                                      "&state=" + tPKCE.Code;


            return Redirect(getAuthorizationUrl);
        }

        //اکشن آدرس بازگشت که در مرحله ساخت کلاینت ست شده
        [HttpGet]
        public async Task<ActionResult> GetAcessToken()
        {
            //دریافت کد از کوری استرینگ
            var authorizationCode = Request.QueryString["code"];

            // در یافت state ارسالی در درخواست اول
            var state = Request.QueryString["state"];

            //پیدا کردن کد چلنج 
            var pkce = PKCEs.First(x => x.Code == state);

            string json = null;

            //درخواست در فرمت فرم دیتا (نه Json)
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(PayPingOauthPath);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", GrantType),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("code_verifier", pkce.CodeVerifier),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUrl)
                });
                var httpResponseMessage = await client.PostAsync(PayPingTokeneURL, content);
                json = await httpResponseMessage.Content.ReadAsStringAsync();
            }

            dynamic results = JsonConvert.DeserializeObject<dynamic>(json);
            string accessToken = results.access_token;
            PKCEs.Remove(pkce);

            return Redirect($"/v1/payping/index?accessToken={accessToken}");
        }


        //ساخت کد وریفایر
        private string GetCodeVerifier()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[32];
            rnd.NextBytes(b);
            return Base64UrlEncoder.Encode(b);
        }

        //ساخت کد چلنج
        private string GetCodeChallenge(string codeVerifyer)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(codeVerifyer);
            byte[] hash = sha256.ComputeHash(bytes);
            var codeChallenge = Base64UrlEncoder.Encode(hash);
            return codeChallenge;
        }

        #endregion
    }
}