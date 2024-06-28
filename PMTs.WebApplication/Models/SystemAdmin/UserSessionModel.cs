using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Models.SystemAdmin
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the User Name")]
        public string USERNAME { get; set; }
        [Required(ErrorMessage = "Please enter the Password")]
        public string PASSWORD { get; set; }


        public DateTime LAST_PASSWORD_CHANGED_DATE { get; set; }
        [DisplayName("Domain: ")]
        public string UserDomain { get; set; }
    }
}
