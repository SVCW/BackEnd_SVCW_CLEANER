using Microsoft.AspNetCore.Mvc;
using SVCW.Interfaces;
using SVCW.DTOs.Likes;
using SVCW.DTOs;
using SVCW.Models;

namespace SVCW.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class LikeController : ControllerBase
	{
		private ILike service;
		public LikeController(ILike service)
		{
			this.service = service;
		}

		[Route("simple-like")]
		[HttpPost]
		public async Task<bool> simpleLike(LikeDTO like)
		{
            ResponseAPI<Like> responseAPI = new ResponseAPI<Like>();
            try
            {
                responseAPI.Data = await this.service.SimpleLike(like);
                return true;
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return true;
            }
        }

		[Route("simple-unlike")]
		[HttpDelete]
		public async Task<bool> simpleUnLike(LikeDTO like)
		{
            ResponseAPI<Like> responseAPI = new ResponseAPI<Like>();
            try
            {
                responseAPI.Data = await this.service.SimpleUnLike(like);
                return true;
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return true;
            }
        }
	}
}

