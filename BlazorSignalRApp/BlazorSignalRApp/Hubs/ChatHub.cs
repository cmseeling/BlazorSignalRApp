using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs;

public class ChatHub : Hub
{
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        // Notify the client that they've successfully joined the room
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"You have joined the room: {roomName}");
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task SendMessage(string user, string message, string roomName)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
    }
}
