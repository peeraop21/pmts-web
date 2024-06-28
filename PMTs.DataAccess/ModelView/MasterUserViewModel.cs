using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{
    public class MasterUserViewModel
    {
        [Key]
        public int USER_ID { get; set; }
        [Required]
        [DisplayName("USERNAME")]
        public string USER_NAME { get; set; }

        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [DisplayName("PASSWORD")]
        [DataType(DataType.Password)]
        public string PASSWORD { get; set; }
        //  [DisplayName("Role")]

        //  public int DEFAULT_ROLE_ID { get; set; } 
        [DisplayName("DOMAIN")]
        public string USER_DOMAIN { get; set; }

        [Required]
        [DisplayName("SALE ORG")]
        public string SALE_ORG { get; set; }
        [Required]
        [DisplayName("PLANT CODE")]
        public string PLANT_CODE { get; set; }
        //public int SALE_ORG { get; set; }
        [Required]
        [DisplayName("FirstName[TH]")]
        public string FIRST_NAME_TH { get; set; }
        [Required]
        [DisplayName("LastName[TH]")]
        public string LAST_NAME_TH { get; set; }
        [Required]
        [DisplayName("FirstName[EN]")]
        public string FIRST_NAME_EN { get; set; }

        [Required]
        [DisplayName("LastName[EN]")]
        public string LAST_NAME_EN { get; set; }
        public string TELEPHONE { get; set; }
        public string MOBILE_NO { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        public string PASSWORD_HINT { get; set; }
        public string COMMENT { get; set; }
        //    public string LAST_PASSWORD_CHANGED_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;
        public string UPDATED_BY { get; set; }
        //   public bool IS_RECEIVE_EMAIL { get; set; }
        //    public bool IS_FLAG_DELETE { get; set; }
        public int DEFAULT_ROLE_ID { get; set; }

        public bool IS_FLAG_DELETE { get; set; }
        public string Roledata { get; set; }

        public virtual MasterRole Role { get; set; }

    }

    public class UserViewModel
    {
        public int USER_ID { get; set; }
        public int RowNumber { get; set; }
        public string USER_NAME { get; set; }
        public string Password { get; set; }
        public string FIRST_NAME_TH { get; set; }
        public string LAST_NAME_TH { get; set; }
        public bool IS_FLAG_DELETE { get; set; }
        public int ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string PlantCode { get; set; }
        public string SaleOrg { get; set; }

        public List<UserViewModel> lstuser { get; set; }
    }


    public class ChangePasswordViewModel
    {
        [Key]
        public int USER_ID { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter the Old Password")]
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "Old Password")]
        //[RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter the New Password")]
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "New Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@@#$%^&*])(?=.{8,})", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
    }

    // Update New 
    public class MaintenanceUserViewModel
    {
        List<UserViewModel> Modellist { get; set; }
    }
}
