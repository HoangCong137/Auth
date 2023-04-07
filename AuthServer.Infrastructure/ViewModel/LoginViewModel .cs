using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace AuthServer.Infrastructure.ViewModel
{
    public class LoginViewEmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class LoginViewMobileModel
    {
        [Required]
        [Phone]
        public string Mobile { get; set; }
        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class LoginViewFacebookEmailModel
    {
        [Required]
        public string FacebookId { get; set; }
        public string Email { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class LoginViewAppleEmailModel
    {
        [Required]
        public string AppleId { get; set; }
        public string Email { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
