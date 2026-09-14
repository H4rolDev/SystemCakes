using H.DTOs;

namespace H.Services
{
    public interface IChatService
    {
        ChatResponseDTO Respond(ChatRequestDTO request, bool isAuthenticated, int? authenticatedPersonaId);
        int? GetPersonaId(int userId);
    }
}
