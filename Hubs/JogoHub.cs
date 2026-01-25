using Microsoft.AspNetCore.SignalR;

namespace Jogos_Backlogger.Hubs
{
    public class JogoHub : Hub
    {
        public async Task JoinUserGroup(string usuarioId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{usuarioId}");
        }
    }
}
