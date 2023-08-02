using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.ReportTypes;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.ProcessTypes;

namespace SVCW.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProcessTypeController : ControllerBase
    {
        private IProcessType _processTypeService;
        public ProcessTypeController(IProcessType processTypeService)
        {
            this._processTypeService = processTypeService;
        }
        [Route("get-all-process-Type")]
        [HttpGet]
        public async Task<IActionResult> GetAllProcessType()
        {

            ResponseAPI<List<ProcessType>> responseAPI = new ResponseAPI<List<ProcessType>>();
            try
            {
                responseAPI.Data = await this._processTypeService.GetAllProcessType();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-process-type-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetProcessTypeById(string? processTypeId)
        {

            ResponseAPI<List<ProcessType>> responseAPI = new ResponseAPI<List<ProcessType>>();
            try
            {
                responseAPI.Data = await this._processTypeService.GetProcessTypeById(processTypeId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-report-type-by-name")]
        [HttpGet]
        public async Task<IActionResult> SearchByNameProcessType(string? processTypeName)
        {

            ResponseAPI<List<ProcessType>> responseAPI = new ResponseAPI<List<ProcessType>>();
            try
            {
                responseAPI.Data = await this._processTypeService.SearchByNameProcessType(processTypeName);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("insert-report-type-by-id")]
        [HttpPost]
        public async Task<IActionResult> InsertProcessType(ProcessTypeDTO processType)
        {
            ResponseAPI<List<ProcessTypeDTO>> responseAPI = new ResponseAPI<List<ProcessTypeDTO>>();
            try
            {
                responseAPI.Data = await this._processTypeService.InsertProcessType(processType);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-process-type-by-id")]
        [HttpPut]
        public async Task<IActionResult> UpdateProcessType(ProcessTypeDTO upProcessType)
        {
            ResponseAPI<List<ProcessType>> responseAPI = new ResponseAPI<List<ProcessType>>();
            try
            {
                responseAPI.Data = await this._processTypeService.UpdateProcessType(upProcessType);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("delete-process-type-by-id")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProcessType([FromQuery] List<string> processTypeId)
        {
            ResponseAPI<List<ProcessType>> responseAPI = new ResponseAPI<List<ProcessType>>();
            try
            {
                responseAPI.Data = await this._processTypeService.DeleteProcessType(processTypeId);
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
