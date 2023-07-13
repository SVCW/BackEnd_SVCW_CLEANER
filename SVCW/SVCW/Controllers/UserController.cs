using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Common;
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
    }
}