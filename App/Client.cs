using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace App
{
    class Client : Point
    {
        protected int id { get; set; }

        public event MethodContainer onTrace;
        
        public void init(int new_port, string new_host)
        {

            port = new_port; // назначаем порт
            host = IPAddress.Parse(new_host);
        }


        public Client(int i)
        {
            id = i;
        }

        public void Start()
        {
            try
            {
                Random r = new Random(unchecked((int)(DateTime.Now.Ticks)));
                if (running)
                {
                    return;
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(host, port);

                running = true;

                Thread th = new Thread(delegate () //создаем новый поток
                {
                    int counter = 0;

                    byte[] intBytes = BitConverter.GetBytes(id);
                    SendBytes(intBytes);

                    while (true)
                    {
                        var now = DateTime.Now;
                        counter++;

                        // Посылаем сообщение
                        onTrace?.Invoke(String.Format("[Клиент #{0}] отправлено серверу: {1}, {2}", id, counter, now));

                        long utcNowAsLong = now.ToBinary();
                        byte[] utcNowBytes = BitConverter.GetBytes(utcNowAsLong);

                        intBytes = BitConverter.GetBytes(counter);
                        
                        SendBytes(intBytes);
                        SendBytes( utcNowBytes);

                        // Ждем ответ                     
                        onTrace?.Invoke(String.Format("[Клиент #{0}] получено от сервера: {1}", id, Encoding.Default.GetString(ReceiveBytes())));
                        
                        // Засыпаем на период до 10с
                        int time_sleep = r.Next(0, 10000);
                        onTrace?.Invoke(String.Format("[Клиент #{0}] заснул на {1} миллисекунд", id, time_sleep));
                         
                        Thread.Sleep( time_sleep );
                    }
                });

                th.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка! Не удается подключиться к серверу.");
                return;
            }
        }

        
    }
}
