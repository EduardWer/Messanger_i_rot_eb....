using Messanger.MVVM.Model;
using Messanger.MVVM.Model.IO;
using Messanger.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Messanger.MVVM.ViewModel
{
    internal class HostVM : BindingsHelper
    {
        private ObservableCollection<string> usernames = new ObservableCollection<string>();

        private ObservableCollection<string> messages = new ObservableCollection<string>();
        public ObservableCollection<string> Messages
        {
            get { return messages; }
        }


        private Socket socket;
        private List<Socket> clients = new List<Socket>();

        public HostVM()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(1000);
            messages.Add($"[{DateTime.Now}] Server has started on {ipPoint.Address}!");

            ListenToClients();
        }

        private async Task ListenToClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
                ReceiveMessagesFromClient(client);
            }
        }

        private bool ValidateUserName(string userName)
        {
            bool isValid = true;
            foreach (string name in usernames)
            {
                if (name == userName || name == "Server" || name == "Disconnect")
                {
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        private async Task ReceiveMessagesFromClient(Socket client)
        {
            while (true)
            {
                var buffer = new byte[1024];
                int receivedBytes =  await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                MessageModel message = MessageManager.GetMessageModel( Encoding.UTF8.GetString(buffer));

                if (message.Username == "Server")
                {
                    if (ValidateUserName(message.MessageText))
                    {
                        messages.Add($"[{DateTime.Now}] {message.MessageText} has connected!");

                        usernames.Add(message.MessageText);

                        MessageModel usernamesMessage = new MessageModel() { Username = "UserList", MessageText = string.Join("|", usernames) };
                        foreach (var item in clients)
                        {
                            SendMessage(item, MessageManager.GenerateMessage(usernamesMessage));
                        }

                        message = new MessageModel() { Username = "Server", MessageText = $"{message.MessageText} has connected!" };

                    }
                    else
                    {
                        MessageModel ErrorMessage = new MessageModel() { Username = "NameError", MessageText = "Kill your self." };
                        messages.Add($"[{DateTime.Now}] Client {((IPEndPoint)client.RemoteEndPoint).Address} tried to connect. Error: Client with same username is already connected.");
                        SendMessage(client, MessageManager.GenerateMessage(ErrorMessage));
                        client.Close();
                    }
                }
                else if (message.Username == "Disconnect")
                {
                    if (usernames.Contains(message.MessageText))
                    {
                        usernames.Remove(message.MessageText);
                        clients.Remove(client);
                        client.Close();
                    }

                    MessageModel usernamesMessage = new MessageModel() { Username = "UserList", MessageText = string.Join("|", usernames) };
                    foreach (var item in clients)
                    {
                        SendMessage(item, MessageManager.GenerateMessage(usernamesMessage));
                    }
                    messages.Add($"[{DateTime.Now}] {message.MessageText} has disconnected!");
                    message = new MessageModel() { Username = "Server", MessageText = $"{message.MessageText} has disconnected!" };

                    client.Close();
                }
                else
                {
                    messages.Add($"[{DateTime.Now}] Received message from client {message.Username}: {message.MessageText}");
                }

                foreach (var item in clients)
                {
                    if(item != client)
                    {
                        SendMessage(item, MessageManager.GenerateMessage(message));
                    }
                }
            }
        }

        private async Task SendMessage(Socket client, string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            await client.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
        }
    }
}