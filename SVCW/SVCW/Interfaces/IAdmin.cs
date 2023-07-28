using SVCW.DTOs.Admin_Moderator.Admin;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IAdmin
    {
        Task<Admin> login(LoginAdmin dto);
        Task<Admin> update(string username, string password);
        Task<Admin> create(string username, string newpassword);
    }
}
