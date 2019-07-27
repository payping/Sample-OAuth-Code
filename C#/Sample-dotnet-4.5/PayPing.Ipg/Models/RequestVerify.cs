using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPing.Ipg.Models
{
    public class RequestVerify
    {
        public int Amount { get; set; }
        public string RefId { get; set; }
    }
}