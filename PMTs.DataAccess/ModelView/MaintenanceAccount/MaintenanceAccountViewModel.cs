using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.MaintenanceAccount
{
    public class MaintenanceAccountViewModel
    {
        //  public dynamic AccountDataList { get; set; }
        public IEnumerable<AccountViewModel> AccountDataList { get; set; }

        public IEnumerable<MasterRoleList> RoleList { get; set; }

        public dynamic AccountData { get; set; }

        public AccountViewModel AccountViewModel { get; set; }
        public string RoleArrayList { get; set; }

        public List<SelectListItem> Lst_SaleOrg { get; set; }
        public List<SelectListItem> Lst_Plant { get; set; }
    }

    public class AccountViewModel
    {
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string FactoryCode { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(32, ErrorMessage = "Must be between 8 and 32 characters", MinimumLength = 8)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!*@#$%^&+=]).{8,32}$", ErrorMessage = "Must be a valid password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(32, ErrorMessage = "Must be between 8 and 32 characters", MinimumLength = 8)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!*@#$%^&+=]).{8,32}$", ErrorMessage = "Must be a valid password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        public int? DefaultRoleId { get; set; }
        public int? NumberOfLogins { get; set; }
        public string UserDomain { get; set; }
        public bool IsLockedOut { get; set; }
        public bool MustChangePassword { get; set; }
        public string FirstNameTh { get; set; }
        public string LastNameTh { get; set; }
        public string FirstNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string Telephone { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PasswordHint { get; set; }
        public string Comment { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsReceiveMail { get; set; }
        public bool IsFlagDelete { get; set; }

        public List<MasterRoleList> RoleList { get; set; }
        public string RoleJsonList { get; set; }
        public string RoleName { get; set; }

        public bool IsChangePassword { get; set; }

        //Tassanai update 06/02/2020

        public string PictureUser { get; set; }
        public string AppName { get; set; }

    }

    public class MasterRoleList
    {

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }

    }

    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

}
