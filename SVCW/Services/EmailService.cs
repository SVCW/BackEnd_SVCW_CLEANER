using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SVCW.DTOs.Email;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
	public class EmailService: IEmail
	{
        protected readonly SVCWContext context;
        protected readonly IConfiguration _config;

        public EmailService(SVCWContext context, IConfiguration config)
		{
            this.context = context;
            this._config = config;
        }

        public async Task<SendEmailResDTO> sendEmail(SendEmailReqDTO sendEmaiReqDTO)
        {
            // add this config to appsetting.json

            //"EmailConfig": {
            //  "EmailHost": "smtp.ethereal.email",
            //  "SVCWEmail": "phatst1232@gmail.com",
            //  "SVCWEmailPw": "yftw-rwup-gheu11"
            //},
            //
            // go to https://myaccount.google.com/apppasswords to gen a [ mail | window pc ] pw to fill
            // in "SVCWEmailPw" key value

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailConfig")["SVCWEmail"]));
            email.To.Add(MailboxAddress.Parse(sendEmaiReqDTO.sendTo));
            email.Subject = sendEmaiReqDTO.subject;
            email.Body = new TextPart(TextFormat.Html) { Text = sendEmaiReqDTO.body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailConfig")["EmailHost"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailConfig")["SVCWEmail"],
                _config.GetSection("EmailConfig")["SVCWEmailPw"]);
            smtp.Send(email);
            smtp.Disconnect(true);

            var result = new SendEmailResDTO();
            result.isSuccess = true;
            result.errorMessage = "SUCCESS";
            return result;
        }
	}
}

