using SVCW.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Comments;
using SVCW.Models;
using SVCW.DTOs;

namespace SVCW.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentController : ControllerBase
    {
		private IComment service;
		public CommentController (IComment service)
		{
			this.service = service;
		}

		[Route("comment")]
		[HttpPost]
		public async Task<IActionResult> comment(CommentDTO comment)
		{
			ResponseAPI<Comment> responseAPI = new ResponseAPI<Comment>();
			try
			{
				responseAPI.Data = await this.service.comment(comment);
				return Ok(responseAPI);
			}
			catch (Exception ex)
			{
				responseAPI.Message = ex.Message;
				return BadRequest(responseAPI);
			}
		}

        [Route("reply-comment")]
        [HttpPost]
        public async Task<IActionResult> replyComment(ReplyCommentDTO comment)
        {
            ResponseAPI<Comment> responseAPI = new ResponseAPI<Comment>();
            try
            {
                responseAPI.Data = await this.service.ReplyComment(comment);
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
        [Route("get-comment")]
        [HttpGet]
        public async Task<IActionResult> getComment()
        {
            ResponseAPI<List<Comment>> responseAPI = new ResponseAPI<List<Comment>>();
            try
            {
                responseAPI.Data = await this.service.GetComments();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-comment-user")]
        [HttpGet]
        public async Task<IActionResult> getCommentUser()
        {
            ResponseAPI<List<Comment>> responseAPI = new ResponseAPI<List<Comment>>();
            try
            {
                responseAPI.Data = await this.service.GetCommentUser();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-comment")]
        [HttpPut]
        public async Task<IActionResult> updateComment(UpdateCommentDTO comment)
        {
            ResponseAPI<Comment> responseAPI = new ResponseAPI<Comment>();
            try
            {
                responseAPI.Data = await this.service.UpdateComment(comment);
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
        [Route("delete-comment")]
        [HttpDelete]
        public async Task<IActionResult> deleteComment(string id)
        {
            ResponseAPI<bool> responseAPI = new ResponseAPI<bool>();
            try
            {
                responseAPI.Data = await this.service.DeleteComment(id);
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

