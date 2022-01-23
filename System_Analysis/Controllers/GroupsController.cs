using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Chat.Web.ViewModels;
using System_Analysis.Models;
using Socket;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHubContext<AppHub> _hubContext;

        public GroupsController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<AppHub> hubContext)
        {
            _dbContext = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<GroupViewModel>>> Get()
        {
            var rooms = await _dbContext.Groups.ToListAsync();

            var roomsViewModel = _mapper.Map<IEnumerable<Group>, IEnumerable<GroupViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> Get(int id)
        {
            var group = await _dbContext.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            var groupViewModel = _mapper.Map<Group, GroupViewModel>(group);
            return Ok(groupViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> Create(GroupViewModel groupViewModel)
        {
            if (_dbContext.Groups.Any(r => r.Name == groupViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var room = new Group()
            {
                Name = groupViewModel.Name,
                AdminId = user.Id
            };

            _dbContext.Groups.Add(room);
            await _dbContext.SaveChangesAsync();

            // await _hubContext.Clients.All.SendAsync("addChatRoom", new { id = room.Id, name = room.Name });

            return CreatedAtAction(nameof(Get), new { id = room.Id }, new { id = room.Id, name = room.Name });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, GroupViewModel roomViewModel)
        {
            if (_dbContext.Groups.Any(r => r.Name == roomViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var admin = _dbContext.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            var room = await _dbContext.Groups
                .Include(r => r.User)
                .Where(r => r.Id == id && r.AdminId == admin.Id)
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            room.Name = roomViewModel.Name;
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("updateChatRoom", new { id = room.Id, room.Name});

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var admin = _dbContext.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

            var group = await _dbContext.Groups
                .Include(r => r.User)
                .Where(r => r.Id == id && r.AdminId == admin.Id)
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound();

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", group.Id);
            await _hubContext.Clients.Group(group.Name).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted.\nYou are moved to the first available room!", group.Name));

            return NoContent();
        }
    }
}
