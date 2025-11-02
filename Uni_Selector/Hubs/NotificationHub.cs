using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Data;

namespace Uni_Selector.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly AppDbContext _context;

        public NotificationHub(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; 

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");


            var roles = Context.User?.Claims
                .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(c => c.Value)
                .ToList();

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
                }
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

            await base.OnDisconnectedAsync(exception);
        }

        

        public async Task SendNotificationToUser(string userId, string title, string message)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
            {
                title,
                message,
                timestamp = DateTime.UtcNow,
                type = "info"
            });
        }

        

        public async Task SendNotificationToRole(string roleName, string title, string message)
        {
            await Clients.Group($"Role_{roleName}").SendAsync("ReceiveNotification", new
            {
                title,
                message,
                timestamp = DateTime.UtcNow,
                type = "info"
            });
        }

       

        public async Task SendNotificationToAll(string title, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", new
            {
                title,
                message,
                timestamp = DateTime.UtcNow,
                type = "broadcast"
            });
        }

      

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var userId = Context.UserIdentifier;
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
            }
        }

      

        public async Task<int> GetUnreadCount()
        {
            var userId = Context.UserIdentifier;
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            await Clients.Caller.SendAsync("UnreadCountUpdated", count);
            return count;
        }

      

        public async Task JoinUniversityGroup(int universityId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"University_{universityId}");
        }

       
        public async Task LeaveUniversityGroup(int universityId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"University_{universityId}");
        }
    }
}
