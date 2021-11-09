using Server.Abstractions;
using System;

namespace Server.Models
{
    public class NotificationSettings
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool UseTelegramNotifications { get; set; }
        public bool UseEmailNotifications { get; set; }

        public int TelegramChatId { get; set; }
        public string Email { get; set; }
    }
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
    }
}