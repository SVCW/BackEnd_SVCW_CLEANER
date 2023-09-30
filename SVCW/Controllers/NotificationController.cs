using SVCW.Interfaces;
using SVCW.DTOs.Notifications;
using SVCW.Models;
using SVCW.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SVCW.Controllers
{
    /// <summary>
    /// hjhj
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotification service;

        public NotificationController(INotification service)
        {
            this.service = service;
        }

        [Route("get-user-notis")]
        [HttpGet]
        public async Task<IActionResult> getUserNotis(string userId)
        {
            ResponseAPI<List<Notification>> responseAPI = new ResponseAPI<List<Notification>>();
            try
            {
                responseAPI.Data = await this.service.GetNotifications(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("new-noti")]
        [HttpPost]
        public async Task<IActionResult> newNoti(NotificationDTO newNoti)
        {
            ResponseAPI<Notification> responseAPI = new ResponseAPI<Notification>();
            try
            {
                responseAPI.Data = await this.service.newNoti(newNoti);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("mark-as-read")]
        [HttpPut]
        public async Task<IActionResult> markAsRead(string notiId)
        {
            ResponseAPI<bool> responseAPI = new ResponseAPI<bool>();
            try
            {
                responseAPI.Data = await this.service.markAsRead(notiId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("mark-as-readAll")]
        [HttpPut]
        public async Task<IActionResult> markAsReadAll(string userId)
        {
            ResponseAPI<List<Notification>> responseAPI = new ResponseAPI<List<Notification>>();
            try
            {
                responseAPI.Data = await this.service.markAsReadAll(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        /// <summary>
        /// key nao update moi truyen nha
        /// </summary>
        /// <returns></returns>
        [Route("update-noti")]
        [HttpPut]
        public async Task<IActionResult> updateNoti(string notiId, NotificationDTO newNoti)
        {
            ResponseAPI<Notification> responseAPI = new ResponseAPI<Notification>();
            try
            {
                responseAPI.Data = await this.service.UpdateNoti(notiId, newNoti);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("delete-noti")]
        [HttpDelete]
        public async Task<IActionResult> deleteNoti(string notiId)
        {
            ResponseAPI<bool> responseAPI = new ResponseAPI<bool>();
            try
            {
                responseAPI.Data = await this.service.DeleteNoti(notiId);
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

