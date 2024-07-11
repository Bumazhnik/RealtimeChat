using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.DTO;
using RealtimeChat.Entities;
using RealtimeChat.Hubs;
using RealtimeChat.Models;

namespace RealtimeChat.Controllers
{
    [Authorize]
    public class ChatController : UserControllerBase
    {
        private readonly IMapper mapper;
        private readonly IHubContext<ChatHub> hub;
        private readonly UserConnectionManager manager;

        public ChatController(IMapper mapper, ApplicationContext db, IHubContext<ChatHub> hub, UserConnectionManager manager) : base(db)
        {
            this.mapper = mapper;
            this.hub = hub;
            this.manager = manager;
        }


        [HttpGet]
        public IActionResult MakeSession()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login","Account");

            var sessions = await db.ChatSessions.Where(x => x.Users.Any(x => x.Id == user.Id)).Include(x => x.Users).ToListAsync();

            var sessionDto = mapper.Map<List<ChatSessionDTO>>(sessions);


            var model = new ChatViewModel
            {
                Sessions = sessionDto
            };
            return View(model);
        }
        [HttpGet]
        [Route("[controller]/sessionInfo/{id}")]
        public async Task<IActionResult> SessionInfo(int id)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return BadRequest();

            var session = await GetSessionOfUser(id,user);
            if(session == null)
                return BadRequest();

            var sessionDto = mapper.Map<ChatSessionDTO>(session);
            return Ok(sessionDto);
        }
        [HttpGet]
        [Route("[controller]/messages/{id}")]
        public async Task<IActionResult> GetMessagesOfSession(int id)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return BadRequest();

            var session = await GetSessionOfUser(id, user);
            if (session == null)
                return BadRequest();

            var l = await db.Messages.Where(x => x.ChatSessionId == id).Include(x=>x.User).ToListAsync();
            var dto = mapper.Map<List<MessageDTO>>(l);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, int sessionId)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest();
            var user = await GetCurrentUser();
            if (user == null) return BadRequest();
            var session = await GetSessionOfUser(sessionId, user);
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
            var dto = mapper.Map<MessageDTO>(msg);
            var sessionUsers = await db.Users.Where(x => x.Sessions.Any(x => x.Id == session.Id)).ToListAsync();
            var targetUsers = sessionUsers
                .Select(x => x.Id)
                .Intersect(manager.ConnectedIds.Select(x => x.Key));
            foreach(var userId in targetUsers)
            {
                foreach(var connectionId in manager.ConnectedIds[userId].Keys())
                {
                    await hub.Clients
                    .Client(connectionId)
                    .SendAsync("MessageReceived", dto);
                }

            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> MakeSession(MakeSessionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var myUser = await GetCurrentUser();
            if (myUser == null) return BadRequest();
            var users = await db.Users.Where(x=>model.UserNames.Contains(x.Name)).ToListAsync();
            if(users.Count != model.UserNames.Length)
            {
                var invalidNames = model.UserNames.Except(users.Select(x=>x.Name)).ToList();
                ModelState.AddModelError("",$"Invalid user names: {string.Join(',', invalidNames)}");
                return View(model);
            }
            var session = new ChatSession
            {
                Name = model.SessionName,
                OwnerId = myUser.Id,
                Users = [myUser, .. users]
            };
            db.ChatSessions.Add(session);
            await db.SaveChangesAsync();
            var dto = mapper.Map<ChatSessionDTO>(session);
            var targetUsers = session.Users
                .Select(x => x.Id)
                .Intersect(manager.ConnectedIds.Select(x => x.Key));
            foreach (var userId in targetUsers)
            {
                foreach (var connectionId in manager.ConnectedIds[userId].Keys())
                {
                    await hub.Clients
                    .Client(connectionId)
                    .SendAsync("SessionCreated", dto);
                }

            }
            return RedirectToAction("Index","Chat");
        }


        private async Task<ChatSession?> GetSessionOfUser(int sessionId, User user)
        {
            return await db.ChatSessions.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == sessionId && x.Users.Any(x => x.Id == user.Id));
        }
    }
}
