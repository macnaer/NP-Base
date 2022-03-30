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

        private void Form1_Load(object sender, EventArgs e)
        {
            if(_chatClient == null)
            {
                // local port ... remote port
                _chatClient = new ChatClient(8080, 1234);
            }

            _chatClient.Send("Hello from client");
        }
    }
}
