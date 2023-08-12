using System;
namespace SVCW.DTOs.Email
{
    //public class 

	public class SendEmailReqDTO
	{
        public string? sendTo { get; set; }
        public string? subject { get; set; }
        public string? body { get; set; }
    }

    public class SendEmailResDTO
    {
        public bool isSuccess { get; set; }
        public string? errorMessage { get; set; }
    }
}

