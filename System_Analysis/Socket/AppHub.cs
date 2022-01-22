using AutoMapper;
using Chat.Web.ViewModels;
using Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
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

        public AppHub(PresenceTracker presenceTracker, ApplicationDbContext dbContext)
        {
            _presenceTracker = presenceTracker;
            _dbContext = dbContext;
        }
        //public override Task OnConnectedAsync()
        //{
        //    try
        //    {
        //        var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
        //        var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
        //        userViewModel.Device = GetDevice();
        //        userViewModel.CurrentRoom = "";

        //        if (!_Connections.Any(u => u.Username == IdentityName))
        //        {
        //            _Connections.Add(userViewModel);
        //            _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
        //        }

        //        Clients.Caller.SendAsync("getProfileInfo", user.FullName, user.Avatar);
        //    }
        //    catch (Exception ex)
        //    {
        //        Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        //    }
        //    return base.OnConnectedAsync();
        //}

        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    try
        //    {
        //        var user = _Connections.Where(u => u.Username == IdentityName).First();
        //        _Connections.Remove(user);

        //        // Tell other users to remove you from their list
        //        Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

        //        // Remove mapping
        //        _ConnectionsMap.Remove(user.Username);
        //    }
        //    catch (Exception ex)
        //    {
        //        Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
        //    }

        //    return base.OnDisconnectedAsync(exception);
        //}
        public override async Task OnConnectedAsync()
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var dbUser = _dbContext.Users.Where(x => x.Id == userId).FirstOrDefault();

            var isOnline = await _presenceTracker.UserConnected(userId, Context.ConnectionId);

            //var viewUser = _mapper.Map<User, UserViewModel>(dbUser);

            await Clients.Caller.SendAsync("getProfileInfo", dbUser.FirstName, dbUser.LastName, dbUser.SnapShot);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var isOffline = await _presenceTracker.UserDisconnected(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivate(string username, string message)
        {
            var user = Guid.Parse(Context.ConnectionId);

            if (await _presenceTracker.UserIsOnline(user))
            {
                var sender = _dbContext.Users.Where(x => x.Id == user).FirstOrDefault();
                var reciever = _dbContext.Users.Where(x => x.UserName == username).FirstOrDefault();

                var recieverConnections = await _presenceTracker.GetConnectionsForUser(reciever.Id);

                if (!string.IsNullOrEmpty(message.Trim()))
                {
                    // Build the message
                    var messageViewModel = new MessageViewModel()
                    {
                        Content = Regex.Replace(message, @"<.*?>", string.Empty),
                        From = sender.FirstName,
                        Avatar = sender.SnapShot,
                        Room = "",
                        Timestamp = DateTime.Now.ToLongTimeString()
                    };

                    // Send the message
                    await Clients.Client(username).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }
    }
}
