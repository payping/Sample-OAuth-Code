using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPing.Ipg.Models
{
    public class ResponseTokenViewModel
    {
        public string access_token { get; set; }
        public long expires_in { get; set; }
        public string token_type { get; set; }

    }
}