using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Activities;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.DTOs.Statistical;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private IStatistical service;
        public StatisticalController(IStatistical service)
        {
            this.service = service;
        }

        [Route("statistical")]
        [HttpPost]
        public async Task<IActionResult> get(string userId, DateTime start, DateTime end)
        {
            ResponseAPI<StatisticalUserDonateDTO> responseAPI = new ResponseAPI<StatisticalUserDonateDTO>();
            try
            {
                responseAPI.Data = await this.service.get(userId,start,end);
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
