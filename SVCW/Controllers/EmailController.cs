using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Email;
using SVCW.Interfaces;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController: ControllerBase
	{
		private IEmail service;
		public EmailController(IEmail service)
		{
			this.service = service;
		}

        [Route("send-email")]
        [HttpPost]
        public async Task<IActionResult> sendEmail(SendEmailReqDTO dto)
		{
            ResponseAPI<SendEmailResDTO> responseAPI = new ResponseAPI<SendEmailResDTO>();
            try
            {
                responseAPI.Data = await this.service.sendEmail(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
	}
}

