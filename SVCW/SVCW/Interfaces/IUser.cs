using SVCW.DTOs.Users.Req;
using SVCW.DTOs.Users.Res;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IUser
    {
        Task<List<User>> getAllUser();
        Task<CommonUserRes> createUser(CreateUserReq req);
        Task<CommonUserRes> validateLoginUser(LoginReq req);
        Task<CommonUserRes> updateUser(UpdateUserReq req);
        Task<List<FollowJoinAvtivity>> historyUserJoin(string id);
    }
}
