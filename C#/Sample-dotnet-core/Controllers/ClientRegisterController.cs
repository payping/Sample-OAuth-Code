using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using test_oauth.Models;

namespace test_oauth.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ClientRegisterController : ControllerBase
    {
        private readonly RestClient _client;

        //Client Rgister ّInfo
        private const string AccessToken = "[YOUR_ACCESS_TOKEN]";
        public const string PayPingClientRegisterUrl = "https://api.payping.ir/v1/client";

        //Redirect to Oauth consent page to get Access Token after register proccess
        public const string PayPingOauthConsentUrl = "http://localhost:5000/Oauth/GetPayPingAccess";

        public ClientRegisterController()
        {
            _client = new RestClient(PayPingClientRegisterUrl);
            _client.AddDefaultHeader("Authorization", "Bearer " + AccessToken);
        }



        [HttpPost("{Email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailExist(string Email)
        {
            try
            {
                var request = new RestRequest("EmailExist", Method.GET);
                request.AddQueryParameter("Email", Email);

                var result = await _client.ExecuteTaskAsync<ResultVM>(request);
                return Ok(result.Data);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ClientRegister([FromBody]ClientRegisterVM model)
        {
            model.ReturnUrl = PayPingOauthConsentUrl;
            try
            {
                //Check Existance of User Email
                var request = new RestRequest("EmailExist", Method.GET);
                request.AddQueryParameter("Email", model.Email);
                var result = await _client.ExecuteTaskAsync<ResultVM>(request);

                //If result is FALSE, then you can register user
                if (result.StatusCode == HttpStatusCode.OK && !result.Data.exist)
                {
                    var registerRequest = new RestRequest("ClientRegisterInit", Method.POST);
                    registerRequest.AddJsonBody(model);
                    var resultId = await _client.ExecuteTaskAsync<ResultVM>(registerRequest);

                    //If user Registered successfully, then redirect it to this url(PayPing Dashboard)
                    //then user must click on notification to redirect itself to the Oauth Consent page
                    if (resultId.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(resultId.Data.Id))
                        return Ok("https://oauth.payping.ir/client/ClientRegister?registerId=" + resultId.Data.Id);
                    else
                        return BadRequest("مشکلی در دریافت آیدی ثبت نام وجود دارد");
                }
                else
                {
                    return BadRequest("مشکلی در ایمیل وارد شده وجود دارد");
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}