using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Activities;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.ActivityResults;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityResultController : ControllerBase
    {
        private IActivityResult service;
        public ActivityResultController(IActivityResult service)
        {
            this.service = service;
        }

        [Route("Insert-activityresult")]
        [HttpPost]
        public async Task<IActionResult> Insert(ActivityResultCreateDTO dto)
        {
            ResponseAPI<List<ActivityResult>> responseAPI = new ResponseAPI<List<ActivityResult>>();
            try
            {
                responseAPI.Data = await this.service.create(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-activityresult")]
        [HttpPut]
        public async Task<IActionResult> update(ActivityResultUpdateDTO dto)
        {
            ResponseAPI<List<ActivityResult>> responseAPI = new ResponseAPI<List<ActivityResult>>();
            try
            {
                responseAPI.Data = await this.service.update(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }


        [Route("getall-activityresult")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<ActivityResult>> responseAPI = new ResponseAPI<List<ActivityResult>>();
            try
            {
                responseAPI.Data = await this.service.getAll();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-activityresult-activity")]
        [HttpGet]
        public async Task<IActionResult> getActivity(string activityId)
        {
            ResponseAPI<List<ActivityResult>> responseAPI = new ResponseAPI<List<ActivityResult>>();
            try
            {
                responseAPI.Data = await this.service.getForActivity(activityId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-activityresult-activity-v2")]
        [HttpGet]
        public async Task<IActionResult> getActivityV2(string activityId)
        {
            ResponseAPI<List<ActivityResult>> responseAPI = new ResponseAPI<List<ActivityResult>>();
            try
            {
                responseAPI.Data = await this.service.getForActivityv2(activityId);
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
