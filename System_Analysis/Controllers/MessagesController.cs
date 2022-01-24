using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Chat.Web.ViewModels;
using System.Text.RegularExpressions;
using System_Analysis.Models;
using Socket;
using Entities.Identity;
using System.Security.Claims;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHubContext<AppHub> _hubContext;

        public MessagesController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<AppHub> hubContext)
        {
            _dbContext = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<System_Analysis.Models.Group>> Get(int id)
        //{
        //    var message = await _dbContext.GroupMessages.FindAsync(id);
        //    if (message == null)
        //        return NotFound();

        //    var messageViewModel = _mapper.Map<GroupMessage, MessageViewModel>(message);
        //    return Ok(messageViewModel);
        //}

        [HttpGet("private/{recieverId}")]
        public ActionResult<List<MessageViewModel>> GetPrivateMessage(Guid recieverId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var privateMsg = _dbContext.PrivateMessages.Where(x => x.ToUserId == recieverId)
                .Select(s => new MessageViewModel
                {
                    Avatar = s.User.SnapShot,
                    Content = s.Content,
                    From = _mapper.Map<User,UserViewModel>(s.User),
                    Group = null,
                    IsMine = s.UserId.Equals(userId),
                })
                .ToList();

            return Ok(privateMsg);
        }

        [HttpGet("Group/{groupName}")]
        public IActionResult GetGroupMessages(string groupName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var group = _dbContext.Groups.FirstOrDefault(r => r.Name == groupName);
            if (group == null)
                return BadRequest();

            var messages = _dbContext.GroupMessages.Where(m => m.ToGroupId == group.Id)
                .Include(m => m.FromUser)
                .Include(m => m.ToGroup)
                .OrderByDescending(m => m.Timestamp)
                .Take(20)
                .AsEnumerable()
                .Reverse()
                .Select(s  => new MessageViewModel
                {
                    Avatar = s.FromUser.SnapShot,
                    Content = s.Content,
                    From = _mapper.Map<User, UserViewModel>(s.FromUser),
                    To = null,
                    Group = _mapper.Map<System_Analysis.Models.Group, GroupViewModel>(s.ToGroup),
                    IsMine = s.FromUser.Equals(userId),
                    Timestamp  = s.Timestamp
                })
                .ToList();

            //var messagesViewModel = _mapper.Map<IEnumerable<GroupMessage>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messages);
        }

        //[HttpPost]
        //public async Task<ActionResult<GroupMessage>> Create(MessageViewModel messageViewModel)
        //{
        //    var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        //    var group = _dbContext.Groups.FirstOrDefault(r => r.Name == messageViewModel.);
        //    if (group == null)
        //        return BadRequest();

        //    var msg = new GroupMessage()
        //    {
        //        Content = Regex.Replace(messageViewModel.Content, @"<.*?>", string.Empty),
        //        FromUser = user,
        //        ToGroup = group,
        //        Timestamp = DateTime.Now
        //    };

        //    _dbContext.GroupMessages.Add(msg);
        //    await _dbContext.SaveChangesAsync();

        //    // Broadcast the message
        //    var createdMessage = _mapper.Map<GroupMessage, MessageViewModel>(msg);
        //    await _hubContext.Clients.Group(group.Name).SendAsync("newMessage", createdMessage);

        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var message = await _dbContext.GroupMessages
                .Include(u => u.FromUser)
                .Where(m => m.Id == id && m.FromUser.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (message == null)
                return NotFound();

            _dbContext.GroupMessages.Remove(message);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
