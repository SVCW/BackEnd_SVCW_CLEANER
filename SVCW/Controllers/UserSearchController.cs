using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Admin_Moderator.Moderator;
using SVCW.DTOs.UserSearchHistory;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSearchController : ControllerBase
    {
        private ISearchContent service;
        public UserSearchController(ISearchContent service)
        {
            this.service = service;
        }

        [Route("recommend-activity")]
        [HttpGet]
        public async Task<IActionResult> getbyId(string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.recommendActivity(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("recommend-fanpage")]
        [HttpGet]
        public async Task<IActionResult> fanpage(string userId)
        {
            ResponseAPI<List<Fanpage>> responseAPI = new ResponseAPI<List<Fanpage>>();
            try
            {
                responseAPI.Data = await this.service.recommendFanpage(userId);
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
        public async Task<IActionResult> create(UserSearchHistoryDTO dto)
        {
            ResponseAPI<List<UserSearch>> responseAPI = new ResponseAPI<List<UserSearch>>();
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
    }
}