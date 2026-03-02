using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace LogonServer
{
    public class server : WatsonTcpServer
    {
        private readonly Dictionary<string, string> _clientIpNames = new Dictionary<string, string>();

        public server(string listenerIp, int listenerPort) : base(listenerIp, listenerPort)
        {
            Console.WriteLine($"My address {listenerIp} my port {listenerPort}");

            this.Events.ClientConnected += ClientConnected;
            this.Events.ClientDisconnected += ClientDisconnected;
            this.Events.MessageReceived += MessageReceived;

            this.Start();
        }

        private async void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var ipPort = e.Client.IpPort;
            var message = Encoding.UTF8.GetString(e.Data);
            await ProccessMessage(ipPort, message);
        }

        private async Task ProccessMessage(string ipPort, string message)
        {
            var parts = message.Split('|');
            if (parts.Length < 2) return;

            string sendMessage = string.Empty;
            string command = parts[0];
            string payload = parts[1];

            switch (command)
            {
                case "Register":
                    _clientIpNames[ipPort] = payload;
                    sendMessage = $"{payload} registered";
                    break;
                
            }

            // Get sender name for the broadcast
            _clientIpNames.TryGetValue(ipPort, out string senderName);
            string finalMessage = $"{(senderName ?? ipPort)}: {sendMessage}";

            // ListClients now returns IEnumerable<ClientMetadata>
            foreach (var client in ListClients())
            {
                // Use client.Guid for SendAsync and client.IpPort for comparison
                if (client.IpPort != ipPort)
                {
                    try
                    {
                        // The primary SendAsync method now requires the client's GUID
                        await SendAsync(client.Guid, finalMessage);
                    }
                    catch
                    {
                        // Handle potential disconnects during broadcast
                    }
                }
            }
        }

        private void ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            _clientIpNames.Remove(e.Client.IpPort);
            Console.WriteLine($"[{e.Client.IpPort}] client disconnected");
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine($"[{e.Client.IpPort}] client connected");
        }
    }
}
