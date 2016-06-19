using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace App
{
    class Logger
    {
        /*
        public List<string> logs = new List<string>();

        public int maxSize = 256;

        StreamWriter writer;

        public Logger(int id)
        {
            writer = new StreamWriter(id.ToString(),true);
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

        }*/

        static Logger()
        {
            XmlConfigurator.Configure();
        }

        public static ILog For(object LoggedObject)
        {
            if (LoggedObject != null)
                return For(LoggedObject.GetType());
            else
                return For(null);
        }

        public static ILog For(Type ObjectType)
        {
            if (ObjectType != null)
                return LogManager.GetLogger(ObjectType.Name);
            else
                return LogManager.GetLogger(string.Empty);
        }
    }
}
