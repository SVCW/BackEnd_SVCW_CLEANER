

using Microsoft.EntityFrameworkCore;

using SVCW.DTOs.Common;
using SVCW.DTOs.Users.Req;
using SVCW.DTOs.Users.Res;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class UserService : IUser
    {
        private readonly SVCWContext _context;

        public UserService(SVCWContext context)
        {
            _context = context;
        }

        public async Task<CommonUserRes> createUser(CreateUserReq req)
        {
            try
            {
                var res = new CommonUserRes();
                var email = req.Email.ToLower();
                //validate create user data
                if (!isValidCreateData(req,res)) return res;

                // check if email existed
                var check = await this._context.User.Where(x => x.Email.Equals(email)).FirstOrDefaultAsync();
                if (check != null)
                {
                    res.resultCode = SVCWCode.EmailExisted;
                    res.resultMsg = "Email đã được đăng ký!";
                    return res;
                }

                var user = new User();

                user.UserId = "USR" + Guid.NewGuid().ToString().Substring(0, 7);
                //maping
                user.Email = req.Email;
                user.FullName = req.FullName ?? "none";
                user.Username = req.Email.Split("@")[0];
                user.Password = req.Password ?? "PWD" + Guid.NewGuid().ToString().Substring(0, 7);
                user.Phone = req.Phone;
                user.Gender = req.Gender ?? true;
                user.DateOfBirth = req.DateOfBirth ?? DateTime.MinValue;
                user.Image = req.Image ?? "none";
                user.CreateAt = req.CreateAt ?? DateTime.Now;
                user.NumberLike = 0;
                user.NumberDislike = 0;
                user.NumberActivityJoin = 0;
                user.NumberActivitySuccess = 0;

                // build data return
                res.user = user;

                //res.user.FullName = user.FullName;
                //res.user.Phone = user.Phone;
                //res.user.Gender = user.Gender;
                //res.user.Image = user.Image;
                //res.user.CreateAt = user.CreateAt;

                //!response data
                user.Status = req.Status ?? "Active";
                user.RoleId = req.RoleId ?? "role1";/////////////

                await this._context.User.AddAsync(user);
                await this._context.SaveChangesAsync();

                res.resultCode = SVCWCode.SUCCESS;
                res.resultMsg = "Tạo tài khoản thành công!";
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool isValidCreateData(CreateUserReq req, CommonUserRes res)
        {
            var usrEmail = req.Email;
            if (!isValidEmail(usrEmail))
            {
                res.resultCode = SVCWCode.InvalidInput;
                res.resultMsg = "Email không hợp lệ! Email sai format hoặc tổ chức chưa được tích hợp!";
                return false;
            }
            return true;
        }

        public async Task<CommonUserRes> validateLoginUser(LoginReq req)
        {
            try
            {
                var res = new CommonUserRes();

                // validate jobs -- build res data
                if (!isValidLogin(req, res)) return res;

                // get user data
                var user = await this._context.User.Where(x => x.Email.Equals(req.Email)).FirstOrDefaultAsync();

                if (user == null)
                {
                    res.resultCode = SVCWCode.FirstTLogin;
                    res.resultMsg = "User lần đầu đăng nhập hệ thống!";
                    return res;
                }

                // build data response - sau này cũng nên tách hàm
                res.user = user;

                res.resultCode = SVCWCode.SUCCESS;
                res.resultMsg = "Đăng nhập thành công!";
                return res;
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool isValidLogin(LoginReq req, CommonUserRes res)
        {
            var usrEmail = req.Email;
            if (!isValidEmail(usrEmail))
            {
                res.resultCode = SVCWCode.InvalidEmail;
                res.resultMsg = "Email không hợp lệ! Email sai format hoặc tổ chức chưa được tích hợp!";
                return false;
            }
            return true;
        }

        private bool isValidEmail(string usrEmail) 
        {
            // Hiện tại đang set cứng, sau này phải check trong list domain của các trường mình đã intergrate
            try
            {
            return usrEmail.Split('@')[1].Equals("fpt.edu.vn");
            }
            catch {
                return false;
            }
        }

        public async Task<CommonUserRes> updateUser(UpdateUserReq req)
        {
            try
            {
                var res = new CommonUserRes();

                var user = await this._context.User.Where(x => x.UserId.Equals(req.UserId)).FirstOrDefaultAsync();

                if (user == null)
                {
                    res.resultCode = SVCWCode.UserNotExist;
                    res.resultMsg = "Không tìm thấy User Id: " + req.UserId + "!";
                    return res;
                }
                user.Username = req.Username ?? user.Username;
                user.Password = req.Password ?? user.Password;
                user.FullName = req.FullName ?? user.FullName;
                user.Phone = req.Phone ?? user.Phone;
                user.Status = req.Status ?? user.Status;
                user.Gender = req.Gender ?? user.Gender;
                user.Image = req.Image ?? user.Image;
                user.DateOfBirth = req.DateOfBirth ?? user.DateOfBirth;
                user.Status = req.Status ?? user.Status;
                user.RoleId = req.RoleId ?? user.RoleId;

                await this._context.SaveChangesAsync();

                //build data return
                res.user = user;

                res.resultCode = SVCWCode.SUCCESS;
                res.resultMsg = "Thông tin người đùng đã được cập nhật!";
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex:" + ex.Message);
                var res = new CommonUserRes();
                res.resultCode = SVCWCode.Unknown;
                res.resultMsg = "Lỗi hệ thống!";
                return res;
            }
        }

        public async Task<List<User>> getAllUser()
        {
            try
            {
                var users = await this._context.User.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<FollowJoinAvtivity>> historyUserJoin(string id)
        {
            try
            {
                var check = await this._context.FollowJoinAvtivity.Where(x=>x.UserId.Equals(id))
                    .Include(x=>x.Activity)
                    .ToListAsync();
                if(check != null)
                {
                    return check;
                }
                else {
                    throw new Exception("not have data");
                }
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
