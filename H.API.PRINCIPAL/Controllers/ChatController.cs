using H.DTOs;
using H.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<ChatResponseDTO> Post([FromBody] ChatRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { message = "Message is required." });

            int? personaId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsed)
                    ? parsed
                    : 0;
                if (userId > 0)
                    personaId = _chatService.GetPersonaId(userId);
            }

            // IdPersona from the request is intentionally ignored. It is client-controlled.
            return Ok(_chatService.Respond(request, User.Identity?.IsAuthenticated == true, personaId));
        }
    }
}
