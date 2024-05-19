using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messanger.MVVM.Model.IO
{
    static class MessageManager
    {
        public static MessageModel GetMessageModel(string message)
        {
            string[] messageSplited = message.Split('~');
            return new MessageModel() {Username = messageSplited[0], MessageText = messageSplited[1]};
        }

        public static string GenerateMessage(MessageModel messageModel)
        {
            return messageModel.Username + "~" + messageModel.MessageText + "~";
        }
    }
}
