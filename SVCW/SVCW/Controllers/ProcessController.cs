using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Fanpage;
using SVCW.DTOs.Process;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private IProcess service;
        public ProcessController(IProcess service)
        {
            this.service = service;
        }

        [Route("getall-process")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.GetAllProcess();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-process-id")]
        [HttpGet]
        public async Task<IActionResult> getId(string processId)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.GetProcessById(processId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-process-title")]
        [HttpGet]
        public async Task<IActionResult> SearchByTitleProcess(string title)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.SearchByTitleProcess(title);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-process-activity")]
        [HttpGet]
        public async Task<IActionResult> GetProcessForActivity(string activityId)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.GetProcessForActivity(activityId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("Insert-process")]
        [HttpPost]
        public async Task<IActionResult> Insert(CreateProcessDTO dto)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.InsertProcess(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("Insert-process-list")]
        [HttpPost]
        public async Task<IActionResult> InsertList(List<CreateProcessDTO> dto)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.InsertProcessList(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-process")]
        [HttpPut]
        public async Task<IActionResult> update(updateProcessDTO dto)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.UpdateProcess(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("delete-process")]
        [HttpDelete]
        public async Task<IActionResult> delete(string id)
        {
            ResponseAPI<List<Process>> responseAPI = new ResponseAPI<List<Process>>();
            try
            {
                responseAPI.Data = await this.service.DeleteProcess(id);
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
