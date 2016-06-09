using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    // Обычный эмулятор. С произвольным количеством клиентов и одним сервером
    public partial class Form1 : Form
    {
        Server server = new Server();
        Client[] clients = new Client[10];

        public Form1()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit; //указываем, что при закрытии программы будет вызываться ивент
            
            for(var i=0; i<clients.Length; i++)
            {
                clients[i] = new Client( i );
                clients[i].onTrace += trace;
            }
        }
        
        void Application_ApplicationExit(object sender, EventArgs e) //ивент закрытия программы
        {
            server.Exit();
            // client.Exit();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;

            server.init(11000, "127.0.0.1");            
            server.Start();

            Thread.Sleep(1000);
            
            foreach (var client in clients)
            {
                client.init(11000, "127.0.0.1");
                client.Start();
            }
        }

        public void trace(string message)
        {
            try
            {
                richTextBox_Logger.Invoke(new Action(() => {
                    richTextBox_Logger.Text += "\n" + message;

                    richTextBox_Logger.SelectionStart = richTextBox_Logger.Text.Length;
                    richTextBox_Logger.ScrollToCaret();
                }));
            }
            catch
            {
                // pass
            }
        }
    }
}
