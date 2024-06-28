using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Models.SystemAdmin
{
    public class MasterUserModel
    {
        [Key]
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public int DEFAULT_ROLE_ID { get; set; }
        public string USER_DOMAIN { get; set; }
        public bool IS_LOCKED_OUT { get; set; }
        public bool MUST_CHANGE_PASSWORD { get; set; }
        public string SALE_ORG { get; set; }
        public string PLANT_CODE { get; set; }
        public string FIRST_NAME_TH { get; set; }
        public string LAST_NAME_TH { get; set; }
        public string FIRST_NAME_EN { get; set; }
        public string LAST_NAME_EN { get; set; }
        public string TELEPHONE { get; set; }
        public string MOBILE_NO { get; set; }
        [EmailAddress]
        public string EMAIL { get; set; }
        public string PASSWORD_HINT { get; set; }
        public string COMMENT { get; set; }
        public DateTime LAST_PASSWORD_CHANGED_DATE { get; set; }
        public DateTime? LAST_LOGIN_DATE { get; set; }
        public DateTime? LAST_LOGOUT_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public bool IS_RECEIVE_EMAIL { get; set; }
        public bool IS_FLAG_DELETE { get; set; }

        //  public virtual MasterRoleModel Role { get; set; }
        //   public virtual MasterRoleModel Role { get; set; }
    }
}
