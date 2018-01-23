using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS_Udemy.Models.ViewModels.Account
{
    public class LoginUserVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PassWord { get; set; }
        public bool RememberMe { get; set; }
    }
}