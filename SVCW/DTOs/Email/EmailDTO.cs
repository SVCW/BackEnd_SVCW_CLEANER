using SVCW.DTOs.Common;

namespace SVCW.DTOs.Email
{
	public class SendEmailReqDTO
	{
        public string sendTo { get; set; }
        public string? subject { get; set; }
        public string body { get; set; }
    }

    public class SendEmailWithTamplateReqDTO
    {
        public string sendTo { get; set; }
        public EmailTamplate tamplateId { get; set; }
        public string fullname { get; set; }
    }

    public class SendEmailResDTO
    {
        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public bool isSuccess { get; set; }
        public string? tamplateId { get; set; }
        public string? errorMessage { get; set; }
    }
}

        