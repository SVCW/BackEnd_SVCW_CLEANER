using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Roles;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRole service;
        public RoleController(IRole service)
        {
            this.service = service;
        }

        /// <summary>
        /// admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("Insert-Role")]
        [HttpPost]
        public async Task<IActionResult> Insert(RoleCreateDTO dto)
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
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

        /// <summary>
        /// admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("update-role")]
        [HttpPut]
        public async Task<IActionResult> update(RoleUpdateDTO dto)
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
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
        /// admin
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("delete-role")]
        [HttpDelete]
        public async Task<IActionResult> delete(string dto)
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
            try
            {
                responseAPI.Data = await this.service.delete(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin
        /// </summary>
        /// <returns></returns>
        [Route("get-role")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
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
        /// admin
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("get-rolename")]
        [HttpGet]
        public async Task<IActionResult> getName(string name)
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
            try
            {
                responseAPI.Data = await this.service.findByName(name);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-roleid")]
        [HttpGet]
        public async Task<IActionResult> getId(string id)
        {
            ResponseAPI<List<Role>> responseAPI = new ResponseAPI<List<Role>>();
            try
            {
                responseAPI.Data = await this.service.findById(id);
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
