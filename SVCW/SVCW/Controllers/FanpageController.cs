using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Activities;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.DTOs.Fanpage;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FanpageController : ControllerBase
    {
        private IFanpage service;
        public FanpageController(IFanpage service)
        {
            this.service = service;
        }


        [Route("Insert-fanpage")]
        [HttpPost]
        public async Task<IActionResult> Insert(FanpageCreateDTO dto)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
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

        [Route("update-fanpage")]
        [HttpPut]
        public async Task<IActionResult> update(FanpageUpdateDTO dto)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
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

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete-fanpage")]
        [HttpDelete]
        public async Task<IActionResult> delete(string id)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
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
        [Route("getall-fanpage")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
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

        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <returns></returns>
        [Route("get-fanpage-moderate")]
        [HttpGet]
        public async Task<IActionResult> getModerate()
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.getModerate();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-fanpage-view")]
        [HttpGet]
        public async Task<IActionResult> getuser()
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.getFotUser();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-fanpage-id")]
        [HttpGet]
        public async Task<IActionResult> getid(string id)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.getByID(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-fanpage-name")]
        [HttpGet]
        public async Task<IActionResult> getName(string name)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.getByName(name);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("follow-fanpage")]
        [HttpPost]
        public async Task<IActionResult> follow(string userId, string fanpageId)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.follow(userId,fanpageId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("unfollow-fanpage")]
        [HttpPut]
        public async Task<IActionResult> unfollow(string userId, string fanpageId)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.unfollow(userId, fanpageId);
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
        [Route("moderate-fanpage")]
        [HttpPut]
        public async Task<IActionResult> moderate(string id)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.moderate(id);
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
