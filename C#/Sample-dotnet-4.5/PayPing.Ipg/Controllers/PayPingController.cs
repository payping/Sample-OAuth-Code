using PayPing.Ipg.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using config = System.Configuration.ConfigurationManager;

namespace PayPing.Ipg.Controllers
{

    [RoutePrefix("v1/[controller]/[action]")]
    public class PayPingController : Controller
    {
        private static string _accessToken { get; set; }

        public ActionResult Index()
        {
            _accessToken = Request.QueryString["accessToken"];
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Pay(RequestPayment model)
        {
            // ساخت کد پرداخت به منظور هدایت به درگاه بانکی
            var payCode = await CallApi<PaymentResult>(config.AppSettings["PAYMENT_URL"], model);

            //هدایت کاربر به درگاه بانک
            return Redirect(config.AppSettings["GOTOIPG_URL"] + payCode.Code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refid">شناسه پرداخت می باشد که به کمک آن یک پرداخت تایید و یا اصطلاحا وریفای می شود</param>
        /// <param name="clientrefid">کد و یا عبارت اختصاصی است که توسط کاربر برای سرویس پرداخت در زمان ساخت پرداخت ارسال شده است وبعد از برگشت از درگاه برای پذیرنده ارسال می گردد            </param>
        /// <param name="cardnumber">
        /// شماره کارت پرداخت کننده. برای داشتن این پارامتر باید عبارت rrn
        /// در پایان ReturnUrl اضافه نمایید
        /// تا بعد از برگشت از درگاه، به آدرس برگشت اضافه گردد
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ActionResult> PaypingVerify(string refid, string clientrefid, string cardnumber)
        {
            var verifyBody = new RequestVerify
            {
                Amount = 100, // مبلغ به توامن می باشد
                RefId = refid
            };

            // اگر وریفای با موفقیت انجام پذیرد، تنها وضعیت 200 را برگشت می دهد و حاوی هیچ دیتایی نیست
            // اما اگر به هر دلیلی ادامه فرایند وریفای مقدور نباشد، 400 برگشت می دهد به همراه یک متن خطا
            var result = await CallApi<ResponseVerifyViewModel>(config.AppSettings["VERIFY_URL"], verifyBody);
            return View("verify", result);
        }
        
        private async Task<T> CallApi<T>(string apiUrl, object value, bool isToken = false) where T : class
        {
            var url = new Uri(apiUrl);
            var baseHost = $"{url.Scheme}://{url.Authority}";
            var method = $"{url.AbsolutePath}";
            var _client = new RestClient(baseHost) { Timeout = 20000 };
            var request = new RestRequest(method, Method.POST);
            if (isToken)
                request.AddObject(value);
            else
            {
                _client.AddDefaultHeader("Authorization", $"bearer {_accessToken}");
                request.AddJsonBody(value);
            }
            var response = await _client.ExecuteTaskAsync<T>(request);
            if (method.Contains("verify"))
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = new ResponseVerifyViewModel { Message = "پرداخت شما با موفقیت انجام پذیرفت" };
                    return result as T;
                }
                else
                {
                    var result = new ResponseVerifyViewModel { Message = response.Content };
                    return result as T;
                }
            return response.Data;
        }
    }
}