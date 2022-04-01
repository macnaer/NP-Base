using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class PrintStringEventArgs : EventArgs
    {
        public string MessageToPrint { get; set; }

        public PrintStringEventArgs(string _message) => MessageToPrint = _message;

    }
}
