using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailAPIRepository _emailAPIRepository;
        private readonly ISendEmailAPIRepository _sendEmailAPIRepository;

        private readonly string _username;
        private readonly string _userEmail;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private string _token;

        public EmailService(IHttpContextAccessor httpContextAccessor, IEmailAPIRepository emailAPIRepository, ISendEmailAPIRepository sendEmailAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _emailAPIRepository = emailAPIRepository;
            _sendEmailAPIRepository = sendEmailAPIRepository;

            var userSession = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSession != null)
            {
                _username = userSession.UserName;
                _userEmail = userSession.Email;
                _saleOrg = userSession.SaleOrg;
                _factoryCode = userSession.FactoryCode;
                _token = userSession.Token;

            }
        }

        public void SendNotifyWhenCreatedBoard(string boardCode, string boardDesc)
        {
            try
            {
                var htmlContent = ReadTemplate("CreatedBoard_Template.html");
                htmlContent = htmlContent.Replace("{BoardCode}", boardCode);
                htmlContent = htmlContent.Replace("{BoardDescription}", boardDesc);
                htmlContent = htmlContent.Replace("{CreatedBy}", _username);
                //List<string> toEmail = JsonConvert.DeserializeObject<List<string>>(_sendEmailAPIRepository.GetEmailForSendNotifyByFactoryCode(_factoryCode, _token));
                List<string> toEmail = new List<string>()
                {
                    "Admin@scg.com","AdminDINPACK@scg.com", "Admin@scg.com", "AdminDINPACK@scg.com", "AdminDYNA@scg.com", "AdminOrient@scg.com", "anirudam@scg.com", "banphokh@scg.com", "bunphotj@scg.com", "chaiyakl@scg.com", "chakaphu@scg.com",
                    "chananch@scg.com", "chayukli@scg.com", "chintapi@scg.com", "chonniky@scg.com", "dowroonc@scg.com", "ekachaih@scg.com", "hataip@scg.com", "hataiths@scg.com", "jakkriti@scg.com", "kanchsuk@scg.com", "kannisar@scg.com",
                    "keeratpa@scg.com", "khanitts@scg.com", "kopchaia@scg.com", "manitasi@scg.com", "mirasngu@scg.com", "monyaphn@scg.com", "nanthith@scg.com", "napaposo@scg.com", "naparaik@scg.com", "nirapatn@scg.com", "ornanonm@scg.com", "orranucp@scg.com",
                    "papanact@scg.com", "Patcnuan@scg.com", "pattamna@scg.com", "peeraprs@scg.com", "peeravin@scg.com", "pimjaith@scg.com", "PPC@scg.com", "pubett@scg.com", "sakonl@scg.com", "sermkiap@scg.com", "sombonam@scg.com", "somlukt@scg.com",
                    "somyoty@scg.com", "sonchaik@scg.com", "SUPACHAI@scg.com", "supachaw@scg.com", "supaporb@scg.com", "suphaway@scg.com", "suppanun@scg.com", "sutharan@scg.com", "taweesup@scg.com", "thitirpr@scg.com", "tulkanyj@scg.com", "tunyaluw@scg.com",
                    "TWN@scg.com", "wannayur@scg.com", "wanwisaa@scg.com", "waraposr@scg.com", "watchako@scg.com", "weerasa@scg.com", "wicheins@scg.com", "wimonm@scg.com", "Wipolk@scg.com", "yupavads@scg.com", "yuthachk@scg.com", "yuwaratp@scg.com"
                };
                if (toEmail is not null && toEmail.Count() > 0 && !string.IsNullOrEmpty(_userEmail))
                {
                    EmailRequest payload = new EmailRequest()
                    {
                        Subject = "New board was created",
                        Content = htmlContent,
                        From = new List<string>() { _userEmail },
                        To = toEmail,
                    };
                    _emailAPIRepository.Send(_factoryCode, JsonConvert.SerializeObject(payload), _token);
                }
            }
            catch (Exception)
            {
            }
        }
        private string ReadTemplate(string filename)
        {
            var path = $"Templates/Email/{filename}";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                path = System.IO.Directory.GetCurrentDirectory() + $"\\Templates\\Email\\{filename}";
            }
            using StreamReader reader = new StreamReader(path);
            return reader.ReadToEnd();
        }


    }
}
