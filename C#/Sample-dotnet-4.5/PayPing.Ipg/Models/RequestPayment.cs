using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayPing.Ipg.Models
{
    public class RequestPayment
    {
        private string _userIdentity;
        /// <summary>
        /// مبلغ
        /// </summary>
        [Required]
        //[Range(100, 50000000, ErrorMessageResourceName = "مبلغ نمی تواند کمتر از 100 و بیشتر از 50,000,000 تومان باشد")]
        public int Amount { get; set; }
        /// <summary>
        /// نام کاربری پرداخت کنند (در صورت وجود)
        /// </summary>
        public string PayerIdentity { get => _userIdentity; set => _userIdentity = value.ToLower(); }
        /// <summary>
        /// نام پرداخت کننده
        /// </summary>
        public string PayerName { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// آدرس صفحه برگشت
        /// </summary>
        [Required]
        //[Url(ErrorMessageResourceName = "صفحه بازگشت باید حتما یک آدرس اینترنتی کامل همراه با Http(s) باشد")]
        public string ReturnUrl { get; set; }
        /// <summary>
        /// کد ارسالی توسط کاربر
        /// </summary>
        public string ClientRefId { get; set; }
    }
}