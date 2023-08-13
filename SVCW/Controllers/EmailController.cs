using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Email;
using SVCW.Interfaces;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private IEmail service;
        public EmailController(IEmail service)
        {
            this.service = service;
        }

        /// <summary>
        /// example: {"sendTo": "","subject": "Great party yesterday!","body": "<i>I drank waaaayyy too much! :D</i>"}
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// example: {"sendTo": "Quyentvse151245@fpt.edu.vn","tamplateId": 1,"fullname": "Văn Quyền"} <- Dear ...
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("send-email-tamplate")]
        [HttpPost]
        public async Task<IActionResult> sendEmailTamplate(SendEmailWithTamplateReqDTO dto)
        {
            ResponseAPI<SendEmailResDTO> responseAPI = new ResponseAPI<SendEmailResDTO>();
            try
            {
                responseAPI.Data = await this.service.sendEmailWithTamplate(dto);
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
