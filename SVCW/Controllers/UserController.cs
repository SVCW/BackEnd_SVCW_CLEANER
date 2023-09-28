using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Common;
using SVCW.DTOs.JWT;
using SVCW.DTOs.Users;
using SVCW.DTOs.Users.Req;
using SVCW.DTOs.Users.Res;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUser service;
        public UserController(IUser service)
        {
            this.service = service;
        }

        [Route("get-statistic-profile")]
        [HttpGet]
        public async Task<IActionResult> getStatistic(string userId)
        {
            ResponseAPI<ProfileDTO> responseAPI = new ResponseAPI<ProfileDTO>();
            try
            {
                responseAPI.Data = await this.service.checkProfile(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-personal-schedule")]
        [HttpGet]
        public async Task<IActionResult> getSchedule(string userId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.personalSchedulev1(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-all-user")]
        [HttpGet]
        public async Task<IActionResult> getAllUser()
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
            try
            {
                responseAPI.Data = await this.service.getAllUser();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-list-user")]
        [HttpPost]
        public async Task<IActionResult> getListUser(List<string> userId)
        {
            ResponseAPI<List<User>> responseAPI = new ResponseAPI<List<User>>();
            try
            {
                responseAPI.Data = await this.service.getListUserByListUserId(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-user-by-id")]
        [HttpGet]
        public async Task<IActionResult> getUserById([FromQuery] GetUserByIdReq req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.getUserById(req);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [Route("get-user-by-idv2")]
        [HttpGet]
        public async Task<IActionResult> getUserByIdv2(string userId)
        {
            ResponseAPI<Profilev2DTO> responseAPI = new ResponseAPI<Profilev2DTO>();
            try
            {
                responseAPI.Data = await this.service.getUserById1(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-history-user")]
        [HttpGet]
        public async Task<IActionResult> getHistoryUser(string userId)
        {
            ResponseAPI<List<FollowJoinAvtivity>> responseAPI = new ResponseAPI<List<FollowJoinAvtivity>>();
            try
            {
                responseAPI.Data = await this.service.historyUserJoin(userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        /// <summary>
        /// Required{email} -- nếu là new user -> userdata
        /// </summary>
        /// <param name="LoginReq"></param>
        /// <returns></returns>
        [Route("validate-login-user")]
        [HttpPost]
        public async Task<IActionResult> validateLoginUser(LoginReq req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                var res = new CommonUserRes();

                var validateRes = await this.service.validateLoginUser(req);
                //var rstCode = res.resultCode;
                Console.WriteLine(validateRes.resultCode + " - " + validateRes.resultMsg);

                //First time login
                if (validateRes.resultCode == SVCWCode.FirstTLogin)
                {
                    var createUserReq = new CreateUserReq();
                    createUserReq.Email = req.Email;

                    // Create new user
                    Console.WriteLine("Do create new User...");
                    var createUserRes = await this.service.createUser(createUserReq);

                    if (createUserRes.resultCode != SVCWCode.SUCCESS)
                    {
                        res.resultCode = SVCWCode.LoginSuccesAndFail;
                        res.resultMsg = "Thông tin login hợp lệ. Nhưng có lỗi khi tạo mới tài khoản!";
                        responseAPI.Data = res;
                        return Ok(responseAPI);
                    }
                    res.resultCode = SVCWCode.SUCCESS;
                    res.resultMsg = "Đăng nhập thành công! Chào mừng thành viên mới!";
                    res.user = createUserRes.user;
                    responseAPI.Data = res;
                    return Ok(responseAPI);
                }
                res = validateRes;
                responseAPI.Data = res;
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// Required{email}
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> createUser(CreateUserReq req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.createUser(req);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("login-username-password")]
        [HttpPost]
        public async Task<IActionResult> lognUser(LoginDTO dto)
        {
            ResponseAPI<User> responseAPI = new ResponseAPI<User>();
            try
            {
                responseAPI.Data = await this.service.Login(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        /// <summary>
        /// này là có hàm băm j đó của Ước
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Route("login-username")]
        [HttpPost]
        public async Task<IActionResult> LoginUserName(LoginDTO dto)
        {
            ResponseAPI<JwtTokenDto> responseAPI = new ResponseAPI<JwtTokenDto>();
            try
            {
                responseAPI.Data = await this.service.LoginUserName(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        /// <summary>
        /// Required{userId, >= 1 thông tin update}
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("update-user")]
        [HttpPut]
        public async Task<IActionResult> updateUser(UpdateUserReq req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.updateUser(req);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("change-password")]
        [HttpPut]
        public async Task<IActionResult> changePassword(ChangePwReq req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.changeUserPassword(req);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("ban-user")]
        [HttpPut]
        public async Task<IActionResult> banUser(BanDTO req)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.banUser(req);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("un-ban-user")]
        [HttpPut]
        public async Task<IActionResult> unBanUser(string userId)
        {
            ResponseAPI<CommonUserRes> responseAPI = new ResponseAPI<CommonUserRes>();
            try
            {
                responseAPI.Data = await this.service.unBanUser(userId);
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