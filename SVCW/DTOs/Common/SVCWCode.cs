using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Common
{
	public enum SVCWCode
	{
        [Display(Name = "Unknown Error")]
        Unknown = -1,

        [Display(Name = "Unknown Error")]
        BANNED = -2,

        [Display(Name = "Success")]
        SUCCESS = 0,

        [Display(Name = "Invalid Input")]
        InvalidInput = 1,

        [Display(Name = "Unauthorized Access")]
        UnauthorizedAccess = 2,

        //---User Error Codes[100,199]---
        [Display(Name = "Email Existed")]
        EmailExisted = 100,

        [Display(Name = "User First Time Login")]
        FirstTLogin = 101,

        [Display(Name = "User Not Exist")]
        UserNotExist = 102,

        [Display(Name = "Login success but can't create new user")]
        LoginSuccesAndFail = 103,

        [Display(Name = "Invalid Email")]
        InvalidEmail = 104,

        [Display(Name = "Invalid User Password")]
        InvalidPassword = 105,

        [Display(Name = "New Password Invalid")]
        InvalidNewPassword = 106,
    }
}

