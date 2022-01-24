using AutoMapper;
using Chat.Web.ViewModels;
using Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System_Analysis.Models;

namespace Socket
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppHub : Hub
    {

        private readonly PresenceTracker _presenceTracker;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public AppHub(PresenceTracker presenceTracker, ApplicationDbContext dbContext, IMapper mapper)
        {
            _presenceTracker = presenceTracker;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var dbUser = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();

            _presenceTracker.AddConnectionMap(userId, Context.ConnectionId);

            var groupList = _dbContext.Groups.Where(x => x.Users.Any(x => x.Id == userId))
                .OrderByDescending(o => o.GroupMessages.Select(s => s.Timestamp))
                .ToList();

            var privateIds = _dbContext.PrivateMessages
                .Where(x => x.UserId == userId || x.ToUserId == userId)
                .Select(x => x.ToUserId)
                .Distinct()
                .ToList();

            var privateList = _dbContext.Users
                .Where(x => privateIds.Contains(x.Id))
                .OrderByDescending(o => o.PrivateMessages.Select(s => s.Timestamp))
                .ToList();

            foreach (var user in privateList)
            {
                var pvConnection = _presenceTracker.GetConnectionMap(user.Id);

                if (pvConnection != null)
                    await _presenceTracker.UserConnected(userId, pvConnection);
            }

            foreach (var group in groupList)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            }

            var userViewModel = _mapper.Map<List<User>, List<UserViewModel>>(privateList);

            var groupViewModel = _mapper.Map<List<System_Analysis.Models.Group>, List<GroupViewModel>>(groupList);


            await Clients.Caller.SendAsync("getConnections", groupViewModel, userViewModel);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var groupList = _dbContext.Groups.Where(x => x.Users.Any(x => x.Id == userId))
                .OrderByDescending(o => o.GroupMessages.Select(s => s.Timestamp))
                .ToList();

            var privateIds = _dbContext.PrivateMessages
                .Where(x => x.UserId == userId || x.ToUserId == userId)
                .Select(x => x.ToUserId)
                .Distinct()
                .ToList();

            var privateList = _dbContext.Users
                .Where(x => privateIds.Contains(x.Id))
                .OrderByDescending(o => o.PrivateMessages.Select(s => s.Timestamp))
                .ToList();

            foreach (var user in privateList)
            {
                var pvConnection = _presenceTracker.GetConnectionMap(user.Id);

                if (pvConnection != null)
                    await _presenceTracker.UserDisconnected(userId, pvConnection);
            }

            foreach (var group in groupList)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivate(string username, string message)
        {
            var user = Guid.Parse(Context.UserIdentifier);

            var sender = _dbContext.Users.Where(x => x.Id == user).FirstOrDefault();
            var reciever = _dbContext.Users.Where(x => x.UserName == username).FirstOrDefault();

            // Build the message
            var messageViewModel = new MessageViewModel()
            {
                Content = Regex.Replace(message, @"<.*?>", string.Empty),
                From = sender.FirstName + " " + sender.LastName,
                Avatar = sender.SnapShot,
                Timestamp = DateTime.Now.ToLongTimeString()
            };

            var recieverConnId = _presenceTracker.GetConnectionMap(reciever.Id);

            if (!string.IsNullOrEmpty(message.Trim()))
            {
                if (recieverConnId != null)
                {
                    // Send the message
                    await Clients.Client(recieverConnId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
                var time = DateTime.Now;
                _dbContext.PrivateMessages.Add(new PrivateMessage
                {
                    Content = Regex.Replace(message, @"<.*?>", string.Empty),
                    Timestamp = time,
                    UserId = sender.Id,
                    ToUserId = reciever.Id,
                });

                _dbContext.SaveChanges();

            }
        }
        public async Task SendToGroup(Guid groupId, string message)
        {
            var group = _dbContext.Groups.Where(x => x.Id == groupId).FirstOrDefault();

            await Clients.Groups(group.Id.ToString()).SendAsync(message);

        }

    }
}
