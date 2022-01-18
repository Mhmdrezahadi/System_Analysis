using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Socket
{
    [EnableCors("MyPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        public AppHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var isOnline = await _presenceTracker.UserConnected(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Guid.Parse(Context.UserIdentifier);

            var isOffline = await _presenceTracker.UserDisconnected(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            var user = Context.User;
            

        }

        // private readonly AppDbContext _db;
        // public ChatHub(AppDbContext db)
        // {
        //     _db = db;
        // }
        // [Authorize(AuthenticationSchemes = "Customer")]
        // public async Task SendMessage([Required] MessageTextDto messageText)
        // {
        //     var user = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //     var chatFilterWords = _db.ChatFilterWords.Select(p => p.WordName).ToList();
        //     if (chatFilterWords.Any(F => messageText.Text.Contains(F)))
        //     {
        //         await Clients.Caller.SendAsync("ReceiveMessage", "your message contains forbidden words");
        //     }
        //     else
        //     {
        //         var dbCustomer = _db.Customers
        //             .Where(p => p.Id == user)
        //             .FirstOrDefault();
        //         if (dbCustomer == null)
        //         {
        //             await Clients.Caller.SendAsync("ReceiveMessage", "you must be customer");
        //         }
        //         else
        //         {
        //             if (!dbCustomer.IsActive)
        //             {
        //                 await Clients.Caller.SendAsync("ReceiveMessage", "you are banned from chat");
        //             }
        //             else
        //             {
        //                 var addMessage = new Message
        //                 {
        //                     Customer = dbCustomer,
        //                     DislikeCounts = 0,
        //                     LikeCounts = 0,
        //                     Text = messageText.Text,
        //                     DateTime = DateTime.Now,
        //                     ParentMessageId = messageText.ParentMessageId
        //                 };

        //                 _db.Messages.Add(addMessage);
        //                 _db.SaveChanges();

        //                 /////////////////////////
        //                 var messageDto = _db.Messages
        //                     .Where(p => p.Id == addMessage.Id)
        //                     .Include(p => p.ParentMessage)
        //                     .Include(p => p.MessageActions.Where(p => p.CustomerId == user && p.MessageId == addMessage.Id))
        //                     .Include(p => p.Customer)
        //                     .AsEnumerable()
        //                     .Select(p => new MessageDto
        //                     {
        //                         Id = p.Id,
        //                         Text = p.Text,
        //                         CustomerName = p.Customer.Name,
        //                         CustomerId = p.CustomerId,
        //                         LikeCounts = p.LikeCounts,
        //                         DislikeCounts = p.DislikeCounts,
        //                         DateTime = p.DateTime,
        //                         IsLiked = p.MessageActions.Where(p => p.ActionType == ActionType.Like).Any(),
        //                         IsDisliked = p.MessageActions.Where(p => p.ActionType == ActionType.Dislike).Any(),
        //                         ParentMessageDto = p.ParentMessage is not null ? new ParentMessageDto
        //                         {
        //                             Text = p.ParentMessage.Text,
        //                             Id = p.ParentMessage.Id,
        //                             CustomerId = p.ParentMessage.CustomerId,
        //                             CustomerName = p.ParentMessage.Customer.Name
        //                         } : null
        //                     }).FirstOrDefault();
        //                 await Clients.All.SendAsync("ReceiveMessage", messageDto);
        //             }
        //         }
        //     }
        // }
        // [Authorize(AuthenticationSchemes = "Customer")]
        // public async Task GetRecentMessages(ListRequest request)
        // {
        //     var page = request?.Page ?? 0;
        //     var pageSize = request?.PageSize ?? 10;
        //     if (pageSize > 100) pageSize = 100;
        //     if (pageSize < 10) pageSize = 10;

        //     var user = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //     var message = _db.Messages
        //         .Include(p => p.MessageActions)
        //         .Include(p => p.ParentMessage)
        //         .Include(p => p.Customer)
        //         .AsEnumerable()
        //         .Select(p => new MessageDto
        //         {
        //             Id = p.Id,
        //             Text = p.Text,
        //             CustomerName = p.Customer.Name,
        //             CustomerId = p.CustomerId,
        //             LikeCounts = p.LikeCounts,
        //             DislikeCounts = p.DislikeCounts,
        //             DateTime = p.DateTime,
        //             IsLiked = p.MessageActions.Where(p => p.ActionType == ActionType.Like && p.CustomerId == user).Any(),
        //             IsDisliked = p.MessageActions.Where(p => p.ActionType == ActionType.Dislike && p.CustomerId == user).Any(),
        //             ParentMessageDto = p.ParentMessage is not null ? new ParentMessageDto
        //             {
        //                 Text = p.ParentMessage.Text,
        //                 Id = p.ParentMessage.Id,
        //                 CustomerId = p.ParentMessage.CustomerId,
        //                 CustomerName = p.ParentMessage.Customer.Name
        //             } : null
        //         })
        //         .OrderBy(p => p.DateTime)
        //         .Skip(page * pageSize)
        //         .Take(pageSize)
        //         .ToList();

        //     await Clients.Caller.SendAsync("RecentChats", message);
        // }

    }
}
