using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Common;
using SVCW.DTOs.Users;
using SVCW.DTOs.Users.Req;
using SVCW.DTOs.Users.Res;
using SVCW.Interfaces;
using SVCW.Models;
using Microsoft.IdentityModel.Tokens;
using SVCW.DTOs.JWT;
using System.Security.Principal;
using SVCW.DTOs.Email;

namespace SVCW.Services
{
    public class UserService : IUser
    {
        private readonly SVCWContext _context;

        private readonly IConfiguration _config;

        private readonly IEmail _service;
        public UserService(SVCWContext context, IConfiguration config, IEmail service)
        {
            _context = context;
            _config = config;
            this._service = service;
        }
        public string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                //new Claim(ClaimTypes.MobilePhone, account.Phone ?? ""),
                new Claim(ClaimTypes.Role, user.RoleId),
            };

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(120), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<JwtTokenDto> LoginUserName(LoginDTO dto)
        {
            //hàm bâm
            /*var a = BCrypt.Net.BCrypt.HashPassword("PWDf37157e");
            Console.WriteLine(a);
            , StringComparison.InvariantCultureIgnoreCase
             */

            try
            {
                var account = await this._context.User.Where(x => x.Username.Equals(dto.username) && x.Status.Equals("Active")).FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new Exception("Không tìm thấy tài khoản");
                }
                if (!(BCrypt.Net.BCrypt.Verify(dto.password, account.Password)))
                {
                    throw new Exception("Tài khoản hoặc mật khẩu không đúng!");
                }

                string token = GenerateJSONWebToken(account);

                var response = new JwtTokenDto
                {
                    accountid = account.UserId,
                    Username = account.Username,
                    Email = account.Email,
                    // Phone = account.Phone,
                    jwtToken = token
                };
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                if (check != null )
                {
                    if (check.UserId.Contains("MDR"))
                    {
                        res.isModer = true;
                    }
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
                var tmpPass = user.Password;
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Phone = req.Phone;
                user.Gender = req.Gender ?? true;
                user.DateOfBirth = req.DateOfBirth ?? DateTime.MinValue;
                user.Image = req.Image ?? "none";
                user.CoverImage = req.coverImage ?? "none";
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
                user.Status = "Active";
                user.RoleId = req.RoleId ?? "role1";/////////////

                await this._context.User.AddAsync(user);
                await this._context.SaveChangesAsync();

                //build data sendEmail account

                var mail = new SendEmailReqDTO();
                mail.sendTo = user.Email;
                mail.subject = "Tài khoản của bạn đã được đăng ký trên hệ thống SVCW";
                mail.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Tài khoản của bạn đã được đăng ký lần đầu </title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Tài khoản của bạn đã được đăng ký</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào " + user.Email + ",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng tài khoản " + user.Email + " đã được tạo trên hệ thống. Do đó, chúng tôi xin gửi tới bạn thông tin tài khoản của bạn để bạn có thể đăng nhập vào hệ thống sau này.</p>\r\n        <p>Thông tin tài khoản của bạn:</p>\r\n<p>Username: "+user.Username+ "</p>\r\n<p>Password: "+tmpPass+"</p>\r\n        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                _service.sendEmail(mail);

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

                if(user!= null)
                {
                    if (user.Status.Equals("Banned"))
                    {
                        var check = await this._context.BanUser.Where(x => x.UserId.Equals(user.UserId)).FirstOrDefaultAsync();
                        res.resultCode = SVCWCode.BANNED;
                        res.resultMsg = "Tài khoản của bạn đã bị khóa với lý do là " + check.ReasonBan;
                        res.isBan = true;
                        return res;
                    }
                }
                

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
                if(req.Password!= null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(req.Password) ?? user.Password;
                }
                user.FullName = req.FullName ?? user.FullName;
                user.Phone = req.Phone ?? user.Phone;
                user.Status = req.Status ?? user.Status;
                user.Gender = req.Gender ?? user.Gender;
                user.Image = req.Image ?? user.Image;
                user.DateOfBirth = req.DateOfBirth ?? user.DateOfBirth;
                user.Status = req.Status ?? user.Status;
                user.RoleId = req.RoleId ?? user.RoleId;
                user.CoverImage = req.CoverImage ?? user.CoverImage;

                this._context.User.Update(user);
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
                var users = await this._context.User
                    .Where(x=>!x.UserId.Contains("MDR"))
                    .Include(u => u.Activity)        // Include the related activities
                    .Include(u => u.Fanpage)        // Include the related fanpage
                    .Include(u => u.Donation)
                    .Include(u => u.FollowJoinAvtivity)
                    .Include(u => u.AchivementUser)
                        .ThenInclude(u => u.Achivement)
                    .Include(u => u.Comment)
                    .Include(u => u.ReportUser)
                    .Include(u => u.BankAccount)
                    .Include(u => u.Like)
                    .Include(u => u.BanUser.Where(x => x.Status))
                    .Include(u => u.VoteUserVote)// Include the related fanpage
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    public async Task<CommonUserRes> changeUserPassword(ChangePwReq req)
        {
            try
            {
                var res = new CommonUserRes();
                var user = await this._context.User.Where(u => u.UserId.Equals(req.UserId)).FirstOrDefaultAsync();
                if (!(BCrypt.Net.BCrypt.Verify(req.oldPassword, user.Password)))
                {
                    res.resultCode = SVCWCode.InvalidPassword;
                    res.resultMsg = "Sai mật khẩu!";
                    return res;
                }
                if (!user.Password.Equals(req.oldPassword))
                {
                    res.resultCode = SVCWCode.InvalidPassword;
                    res.resultMsg = "Sai mật khẩu!";
                    return res;
                }

                if (!isValidPassword(req.newPassword))
                {
                    res.resultCode = SVCWCode.InvalidNewPassword;
                    res.resultMsg = "Mật khẩu mới không hợp lệ!";
                    return res;
                }
                // update pw
                user.Password = BCrypt.Net.BCrypt.HashPassword(req.newPassword);
                this._context.User.Update(user);
                await this._context.SaveChangesAsync();

                res.resultCode = SVCWCode.SUCCESS;
                res.resultMsg = "Mật khẩu người đùng đã được cập nhật!";
                return res;
            } catch (Exception ex)
            {
                Console.WriteLine("Ex:" + ex.Message);
                var res = new CommonUserRes();
                res.resultCode = SVCWCode.Unknown;
                res.resultMsg = "Lỗi hệ thống!";
                return res;
            }
        }

        public bool isValidPassword(string pw)
        {
            string pattern = @"^.+$"; // empty pattern
            return Regex.IsMatch(pw, pattern);
        }

        public async Task<List<FollowJoinAvtivity>> historyUserJoin(string id)
        {
            try
            {
                var check = await this._context.FollowJoinAvtivity.Where(x=>x.UserId.Equals(id))
                    .Include(x=>x.Activity)
                        .ThenInclude(x=>x.Media)
                    .Include(x=>x.User)
                    .OrderByDescending(x=>x.Datetime)
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

        public async Task<CommonUserRes> getUserById(GetUserByIdReq req)
        {
            try
            {
                var res = new CommonUserRes();

                var user = await this._context.User
                    //.Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))  // Include the related activities
                    //    .ThenInclude(x=>x.Comment)
                    //        .ThenInclude(x=>x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))  // Include the related activities
                        .ThenInclude(x => x.Like.Where(a=>a.Status))
                            .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x=>x.Media)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x => x.FollowJoinAvtivity)
                            .ThenInclude(x=>x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x => x.Donation)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                            .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                            .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime))
                                .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active") || x.Status.Equals("Quit")))
                        .ThenInclude(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                            .ThenInclude(x => x.Media)
                    .Include(u => u.Fanpage)                                            // Include the related fanpage
                    .Include(u => u.Donation)
                    .Include(u => u.FollowJoinAvtivity)
                    .Include(u => u.ReportUser)
                    .Include(u => u.BankAccount)
                    .Include(u => u.Like)
                    .Include(u => u.VoteUserVote)
                    .Include(u=>u.AchivementUser)
                        .ThenInclude(u=>u.Achivement)
                    .Include(u=>u.FollowFanpage.Where(x=>x.Status))
                        .ThenInclude(u=>u.Fanpage)
                    .Include(u => u.BanUser.Where(x => x.Status))
                    .Where(u => u.UserId.Equals(req.UserId))
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    res.resultCode = SVCWCode.UserNotExist;
                    res.resultMsg = "User không tồn tại trong hệ thống!";
                    return res;
                }

                res.user = user;
                res.resultCode = SVCWCode.SUCCESS;
                res.resultMsg = "Success";

                return res;
            } catch (Exception ex)
            {
                Console.WriteLine("Ex:" + ex.Message);
                var res = new CommonUserRes();
                res.resultCode = SVCWCode.Unknown;
                res.resultMsg = "Lỗi hệ thống!";
                return res;
            }
        }

