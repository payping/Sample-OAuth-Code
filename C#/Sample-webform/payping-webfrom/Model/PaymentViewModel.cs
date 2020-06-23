using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace payping_webfrom.Model
{
    public class PaymentViewModel
    {
        /// <summary>
        /// مبلغ پرداخت
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// نام کاربری پرداخت کننده در پی پینگ
        /// که می تواند نام کاربر و یا شماره موبایل باشد (اختیاری)
        /// </summary>
        public object PayerIdentity { get; set; }
        /// <summary>
        /// نام پرداخت کننده (اختیاری)
        /// </summary>
        public string PayerName { get; set; }
        /// <summary>
        /// توضیح پرداخت
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// آدرس برگشت بعد از برگشت از درگاه بانک که باید به آن redirect شود
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// هر مقداری بعد از برگشت از درگاه عینا به سمت کلاینت برگشت داده می شود
        /// </summary>
        public string ClientRefId { get; set; }
    }
}