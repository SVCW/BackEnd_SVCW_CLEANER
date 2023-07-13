using SVCW.DTOs.Comments;
using SVCW.DTOs.Notifications;
using SVCW.Models;


namespace SVCW.Interfaces
{
	public interface INotification
	{
        Task<Notification> newNoti(NotificationDTO comment);
        Task<bool> markAsRead(string notiId);
        Task<List<Notification>> GetUserNotis(string userId);
        Task<Notification> UpdateNoti(string notiId, NotificationDTO notiInfo);
        Task<bool> DeleteNoti(string notiId);
    }
}