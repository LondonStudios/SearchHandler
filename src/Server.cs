using System;
using CitizenFX.Core;

namespace Server
{
    public class Server : BaseScript
    {
        //London Studios
        public Server()
        {
            EventHandlers["Server:RequestSearch"] += new Action<int, int, string, Vector3>((local, request, name, location) =>
            {
                TriggerClientEvent("Client:RequestSearch", local, request, name, location);
            });

            EventHandlers["Server:ReturnSearch"] += new Action<int, int, string, string, bool>((local, request, name, contents, success) =>
            {
                TriggerClientEvent("Client:ReturnSearch", local, request, name, contents, success);
            });
        }
    }
}
