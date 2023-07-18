using System;
namespace SVCW.DTOs.Users.Req
{
	public class ChangePwReq
	{
		public string UserId { get; set; }
		public string oldPassword { get; set; }
		public string newPassword { get; set; }
	}
}

