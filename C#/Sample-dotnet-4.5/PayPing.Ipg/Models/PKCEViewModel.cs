namespace PayPing.Ipg.Models
{
    public class PKCEViewModel
    {
        public string Code { get; set; }
        public string CodeVerifier { get; set; }
        public string CodeChallenge { get; set; }
    }
}