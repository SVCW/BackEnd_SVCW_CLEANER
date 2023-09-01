using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Roles;
using SVCW.Interfaces;
using SVCW.Models;
using System.Xml.Linq;

namespace SVCW.Services
{
    public class RoleService : IRole
    {
        private readonly SVCWContext _context;
        public RoleService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<Role> create(RoleCreateDTO role)
        {
            try
            {
                var rle = new Role();
                rle.Description = role.Description;
                rle.RoleName = role.RoleName;
                rle.RoleId = "ROLE" + Guid.NewGuid().ToString().Substring(0,6);
                rle.Status = true;

                await this._context.Role.AddAsync(rle);
                if(await this._context.SaveChangesAsync() > 0)
                {
                    return rle;
                }
                else
                {
                    throw new Exception("Save DB Fail");
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Role> delete(string id)
        {
            try
            {
                var check = await this._context.Role.Where(x => x.RoleId.Equals(id)).FirstOrDefaultAsync();
                check.Status = false;
                this._context.Role.Update(check);
                await this._context.SaveChangesAsync();
                return check;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Role> findById(string id)
        {
            try
            {
                var check = await this._context.Role.Where(x => x.RoleId.Equals(id)).FirstOrDefaultAsync();
                if(check != null)
                {
                    return check;
                }
                return null;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Role>> findByName(string name)
        {
            try
            {
                var check = await this._context.Role.Where(x => x.RoleName.Contains(name)).ToListAsync();
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

        public async Task<List<Role>> getAll()
        {
            try
            {
                var check = await this._context.Role
                    .Where(x=>x.Status)
                    .ToListAsync();
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

        public async Task<Role> update(RoleUpdateDTO role)
        {
            try
            {
                var check = await this._context.Role.Where(x => x.RoleId.Equals(role.RoleId)).FirstOrDefaultAsync();
                check.Description = role.Description;
                check.RoleName = role.RoleName;
                this._context.Role.Update(check);
                await this._context.SaveChangesAsync();

                return check;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
