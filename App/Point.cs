using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace App
{
    class Point
    {
        protected bool running = false;

        protected IPAddress host;
        protected int port;

        protected Socket socket;
        protected IPEndPoint point;
        
        public List<Thread> threads = new List<Thread>();

        public delegate void MethodContainer(string message);


        public void ClearThreads()
        {
            for (int i = 0; i < threads.Count(); i++)
            {
                if (threads[i].ThreadState == ThreadState.Running)
                {
                    threads[i].Abort();
                    threads.Remove(threads[i]);
                }
            }
        }

        public void Exit()
        {
            if (running)
            {
                socket.Close();
            } //если порт был привязан, то прекращаем слушание

            running = false;

            ClearThreads();
        }


        protected byte[] Receive(Socket client) //функция получения байтов
        {
            byte[] bytes = new byte[1024];

            int bytesRec = client.Receive(bytes);

            return bytes.Take(bytesRec).ToArray(); ; //возвращаем байты из функции   
        }

        protected void SendBytes(Socket client, byte[] bytes) //функция отправки байтов
        {
            client.Send(bytes); //отправляем            
        }


        protected byte[] ReceiveBytes() //функция получения байтов
        {
            byte[] bytes = new byte[1024];

            int bytesRec = socket.Receive(bytes);

            return bytes.Take(bytesRec).ToArray() ; ; //возвращаем байты из функции   
        }

        protected void SendBytes(byte[] bytes) //функция отправки байтов
        {
            socket.Send(bytes); //отправляем            
        }
    }
}
