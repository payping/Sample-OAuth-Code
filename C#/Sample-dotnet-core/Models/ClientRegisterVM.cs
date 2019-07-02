using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test_oauth.Models
{
    public class ClientRegisterVM
    {
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string NationalCode { get; set; }

        public DateTime? BirthDay { get; set; }

        public string Sheba { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
}