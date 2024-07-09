using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.DTO;
using RealtimeChat.Entities;
using RealtimeChat.Models;

namespace RealtimeChat.Controllers
{
    [Authorize]
    public class ChatController : UserControllerBase
    {
        IMapper mapper;

        public ChatController(IMapper mapper,ApplicationContext db):base(db)
        {
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return BadRequest();
            var userDto = mapper.Map<PublicUserDTO>(user);

            var sessions = await db.ChatSessions.Where(x => x.Users.Any(x => x.Id == user.Id)).Include(x=>x.Users).ToListAsync();

            var sessionDto = mapper.Map<List<ChatSessionDTO>>(sessions);
            
            
            var model = new ChatViewModel
            {
                CurrentUser = userDto,
                Sessions = sessionDto
            };
            return View(model);
        }
        [HttpGet]
        [Route("[controller]/messages/{id}")]
        public async Task<IActionResult> GetMessagesOfSession(int id)
        {
            var l = await db.Messages.Where(x => x.ChatSessionId == id).Include(x=>x.User).ToListAsync();
            var dto = mapper.Map<List<MessageDTO>>(l);
            return Ok(dto);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, int sessionId)
        {
            var user = await GetCurrentUser();
            if (user == null) return BadRequest();
            var session = await db.ChatSessions.FirstOrDefaultAsync(x=>x.Id == sessionId && x.Users.Any(x=>x.Id == user.Id));
            if (session == null) return BadRequest();
            Message msg = new Message
            {
                UserId = user.Id,
                Text = message,
                ChatSessionId = session.Id,
                Date = DateTime.Now,
            };
            db.Messages.Add(msg);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
