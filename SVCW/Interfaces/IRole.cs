using SVCW.DTOs.Roles;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IRole
    {
        Task<Role> create(RoleCreateDTO role);
        Task<Role> update(RoleUpdateDTO role);
        Task<Role> delete(string id);
        Task<List<Role>> getAll();
        Task<Role> findById(string id);
        Task<List<Role>> findByName(string name);
    }
}
