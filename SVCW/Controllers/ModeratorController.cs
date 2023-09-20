using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Admin_Moderator.Moderator;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private IModerator service;
        public ModeratorController(IModerator service)
        {
            this.service = service;
        }

        [Route("get-all")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
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

        [Route("get-inActive")]
        [HttpGet]
        public async Task<IActionResult> getInactive()
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
            try
            {
                responseAPI.Data = await this.service.getAllInActive();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-byId")]
        [HttpGet]
        public async Task<IActionResult> getbyId(string id)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
            try
            {
                responseAPI.Data = await this.service.getModerator(id);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> login(LoginModerator dto)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
            try
            {
                responseAPI.Data = await this.service.login(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        /// <summary>
        /// require: nhập hết nha
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> dreate(CreateModerator dto)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
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
        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> update(updateModerator dto)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
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
        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> delete(string id)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
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
    }
}