        public async Task<User> Login(LoginDTO dto)
        {
            try
            {
                var check = await this._context.User.Where(x=>x.Username.Equals(dto.username) )
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))  // Include the related activities
                        .ThenInclude(x => x.Like.Where(a => a.Status))
                            .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                        .ThenInclude(x => x.Media)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                        .ThenInclude(x => x.FollowJoinAvtivity)
                            .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                        .ThenInclude(x => x.Donation)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                            .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                        .ThenInclude(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                            .ThenInclude(x => x.Media)
                    .Include(u => u.Fanpage)                                            // Include the related fanpage
                    .Include(u => u.Donation)
                    .Include(u => u.Comment)
                    .Include(u => u.FollowJoinAvtivity)
                    .Include(u => u.ReportUser)
                    .Include(u => u.BankAccount)
                    .Include(u => u.Like)
                    .Include(u => u.VoteUserVote)
                    .Include(u => u.AchivementUser)
                        .ThenInclude(u => u.Achivement)
                    .Include(u => u.FollowFanpage.Where(x => x.Status))
                        .ThenInclude(u => u.Fanpage)
                    .Include(u=>u.BanUser.Where(x=>x.Status))
                    .FirstOrDefaultAsync();
                if (check == null)
                {
                    throw new Exception("username is not valid");
                }
                else
                {
                    if (BCrypt.Net.BCrypt.Verify(dto.password, check.Password))
                    {
                        return check;
                    }

                    else
                    {
                        throw new Exception("password is not valid");
                    }
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProfileDTO> checkProfile(string userId)
        {
            try
            {
                var result = new ProfileDTO();
                int count = 0;
                int total = 0;
                var check = await this._context.User.Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync();
                if(check!= null)
                {
                    if(check.Username !=null) count++;
                    result.Username = check.Username; total++;
                    if(check.Password !=null) count++;
                    result.Password = check.Password; total++;
                    if (check.Email !=null) count++;
                    result.Email = check.Email; total++;
                    if (check.Phone != null && !check.Phone.Equals("none")) count++;
                    result.Phone = check.Phone; total++;
                    if (check.Image != null && !check.Image.Equals("none")) count++;
                    result.Image = check.Image; total++;
                    if (check.DateOfBirth != null && check.DateOfBirth > DateTime.MinValue) count++;
                    result.DateOfBirth = check.DateOfBirth; total++;
                    if (check.FullName != null && !check.FullName.Equals("none")) count++;
                    result.FullName = check.FullName; total++;
                    if (check.CoverImage != null && !check.CoverImage.Equals("none")) count++;
                    result.CoverImage = check.CoverImage; total++;
                    if (check.Gender!= null) count++;
                    result.Gender = check.Gender; total++;
                    double hihi = (double)count / (double)total;
                    result.total = hihi.ToString();
                    if (hihi == 0)
                    {
                        result.total = "1";
                        var arUs = await this._context.AchivementUser.Where(x => x.UserId.Equals(check.UserId) && x.AchivementId.Equals("AId5e65637")).FirstOrDefaultAsync();
                        if (arUs == null)
                        {
                            var archivement = new AchivementUser();
                            archivement.AchivementId = "AId5e65637";
                            archivement.UserId = check.UserId;
                            archivement.EndDate = DateTime.MaxValue;
                            await this._context.AchivementUser.AddAsync(archivement);
                            await this._context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        result.total = hihi.ToString();
                    }
                   
                    return result;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CommonUserRes> banUser(BanDTO req)
        {
            try
            {
                var res = new CommonUserRes();

                var check = await this._context.User.Where(x => x.UserId.Equals(req.userId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "Banned";
                    this._context.User.Update(check);
                    if(await this._context.SaveChangesAsync() > 0)
                    {
                        var ban = new BanUser();
                        ban.UserId = req.userId;
                        ban.ReasonBan = req.reasonBan;
                        ban.Datetime = DateTime.Now;
                        ban.Status = true;
                        ban.BanId = "BAN"+Guid.NewGuid().ToString().Substring(0,7);
                        await this._context.BanUser.AddAsync(ban);
                        this._context.SaveChangesAsync();

                        var email = new SendEmailReqDTO();
                        email.sendTo = check.Email;
                        email.subject = "Tài khoản của bạn đã bị vô hiệu hóa trên hệ thống SVCW";
                        email.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Tài khoản của bạn đã bị vô hiệu hóa </title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Tài khoản bị vô hiệu hóa</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào " + check.Email + ",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng tài khoản " + check.Email + " đã bị vô hiệu hóa với lí do "+ ban.ReasonBan+". Do đó, chúng tôi sẽ khóa tài khoản của bạn.</p>\r\n        <p></p>\r\n        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                        _service.sendEmail(email);

                        res.user = check;
                        res.resultCode = SVCWCode.SUCCESS;
                        res.resultMsg = "Success";
                        res.isBan = true;
                        return res;
                    }
                    else
                    {
                        res.resultCode = SVCWCode.Unknown;
                        res.resultMsg = "Lỗi hệ thống!";
                        return res;
                    }
                }
                else
                {
                    res.resultCode = SVCWCode.UserNotExist;
                    res.resultMsg = "User không tồn tại trong hệ thống!";
                    return res;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ex:" + ex.Message);
                var res = new CommonUserRes();
                res.resultCode = SVCWCode.Unknown;
                res.resultMsg = "Lỗi hệ thống!";
                return res;
            }
        }

        public async Task<bool> unBanUser(string userId)
        {
            try
            {
                var check = await this._context.User.Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync();
                var checkban = await this._context.BanUser.Where(x => x.UserId.Equals(userId) && x.Status).ToListAsync();
                if (checkban != null)
                {
                    foreach(var item in checkban)
                    {
                        item.Status = false;
                        this._context.BanUser.Update(item);
                        await this._context.SaveChangesAsync();
                    }

                    var email = new SendEmailReqDTO();
                    email.sendTo = check.Email;
                    email.subject = "Tài khoản của bạn đã được kích hoạt trên hệ thống SVCW";
                    email.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Tài khoản của bạn đã được kích hoạt </title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Tài khoản đã được kích hoạt</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào " + check.Email + ",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng tài khoản " + check.Email + " đã được kích hoạt </p>\r\n        <p></p>\r\n        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                    _service.sendEmail(email);

                    check.Status = "Active";
                    this._context.User.Update(check);
                    return await this._context.SaveChangesAsync() > 0;
                }
                else
                {
                    check.Status = "Active";
                    this._context.User.Update(check);
                    return await this._context.SaveChangesAsync() > 0;
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> personalSchedulev1(string userId)
        {
            try
            {
                var result = new List<Activity>();
                var join = await this._context.FollowJoinAvtivity.Where(x=>x.UserId.Equals(userId) && x.IsFollow == true).ToListAsync();

                if(join != null && join.Count>0)
                {
                    foreach(var item in join)
                    {
                        var activity = await this._context.Activity
                            .Where(x=>x.Status.Equals("Active") && x.ActivityId.Equals(item.ActivityId))
                            .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                                .ThenInclude(x => x.Media)
                            .Include(x => x.ActivityResult)
                            .Include(x => x.Media)
                            .OrderByDescending(x => x.CreateAt)
                            .FirstOrDefaultAsync();

                        result.Add(activity);
                    }
                    return result;
                }
                else
                {
                    throw new Exception("Bạn chưa tham gia chiến dịch nào");
                }
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> personalSchedulev2(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var check = await this._context.Activity.Where(x => x.Status.Equals("Active"))
                    .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                        .ThenInclude(x => x.Media)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity.Where(x => (x.IsJoin.Equals("Join") || x.IsJoin.Equals("success") || x.IsFollow == true) && x.UserId.Equals(userId)))
                    .Include(x => x.Media)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();

                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Bạn chưa tham gia chiến dịch nào trong khoảng thời gian từ: "+ startDate + " đến: " + endDate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> getListUserByListUserId(List<string> userId)
        {
            try
            {
                var result = new List<User>();
                if (userId.Count > 0)
                {
                    foreach (var u in userId)
                    {
                        var check = await this._context.User.Where(x => x.UserId.Equals(u.ToString())).FirstOrDefaultAsync();
                        result.Add(check);
                    }
                    return result;
                }
                else
                {
                    throw new Exception("null");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
