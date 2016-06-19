
namespace App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Net.Sockets;
    using System.Net;
    using log4net;
    using System.Reflection;

    class Server:Point
    {
        public event MethodContainer onTrace;

        List<Socket> clients = new List<Socket>(); // клиент

        protected ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Server()
        {
        }

        public void init(int new_port, string new_host)
        {
            port = new_port; // назначаем порт
            host = IPAddress.Parse(new_host);
        }

        public void Start()
        {
            if (!running)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                point = new IPEndPoint(IPAddress.Any, port);

                socket.Bind(point); // связываем сокет с точкой
                socket.Listen(300); // начинаем прослушивать, макс. 300
                
                running = true;
                
                AcceptSocket(); // начинаем принимать сокеты
            }
            else
            {
                running = false;
                socket.Close();

                ClearThreads();
            }
        }

        
        // функция прослушки юзеров
        private void AcceptSocket()
        {
            Thread th = new Thread(delegate () //создаем новый поток для прослушки
            {
                try
                {

                    while (running) // делаем, пока сервер запущен
                    {
                        Socket temp = socket.Accept();// Появилось новое соединение

                        Thread subth = new Thread(delegate () //создаем новый поток для прослушки
                        {
                            clients.Add(temp);

                            try
                            {
                                AutoResetEvent autoEvent = new AutoResetEvent(false);
                                
                                var intBytes = Receive(temp);

                                int client_id = BitConverter.ToInt32(intBytes, 0);

                                // коллекционируем лог, а потом - флушим
                                //Logger current_logger = new Logger(client_id);

                                //TimerCallback tcb = current_logger.flush;

                                //Timer timer = new Timer(tcb, autoEvent, 0, 10000);
                                
                                // Пока есть соединение - ловим биты-байты
                                while (temp.Connected)
                                {
                                    // Прочитаем номер
                                    intBytes = Receive(temp);

                                    if (!BitConverter.IsLittleEndian)
                                        Array.Reverse(intBytes);

                                    int number = BitConverter.ToInt32(intBytes, 0);

                                    // Прочитаем дату
                                    long dateLongBack = BitConverter.ToInt32(Receive(temp), 0);
                                    
                                    DateTime date = DateTime.FromBinary(dateLongBack);

                                    Logger.For( this ).Info( String.Format("{0}, {1}", number, date) );

                                    //current_logger.logs.Add(String.Format("{0}, {1}", number, date));

                                    onTrace?.Invoke(String.Format("[Сервер] Получено: {0}, {1}", number, date));

                                    // Отправим сообщение, что всё нормально
                                    var message = Encoding.Default.GetBytes("[Сервер] Всё нормально");
                                    SendBytes(temp, message);
                                }

                                //timer.Dispose();
                            } catch(Exception error)
                            {
                                onTrace?.Invoke(String.Format("[Сервер] ошибка: {0}", error.Message));
                            }
                        });

                        subth.Start(); // запускаем этот поток

                        threads.Add(subth);
                    }
                } catch(Exception error)
                {
                    Logger.For(this).Error( error.Message );

                    onTrace?.Invoke(String.Format("[Сервер] ошибка: {0}", error.Message));
                }
            });

            th.Start(); // запускаем весь предыдущий поток
            threads.Add(th); // Добавляем поток в список потоков
        }


    }
}
