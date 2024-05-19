using Messanger.MVVM.Model;
using Messanger.MVVM.Model.IO;
using Messanger.MVVM.View;
using Messanger.MVVM.ViewModel;
using Messanger.View;
using Messanger.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messanger.ViewModel
{
    internal class ChatVM : BindingsHelper
    {
        #region Свойства

        private string hostname;

        private Window _window;
        public Window Window
        {
            get { return _window; }
            set { _window = value; _window.Closed += OnWindowClosed; }
        }

        public string username;

        public string OnlineUsers { get; set; }

        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> Messages
        {
            get { return messages; }
        }

        private ObservableCollection<string> usernames = new ObservableCollection<string>();
        public ObservableCollection<string> Usernames
        {
            get { return usernames; }
        }

        private string ConnectionInfo;
        public string connectionInfo
        {
            get 
            {   
                if(ConnectionInfo == null)
                {
                    return "Connect"; 
                }
                else
                {
                    return ConnectionInfo;
                }
            }
            set
            {
                ConnectionInfo = value;
                OnPropertyChanged();
            }
        }

        private Socket _server;
        
        #endregion

        #region Комманды
        public BindableCommand OpenConnectDialogCommand { get; set; }
        
        public BindableCommand SendMessageCommand { get; set; }

        public BindableCommand OpenMainMenuCommand { get; set; }
        #endregion
        public ChatVM(string name) 
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            username = name;
            OpenConnectDialogCommand = new BindableCommand(_ => OpenConectDialog());
            SendMessageCommand = new BindableCommand(_ => { SendMessage(username, _messageText); _messageText = ""; });
            OpenMainMenuCommand = new BindableCommand(_ => { OpenMainMenu(); Usernames.Clear(); });
        }

        private async Task ReceiveMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                await _server.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                MessageModel message = MessageManager.GetMessageModel( Encoding.UTF8.GetString(buffer));

                if(message.Username == "UserList")
                {
                    string[] strings = message.MessageText.Split('|');
                    usernames.Clear();

                    foreach (string s in strings)
                    {
                        usernames.Add(s);
                    }
                    OnlineUsers = usernames.Count + " ";

                    connectionInfo = "Connected to " + hostname;
                }
                else if (message.Username == "NameError")
                {
                    MessageBox.Show("User with same login is already connected.\nChoose another login.\n");
                }
                else
                {
                    message.Username = message.Username + " " + DateTime.Now;
                    messages.Add(message);
                }
            }
        }

        private async Task SendMessage(string username, string msg)
        {
            MessageModel messageModel = new MessageModel() {Username = username, MessageText = msg };
            byte[] bytes = Encoding.UTF8.GetBytes(MessageManager.GenerateMessage(messageModel));
            try
            {
                await _server.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

            } catch { }

            switch (username)
            {
                case "Server":
                    break;
                case "Disconnect":
                    messages.Clear();
                    break;
                default:
                    messageModel.Username = "You " + DateTime.Now;
                    messageModel.IsOwn = true;
                    messages.Add(messageModel);
                    break;
            }
        }

        void OpenConectDialog()
        {
            if (connectionInfo == "Connect")
            {
                var dialog = new ConnectDialogWindow();
                var result = dialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    var dialogResult = dialog.DataContext as ConnectDialogWindowVM;
                    hostname = dialogResult.Text;

                    try
                    {
                        _server.Connect(hostname, 8888);
                        SendMessage("Server", username);
                        ReceiveMessages();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Connection Error: invalid IP!");
                    }

                }
            }
            else
            {
                SendMessage("Disconnect", username);
                connectionInfo = "Connect";
            }
           
        }

        private void OnWindowClosed(Object sender, EventArgs e)
        {
            // Обработка закрытия окна
            SendMessage("Disconnect", username);
        }

        private void OpenMainMenu()
        {
            SendMessage("Disconnect", username);
            new MainWindow().Show();
            Window.Close(); 
        }
    }
}
