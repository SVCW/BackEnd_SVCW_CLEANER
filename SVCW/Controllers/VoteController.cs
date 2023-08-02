using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Roles;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.Votes;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private IVote service;
        public VoteController(IVote service)
        {
            this.service = service;
        }

        [Route("Insert-Vote")]
        [HttpPost]
        public async Task<IActionResult> Insert(VoteDTO dto)
        {
            ResponseAPI<List<Vote>> responseAPI = new ResponseAPI<List<Vote>>();
            try
            {
                responseAPI.Data = await this.service.createVote(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-vote")]
        [HttpPut]
        public async Task<IActionResult> update(VoteDTO dto)
        {
            ResponseAPI<List<Vote>> responseAPI = new ResponseAPI<List<Vote>>();
            try
            {
                responseAPI.Data = await this.service.updateVote(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("delete-vote")]
        [HttpDelete]
        public async Task<IActionResult> delete(VoteDTO dto)
        {
            ResponseAPI<List<Vote>> responseAPI = new ResponseAPI<List<Vote>>();
            try
            {
                responseAPI.Data = await this.service.deleteVote(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-vote")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<Vote>> responseAPI = new ResponseAPI<List<Vote>>();
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
    }
}
