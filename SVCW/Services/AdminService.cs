using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Admin_Moderator.Admin;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class AdminService : IAdmin
    {
        private readonly SVCWContext _context;
        public AdminService(SVCWContext context)
        {
            _context = context;
        }

        public async Task<Admin> create(string username, string password)
        {
            try
            {
                var admin = new Admin();    
                admin.Username = username;
                admin.Password = password;
                admin.Status = true;

                await this._context.Admin.AddAsync(admin);
                if(await this._context.SaveChangesAsync() > 0)
                {
                    return admin;
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

        public async Task<Admin> login(LoginAdmin dto)
        {
            try
            {
                var check = await this._context.Admin.Where(x=>x.Username.Equals(dto.Username) && x.Password.Equals(dto.Password)).FirstOrDefaultAsync();
                if(check != null)
                {
                    return check;
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

        public async Task<Admin> update(string username, string newpassword)
        {
            try
            {
                var check = await this._context.Admin.Where(x => x.Username.Equals(username)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.Password = newpassword;
                }
                this._context.Admin.Update(check);
                await this._context.SaveChangesAsync();
                return check;
            }
            catch(Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
