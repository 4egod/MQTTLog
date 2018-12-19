using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MQTTLog
{
    public static class Logger
    {
        public static void Start()
        {
            string directory = Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            directory = directory.Substring(0, directory.LastIndexOf(@"\") + 1) + @"Logs\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Debug.Listeners.Add(new ConsoleTraceListener(true));

            string filePath = string.Format(@"{0}{1}.log", directory, DateTime.Now.ToString("yy.MM.dd-HH.mm"));
            Debug.Listeners.Add(new TextWriterTraceListener(filePath));
            Debug.AutoFlush = true;
            Debug.Print("Initialize Logging...");
        }
    }
}
