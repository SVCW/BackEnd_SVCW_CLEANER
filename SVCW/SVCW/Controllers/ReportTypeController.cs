using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Achivements;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.ReportTypes;

namespace SVCW.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ReportTypeController : ControllerBase
    {
        private IReportType _reportTypeService;
        public ReportTypeController(IReportType reportTypeService)
        {
            this._reportTypeService = reportTypeService;
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <returns></returns>
        [Route("get-all-Report-Type")]
        [HttpGet]
        public async Task<IActionResult> GetAllAchivement()
        {

            ResponseAPI<List<ReportType>> responseAPI = new ResponseAPI<List<ReportType>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.GetAllReportTypes();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="reportTypeId"></param>
        /// <returns></returns>
        [Route("get-report-type-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetAchivementById(string? reportTypeId)
        {

            ResponseAPI<List<ReportType>> responseAPI = new ResponseAPI<List<ReportType>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.GetReportTypeById(reportTypeId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="reportTypeName"></param>
        /// <returns></returns>
        [Route("get-report-type-by-name")]
        [HttpGet]
        public async Task<IActionResult> SearchByNameReportType(string? reportTypeName)
        {

            ResponseAPI<List<ReportType>> responseAPI = new ResponseAPI<List<ReportType>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.SearchByNameReportType(reportTypeName);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        [Route("insert-report-type-by-id")]
        [HttpPost]
        public async Task<IActionResult> InsertReportType(ReportTypeDTO reportType)
        {
            ResponseAPI<List<ReportTypeDTO>> responseAPI = new ResponseAPI<List<ReportTypeDTO>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.InsertReportType(reportType);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="upReportType"></param>
        /// <returns></returns>
        [Route("update-report-Type-by-id")]
        [HttpPut]
        public async Task<IActionResult> UpdateReportType(ReportTypeDTO upReportType)
        {
            ResponseAPI<List<ReportType>> responseAPI = new ResponseAPI<List<ReportType>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.UpdateReportType(upReportType);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="achivementId"></param>
        /// <returns></returns>
        [Route("delete-report-type-by-id")]
        [HttpDelete]
        public async Task<IActionResult> DeleteReportType([FromQuery] List<string> achivementId)
        {
            ResponseAPI<List<ReportType>> responseAPI = new ResponseAPI<List<ReportType>>();
            try
            {
                responseAPI.Data = await this._reportTypeService.DeleteReportType(achivementId);
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
