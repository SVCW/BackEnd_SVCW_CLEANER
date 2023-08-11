﻿using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Admin_Moderator.Moderator;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class ModeratorService : IModerator
    {
        private readonly SVCWContext _context;
        public ModeratorService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<User> create(CreateModerator dto)
        {
            try
            {
                var check = await this._context.User.Where(x=>x.Email.Equals(dto.Email) ).FirstOrDefaultAsync(); 
                if (check == null)
                {
                    check = await this._context.User.Where(x => x.Username.Equals(dto.Username)).FirstOrDefaultAsync();
                    if (check != null)
                    {
                        throw new Exception("trùng username");
                    }
                    check = await this._context.User.Where(x => x.Phone.Equals(dto.Phone)).FirstOrDefaultAsync();
                    if (check!=null)
                    {
                        throw new Exception("trùng số điện thoại");
                    }
                }
                else
                {
                    throw new Exception("trùng email");
                }
                var user = new User();
                user.UserId = "MDR" + Guid.NewGuid().ToString().Substring(0, 7);
                //maping
                user.Email = dto.Email;
                user.FullName = dto.FullName ?? "none";
                user.Username = dto.Username;
                user.Password = dto.Password ?? "PWD" + Guid.NewGuid().ToString().Substring(0, 7);
                user.Phone = dto.Phone;
                user.Gender = dto.Gender ?? true;
                user.DateOfBirth = dto.DateOfBirth ?? DateTime.MinValue;
                user.Image = dto.Image ?? "none";
                user.CoverImage = dto.CoverImage ?? "none";
                user.CreateAt = DateTime.Now;
                user.NumberLike = 0;
                user.NumberDislike = 0;
                user.NumberActivityJoin = 0;
                user.NumberActivitySuccess = 0;
                user.Status = "Active";
                user.RoleId = "role2";

                this._context.User.Add(user);
                await this._context.SaveChangesAsync();
                return user;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> delete(string id)
        {
            try
            {
                var check = await this._context.User.Where(x => x.UserId.Equals(id)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "InActive";
                    this._context.User.Update(check);
                    await this._context.SaveChangesAsync();
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> getAll()
        {
            try
            {
                var check = await this._context.User.Where(x => x.UserId.Contains("MDR")).ToListAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> getModerator(string id)
        {
            try
            {
                var check = await this._context.User.Where(x => x.UserId.Equals(id)).FirstOrDefaultAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> login(LoginModerator dto)
        {
            try
            {
                var check = await this._context.User.Where(x=>x.Username.Equals(dto.Username) && x.Password.Equals(dto.Password)).FirstOrDefaultAsync();
                if(check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Lỗi khi đăng nhập do tên đăng nhập hoặc password không đúng");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> update(updateModerator dto)
        {
            try
            {
                var check = await this._context.User.Where(x=>x.UserId.Equals(dto.UserId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Username = check.Username;
                    check.Password = dto.Password ?? check.Password;
                    check.FullName = dto.FullName ?? check.FullName;
                    check.Phone = check.Phone;
                    check.Status = check.Status;
                    check.Gender = dto.Gender ?? check.Gender;
                    check.Image = dto.Image ?? check.Image;
                    check.CoverImage = dto.CoverImage ?? check.CoverImage;
                    check.DateOfBirth = dto.DateOfBirth ?? check.DateOfBirth;
                    check.Status = check.Status;
                    check.RoleId = check.RoleId;
                    this._context.User.Update(check);
                    await this._context.SaveChangesAsync();
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
