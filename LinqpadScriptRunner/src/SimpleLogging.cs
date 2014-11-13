using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;


namespace scriptserver
{
    public class SimpleLogging
    {

        static private string logfile = null;//"enable.log";

        public static bool klogging = true;

        private static bool firstTime = false;


        public static string formatLogHistroyString(string type, string id, string name, string field)
        {
            string formatstr = "";
            formatstr = formatstr + type + "(''";
            formatstr = formatstr + id + ":";
            formatstr = formatstr + name + "'')";
            formatstr = formatstr + "." + field;
            return formatstr;
        }

        public static void startKLog(string text)
        {
            if (logfile == null && klogging)
            {
                String lp = ConfigurationManager.AppSettings["logpath"];
                if (lp != null)
                {
                    logfile =
                    Path.Combine(ConfigurationManager.AppSettings["logpath"], "PimqlBic.log");


                    var ts = DoLogDateString().Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", "");


                    if (File.Exists(logfile))
                    {

                        var archiveFile = Path.Combine(lp, "zPimqlBic-" + ts + ".log");

                        try
                        {
                            File.Move(logfile, archiveFile);
                        }
                        catch (Exception e)
                        {
                            // If this fails, it's a good thing.  A file already exists
                            // with the same dattimestamp to the second.
                        }
                    }

                }
                else
                {
                    klogging = false;
                }
            }
            if (klogging)
            {
                try
                {
                    if (File.Exists(logfile))
                    {
                        File.AppendAllText(logfile, DoLogDateString() + text + "\n");
                    }
                    else
                    {
                        System.IO.File.WriteAllText(logfile, DoLogDateString() + text + "\n");
                    }
                }
                catch (Exception)
                {
                }
            }

        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void klog(string text)
        {

            if (klogging)
            {
                if (logfile == null)
                {
                    startKLog(text);
                }
                else
                {
                    try
                    {
                        System.IO.File.AppendAllText(logfile, DoLogDateString() + text + "\r\n");
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        public static void LogMessageSpecial(string message, string alternateLogFile)
        {
            var speciallogfile =
                Path.Combine(ConfigurationManager.AppSettings["logpath"], alternateLogFile);

            try
            {

                System.IO.File.AppendAllText(speciallogfile, DoLogDateString() + message + "\r\n");
            }
            catch (Exception e)
            {
            }

        }

        public static string DoLogDateString()
        {
            return String.Format(String.Format("{0:u}", DateTime.Now)) + " ";


        }

        public static void LogMessage(string message)
        {
            if (firstTime)
            {
                startKLog(message);
                firstTime = false;
            }
            else
            {
                klog(message);
            }
        }

        public static void LogMessageOneUp(string message)
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(2);

            message = @"File:" + frame.GetFileName() + " Method:" + frame.GetMethod().Name + " line:" + frame.GetFileLineNumber() + " -- " + message;
            if (firstTime)
            {
                startKLog(message);
                firstTime = false;
            }
            else
            {
                klog(message);
            }
        }
    }
}
