using BlazorSignalRApp.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Hubs
{
    public class MatchHub : Hub
    {
        private Dictionary<string, LanguageQueue> _queueMap;

        public MatchHub(Dictionary<string, LanguageQueue> queueMap)
        {
            _queueMap = queueMap;
        }

        public async Task JoinQueue(UserType userType, string language)
        {
            if(_queueMap.ContainsKey(language))
            {
                var queue = _queueMap[language];
                if (userType == UserType.Helper)
                {
                    // check for someone who needs help
                    if(queue.Users.Count > 0)
                    {
                        var user = queue.Users[0];
                        queue.Users.RemoveAt(0);
                        string roomId = Guid.NewGuid().ToString();
                        await Clients.Client(user).SendAsync("Matched", "System", roomId);
                        await Clients.Caller.SendAsync("Matched", "System", roomId);
                    }
                    else
                    {
                        queue.Helpers.Add(Context.ConnectionId);
                    }
                }
                else
                {
                    if (queue.Helpers.Count > 0)
                    {
                        var helper = queue.Helpers[0];
                        queue.Helpers.RemoveAt(0);
                        string roomId = Guid.NewGuid().ToString();
                        await Clients.Client(helper).SendAsync("Matched", "System", roomId);
                        await Clients.Caller.SendAsync("Matched", "System", roomId);
                    }
                    else
                    {
                        queue.Users.Add(Context.ConnectionId);
                    }
                }
            }
            else
            {
                LanguageQueue newQueue = new LanguageQueue();
                if (userType == UserType.Helper)
                {
                    newQueue.Helpers.Add(Context.ConnectionId);
                }
                else
                {
                    newQueue.Users.Add(Context.ConnectionId);
                }

                _queueMap.Add(language, newQueue);
            }
        }

        public void LeaveQueue(UserType userType, string language)
        {
            if (_queueMap.ContainsKey(language))
            {
                var queue = _queueMap[language];
                if (userType == UserType.Helper)
                {
                    queue.Helpers.Remove(Context.ConnectionId);
                }
                else
                {
                    queue.Users.Remove(Context.ConnectionId);
                }
            }
        }
    }
}
