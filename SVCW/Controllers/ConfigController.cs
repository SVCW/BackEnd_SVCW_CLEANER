using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Config;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private IConfig Service;
        public ConfigController(IConfig Service)
        {
            this.Service = Service;
        }

        [Route("get-AdminConfig")]
        [HttpGet]
        public async Task<IActionResult> getAdminConfig()
        {

            ResponseAPI<adminConfig> responseAPI = new ResponseAPI<adminConfig>();
            try
            {
                responseAPI.Data = this.Service.GetAdminConfig();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-userCreateActivityConfig")]
        [HttpPost]
        public async Task<IActionResult> getUserConfig(configDTO dto)
        {

            ResponseAPI<userCreateActivityConfig> responseAPI = new ResponseAPI<userCreateActivityConfig>();
            try
            {
                responseAPI.Data = this.Service.getConfig(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-process-config")]
        [HttpPost]
        public async Task<IActionResult> getConfigForUser(configDTO dto)
        {
            ResponseAPI<ProcessConfigDTO> responseAPI = new ResponseAPI<ProcessConfigDTO>();
            try
            {
                responseAPI.Data = this.Service.getConfigForUser(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-adminConfig")]
        [HttpPut]
        public async Task<IActionResult> updateAdminConfig(adminConfig update)
        {

            ResponseAPI<adminConfig> responseAPI = new ResponseAPI<adminConfig>();
            try
            {
                responseAPI.Data = this.Service.updateAdminConfig(update);
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
