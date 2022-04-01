using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatClientForm : Form
    {
        ChatClient _chatClient;
        public ChatClientForm()
        {
            InitializeComponent();
        }

        private void btnSendBroadcast_Click(object sender, EventArgs e)
        {
            if (_chatClient == null)
            {
                int.TryParse(tbLocalPort.Text, out var nLocalPort);
                int.TryParse(tbRemotePort.Text, out var nRemotePort);

                _chatClient =
                    new ChatClient
                    (nLocalPort, nRemotePort);

            }
            _chatClient.Send(tbBroadcastText.Text);
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            _chatClient.SendMessageToKnownServer(tbMessage.Text);
        }

        private void chatClient_PrintString(object sender, PrintStringEventArgs e)
        {
            Action<string> print = PrintToTextBox;

            tbConsole.Invoke(print, new string[] { e.MessageToPrint });
            //tbConsole.Text += $"{Environment.NewLine}{DateTime.Now} - {e.MessageToPrint}";
        }

        private void PrintToTextBox(string stringToPrint)
        {
            tbConsole.Text += $"{Environment.NewLine}{DateTime.Now} - {stringToPrint}";
        }
    }
}
