using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Achivements;
using SVCW.Interfaces;
using SVCW.Models;
using IronBarCode;

namespace SVCW.Controllers
{
    /// <summary>
    /// hjhj
    /// </summary>
    [Route("/api/[controller]")]
    [ApiController]
    public class AchivementController : ControllerBase
    {
        private IAchivement _achivementService;
        public AchivementController(IAchivement achivementService)
        {
            this._achivementService = achivementService;
        }
        /// <summary>
        /// admin, moderator
        /// </summary>
        /// <returns></returns>
        [Route("get-all-achivement")]
        [HttpGet]
        public async Task<IActionResult> GetAllAchivement()
        {

            ResponseAPI<List<Achivement>> responseAPI = new ResponseAPI<List<Achivement>>();
            try
            {
                responseAPI.Data = await this._achivementService.GetAllAchivements();
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
        /// <param name="achivementId"></param>
        /// <returns></returns>
        [Route("get-achivement-by-id")]
        [HttpGet]
        public async Task<IActionResult> GetAchivementById(string? achivementId)
        {

            ResponseAPI<List<Achivement>> responseAPI = new ResponseAPI<List<Achivement>>();
            try
            {
                responseAPI.Data = await this._achivementService.GetAchivementById(achivementId);
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
        /// <param name="achivementId"></param>
        /// <returns></returns>
        [Route("insert-achivement")]
        [HttpPost]
        public async Task<IActionResult> InsertFood(AchivementDTO achivementId)
        {
            ResponseAPI<List<AchivementDTO>> responseAPI = new ResponseAPI<List<AchivementDTO>>();
            try
            {
                responseAPI.Data = await this._achivementService.InsertAchivement(achivementId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// admin, moderate
        /// </summary>
        /// <param name="achivementId"></param>
        /// <returns></returns>
        [Route("insert-achivement-user")]
        [HttpPost]
        public async Task<IActionResult> achivementUser(string userId, string achivementId)
        {
            ResponseAPI<List<AchivementDTO>> responseAPI = new ResponseAPI<List<AchivementDTO>>();
            try
            {
                responseAPI.Data = await this._achivementService.UserAchivement(userId,achivementId);
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
        /// <param name="upAchivement"></param>
        /// <returns></returns>
        [Route("update-achivement-by-id")]
        [HttpPut]
        public async Task<IActionResult> UpdateAchivement(AchivementDTO upAchivement)
        {
            ResponseAPI<List<Achivement>> responseAPI = new ResponseAPI<List<Achivement>>();
            try
            {
                responseAPI.Data = await this._achivementService.UpdateAchivement(upAchivement);
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
        /// <param name="achivementId"></param>
        /// <returns></returns>
        [Route("delete-achivement-by-id")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFood(string achivementId)
        {
            ResponseAPI<List<Achivement>> responseAPI = new ResponseAPI<List<Achivement>>();
            try
            {
                responseAPI.Data = await this._achivementService.DeleteAchivement(achivementId);
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
