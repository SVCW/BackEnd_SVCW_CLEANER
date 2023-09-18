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
        [Route("statistical-activity")]
        [HttpPost]
        public async Task<IActionResult> getActivity(DateTime start, DateTime end)
        {
            ResponseAPI<StatisticalActivityDTO> responseAPI = new ResponseAPI<StatisticalActivityDTO>();
            try
            {
                responseAPI.Data = await this.service.getActivityBytime( start, end);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("statistical-donate")]
        [HttpPost]
        public async Task<IActionResult> getDonate(DateTime start, DateTime end)
        {
            ResponseAPI<StatisticalDonateDTO> responseAPI = new ResponseAPI<StatisticalDonateDTO>();
            try
            {
                responseAPI.Data = await this.service.getDonateByTime(start, end);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("statistical-admin")]
        [HttpPost]
        public async Task<IActionResult> getAdmin(DateTime start, DateTime end)
        {
            ResponseAPI<StatisticalDonateDTO> responseAPI = new ResponseAPI<StatisticalDonateDTO>();
            try
            {
                responseAPI.Data = await this.service.getByTime(start, end);
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
