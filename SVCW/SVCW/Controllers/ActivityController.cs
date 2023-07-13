using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Votes;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.Activities;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private IActivity service;
        public ActivityController(IActivity service)
        {
            this.service = service;
        }

        [Route("Insert-Activity")]
        [HttpPost]
        public async Task<IActionResult> Insert(ActivityCreateDTO dto)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.createActivity(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("join-Activity")]
        [HttpPost]
        public async Task<IActionResult> join(string activityId, string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.joinActivity(activityId,userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("follow-Activity")]
        [HttpPost]
        public async Task<IActionResult> follow(string activityId, string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.followActivity(activityId,userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("unfollow-activity")]
        [HttpPut]
        public async Task<IActionResult> unfollow(string activityId, string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.unFollowActivity(activityId, userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("disJoin-activity")]
        [HttpPut]
        public async Task<IActionResult> disjoin(string activityId, string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.disJoinActivity(activityId, userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("update-activity")]
        [HttpPut]
        public async Task<IActionResult> update(ActivityUpdateDTO dto)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.updateActivity(dto);
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
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete-activity")]
        [HttpDelete]
        public async Task<IActionResult> delete(string id)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.delete(id);
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
        /// <returns></returns>
        [Route("get-activity")]
        [HttpGet]
        public async Task<IActionResult> getall(int pageSize, int PageLoad)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = this.service.getAll(pageSize, PageLoad);
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
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-activity-id")]
        [HttpGet]
        public async Task<IActionResult> getid(string id)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.getById(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-activity-title")]
        [HttpGet]
        public async Task<IActionResult> getTitle(string title)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.getByTitle(title);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-activity-user")]
        [HttpGet]
        public async Task<IActionResult> getUser()
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.getForUser();
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
