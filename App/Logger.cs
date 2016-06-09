using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Logger
    {
        public int client_id { get; set; }

        public List<string> logs = new List<string>();

        StreamWriter writer;

        public Logger(int id)
        {
            client_id = id;
            writer = new StreamWriter(client_id.ToString(),true);
        }

        // timer - раз в минуту всё выгружаем в лог пользователя
        public void flush(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            
            lock (logs)
            {
                foreach (var row in logs)
                    writer.WriteLine(row);
            }

            writer.Flush();

            logs.Clear();

            autoEvent.Set();

        }
    }
}
