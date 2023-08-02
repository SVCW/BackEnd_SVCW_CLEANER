using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Activities;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.DTOs.Admin_Moderator.Admin;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdmin service;
        public AdminController(IAdmin service)
        {
            this.service = service;
        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Insert(LoginAdmin dto)
        {
            ResponseAPI<Admin> responseAPI = new ResponseAPI<Admin>();
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
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> create(string username, string newpassword)
        {
            ResponseAPI<Admin> responseAPI = new ResponseAPI<Admin>();
            try
            {
                responseAPI.Data = await this.service.create(username,newpassword);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("updateAdmin")]
        [HttpPut]
        public async Task<IActionResult> updateAdmin(string username, string newpassword)
        {
            ResponseAPI<Admin> responseAPI = new ResponseAPI<Admin>();
            try
            {
                responseAPI.Data = await this.service.update(username, newpassword);
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
