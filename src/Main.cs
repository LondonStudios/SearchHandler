using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Client
{
    public class Main : BaseScript
    {
        // London Studios
        private string clientInventory;
        public Main()
        {
            TriggerEvent("chat:addSuggestion", "/setinv", "Sets your player inventory", new[]
            {
                new { name="inventory", help="Inventory contents" },
            });

            TriggerEvent("chat:addSuggestion", "/search", "Search another player", new[]
            {
                new { name="Player ID", help="Target player search ID" },
            });

            EventHandlers["Client:RequestSearch"] += new Action<int, int, string, Vector3>((local, request, name, location) =>
            {
                if (local == GetPlayerServerId(PlayerId()))
                {
                    var localLocation = GetEntityCoords(PlayerPedId(), true);
                    if (localLocation.DistanceToSquared(location) > 7f)
                    {
                        TriggerServerEvent("Server:ReturnSearch", local, request, GetPlayerName(PlayerId()), "", false);
                    }
                    else
                    {
                        if (!IsStringNullOrEmpty(clientInventory))
                        {
                            TriggerEvent("chat:addMessage", new
                            {
                                color = new[] { 255, 192, 41 },
                                args = new[] { "[SearchHandler]", $"You are being searched by {name}." }
                            });
                            TriggerServerEvent("Server:ReturnSearch", local, request, GetPlayerName(PlayerId()), clientInventory, true);
                        }
                        else
                        {
                            TriggerEvent("chat:addMessage", new
                            {
                                color = new[] { 255, 192, 41 },
                                args = new[] { "[SearchHandler]", $"You are being searched by {name}." }
                            });
                            TriggerServerEvent("Server:ReturnSearch", local, request, GetPlayerName(PlayerId()), "Nothing", true);
                        }
                    }
                }
            });

            EventHandlers["Client:ReturnSearch"] += new Action<int, int, string, string, bool>((local, request, name, contents, success) =>
            {
                if (request == GetPlayerServerId(PlayerId()))
                {
                    if (success == false)
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 192, 41 },
                            args = new[] { "[SearchHandler]", $"You are too far away from {name} to perform a search." }
                        });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 192, 41 },
                            args = new[] { "[SearchHandler]", $"You found {contents} on {name}." }
                        });
                    }
                }
            });

            RegisterCommand("setinv", new Action<int, List<object>, string>((source, args, raw) =>
            {
                
                if(IsStringNullOrEmpty(Convert.ToString(args[0])))
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 192, 41 },
                        args = new[] { "[SearchHandler]", $"Usage /setinv [inventory]." }
                    });
                }
                else
                {
                    var argshandler = args.ConvertAll(x => Convert.ToString(x));
                    clientInventory = string.Join("", argshandler);
                    Debug.WriteLine($"Inventory set to: {clientInventory}");
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 192, 41 },
                        args = new[] { "[SearchHandler]", $"Inventory set to {clientInventory}." }
                    });
                }
                
            }), false);

            RegisterCommand("search", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (IsStringNullOrEmpty(Convert.ToString(args[0])))
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 192, 41 },
                        args = new[] { "[SearchHandler]", $"Usage /search [playerID]." }
                    });
                }
                else
                {
                    var targetId = Convert.ToInt32(args[0]);
                    if (targetId == GetPlayerServerId(PlayerId()))
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 192, 41 },
                            args = new[] { "[SearchHandler]", $"You cannot search yourself." }
                        });
                    }
                    else
                    {
                        TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 192, 41 },
                            args = new[] { "[SearchHandler]", $"You are starting a search." }
                        });
                        TriggerServerEvent("Server:RequestSearch", targetId, GetPlayerServerId(PlayerId()), GetPlayerName(PlayerId()), GetEntityCoords(PlayerPedId(), true));
                    }
                }
            }), false);
        }

        
    }
}
