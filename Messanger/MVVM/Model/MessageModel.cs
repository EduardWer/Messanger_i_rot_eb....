using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messanger.MVVM.Model
{
    internal class MessageModel
    {
        public string Username { get; set; }
        public string MessageText { get; set; }
        public bool IsOwn { get; set; }
    }
}
