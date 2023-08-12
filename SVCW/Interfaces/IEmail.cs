using SVCW.DTOs.Email;

namespace SVCW.Interfaces
{
	public interface IEmail
	{
        Task<SendEmailResDTO> sendEmail(SendEmailReqDTO dto);
	}
}

