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

            var groupList = _dbContext.Users.Where(x => x.Id == dbUser.Id)
                .Include(i => i.Groups)
                .Select(x => x.Groups)
                .FirstOrDefault()
                .ToList();

            var privateList = _dbContext.Users.Where(x => x.Id == dbUser.Id)
                .SelectMany(s => s.PrivateMessageUsers)
                .Select(x => x.User)
                .Distinct()
                .ToList();

            foreach (var user in privateList)
            {
                var pvConnection = _presenceTracker.GetConnectionMap(user.Id);

                if (pvConnection != null)
                    await _presenceTracker.UserConnected(userId, pvConnection);
            }

            foreach (var group in groupList)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);
            }

            var userViewModel = _mapper.Map<List<User>, List<UserViewModel>>(privateList);

            var groupViewModel = _mapper.Map<List<System_Analysis.Models.Group>, List<GroupViewModel>>(groupList);


            await Clients.Caller.SendAsync("getConnections", new { groupList, privateList });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var isRemoved = await _presenceTracker.RemoveConnection(userId);

            var isOffline = await _presenceTracker.UserDisconnected(userId, Context.ConnectionId);

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

            if (recieverConnId != null)
            {
                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    // Send the message
                    await Clients.Client(recieverConnId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }
        public async Task SendToGroup(Guid groupId, string message)
        {
            var group = _dbContext.Groups.Where(x => x.Id == groupId).FirstOrDefault();
            await Clients.Groups(group.Name).SendAsync(message);

        }

    }
}
