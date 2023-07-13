using System;
namespace SVCW.DTOs.Notifications
{
	public class NotificationDTO
	{
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? NotificationContent { get; set; }
        public DateTime? Datetime { get; set; }
        public bool? isRead { get; set; }
    }
}

