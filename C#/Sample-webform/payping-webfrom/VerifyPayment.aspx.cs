using payping_webfrom.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace payping_webfrom
{
    public partial class VerifyPayment : System.Web.UI.Page
    {
        private const string TOKEN = ""; // توکن ساخته شده در سایت 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var payBody = new VerifyPaymentViewModel
                {
                    RefId = Request.Form["refid"],// شناسه پرداخت برای تایید در سرویس پی پینگ
                    Amount = 100 // این مبلغ بصورت پیش فرض می باشد و شما باید مبلغ مورد محصول مورد نظر را جایگزین نمایید
                };

                var request = new HttpClient();
                request.BaseAddress = new Uri("https://api.payping.ir");
                request.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"bearer {TOKEN}"); // توکن ساخته شده در سایت را باید در این قسمت قرار دهید
                var response = request.PostAsync("/v2/pay/verify", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payBody), System.Text.Encoding.UTF8, "application/json")).Result;

                IEnumerable<string> values;
                response.Headers.TryGetValues("X-PayPingRequest-ID", out values);                

                var PayPing_RequestId = values.ToList()[0];// برای پی گیری تراکنش حتما نگهدار شود
                Label3.Text = $"PayPing_RequestId: {PayPing_RequestId}";

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Label1.Text = "کد محصول در سایت پذیرنده: " + Request.Form["clientrefid"];
                    Label2.Text = "پرداخت شما با موفقیت انجام پذیرفت";
                }
                else
                    Label2.Text = response.Content.ReadAsStringAsync().Result;
            }

        }
    }
}