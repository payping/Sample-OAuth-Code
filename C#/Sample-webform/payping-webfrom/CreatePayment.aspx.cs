using payping_webfrom.Model;
using System;
using System.Net.Http;

namespace payping_webfrom
{
    public partial class CreatePayment : System.Web.UI.Page
    {

        private const string TOKEN = ""; // توکن ساخته شده در سایت 
        private const string GOTOIPG = "https://api.payping.ir/v1/pay/gotoipg/{0}";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void pay_Click(object sender, EventArgs e)
        {
            var payBody = new PaymentViewModel
            {
                Amount = int.Parse(txtAmount.Text),
                ClientRefId = txtClientRefId.Text,
                PayerName = txtPayerName.Text,
                PayerIdentity = txtPayerIdentity.Text,
                Description = txtDescription.Text,
                ReturnUrl = txtReturnUrl.Text
            };

            var request = new HttpClient();
            request.BaseAddress = new Uri("https://api.payping.ir");
            request.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"bearer {TOKEN}"); // توکن ساخته شده در سایت را باید در این قسمت قرار دهید
            var response = request.PostAsync("/v2/pay", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payBody), System.Text.Encoding.UTF8, "application/json")).Result;
            var responseCode = response.Content.ReadAsStringAsync().Result;

            var payCode = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentCodeVoewModel>(responseCode);

            Response.Redirect(string.Format(GOTOIPG, payCode.Code));

        }
    }
}