using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Reports;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReport service;

        public ReportController(IReport service)
        {
            this.service = service;
        }

        [Route("get-all-report")]
        [HttpGet]
        public async Task<IActionResult> getAllReport()
        {
            ResponseAPI<List<Report>> responseAPI = new ResponseAPI<List<Report>>();
            try
            {
                responseAPI.Data = await this.service.getAllReport();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("new-report")]
        [HttpPost]
        public async Task<IActionResult> newReport(ReportDTO rp)
        {
            ResponseAPI<Report> responseAPI = new ResponseAPI<Report>();
            try
            {
                responseAPI.Data = await this.service.newReport(rp);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-report")]
        [HttpPut]
        public async Task<IActionResult> updateReport(ReportDTO rp)
        {
            ResponseAPI<Report> responseAPI = new ResponseAPI<Report>();
            try
            {
                responseAPI.Data = await this.service.updateReport(rp);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("dalete-report")]
        [HttpDelete]
        public async Task<IActionResult> deleteReport(string rpId)
        {
            ResponseAPI<bool> responseAPI = new ResponseAPI<bool>();
            try
            {
                responseAPI.Data = await this.service.deleteReport(rpId);
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
