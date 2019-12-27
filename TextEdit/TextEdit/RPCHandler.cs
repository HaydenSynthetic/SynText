using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace TextEdit
{
    static class RPCHandler
    {
        static public DiscordRpcClient client;

        static public void Awaken()
        {
            client = new DiscordRpcClient("659614526022615041");
            client.Initialize();
        }

        static public void SetPresence(RichPresence presence)
        {
            if (client.IsInitialized)
            client.SetPresence(presence);
        }
    }
}
