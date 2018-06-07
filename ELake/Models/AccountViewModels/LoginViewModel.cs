using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
