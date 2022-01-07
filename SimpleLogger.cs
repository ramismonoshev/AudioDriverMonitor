using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MAudioDriverMonitor
{
    class SimpleLogger : IDisposable
    {

        internal const String DEFAULT_LOG_FILENAME = "MAudioDriverMonitor.log";


        private static SimpleLogger instance = null;

        private String logFilename;

        private TextWriter logFile;


        private bool initialized;


        internal static SimpleLogger Instance(String logFilename = null)
        {
            if (logFilename == null)
            {
                logFilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SimpleLogger.DEFAULT_LOG_FILENAME);
            }
            if (SimpleLogger.instance == null)
            {
                SimpleLogger.instance = new SimpleLogger(logFilename);
            }
            return SimpleLogger.instance;
        }


        private SimpleLogger(String logFilename)
        {
            this.logFilename = logFilename;
            try
            {
                logFile = new StreamWriter(logFilename, true, Encoding.UTF8);
                initialized = true;
            }
            catch (IOException)
            {
                MessageBox.Show("Log file '" + logFilename + "' already in use !");
                initialized = false;
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing && initialized)
            {
                logFile.Close();
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void WriteLine(String message)
        {
            if (initialized)
            {
                logFile.WriteLine(DateTime.Now + " - " + message);
                logFile.Flush();
            }
        }


        internal void Write(String message)
        {
            if (initialized)
            {
                logFile.Write(DateTime.Now + " - " + message);
                logFile.Flush();
            }
        }
    }
}
