﻿using SVCW.DTOs.Admin_Moderator.Moderator;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IModerator
    {
        Task<List<User>> getAll();
        Task<User> getModerator(string id);
        Task<User> login(LoginModerator dto);
        Task<User> create(CreateModerator dto);
        Task<User> update(updateModerator dto);
        Task<User> delete(string id);
    }
}
