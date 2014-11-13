using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web;
using System.ComponentModel;

using scriptserver;

namespace scriptserver
{


    /// <summary>
    /// Short Term Job Queue.  This class manages kicking off BackgroundWorkers.
    /// These are jobs that are short term.  They may never complete if server is restarted, etc.
    /// The are meant to be user GUI events that could take a minute or so to process.
    /// 
    /// Once a job is started, it leaves the queue and the Application no longer tracks it.
    /// You should also put the job in your session and manage it's life cycle.  You simply can't start it.
    /// 
    /// Globals.asax starts off the process that kicks jobs off.
    /// </summary>
    public class ShortTermJobQueue : BackgroundWorker
    {
        ConcurrentQueue<SessionJob> jobQueue = new ConcurrentQueue<SessionJob>();

        private static ShortTermJobQueue instance = null;

        public static void AddJob(BackgroundWorker worker, HttpSessionState Session)
        {

            if (!Instance().IsBusy)
            {
                SimpleLogging.LogMessage("Warning! ShortTermJobQue = Adding Job to stopped Job Queue");
            }

            Instance().jobQueue.Enqueue(new SessionJob()
            {
                SessionId = Session.SessionID,
                Worker = worker
            });

        }


        public static ShortTermJobQueue Instance()
        {
            if (instance == null)
            {
                instance = new ShortTermJobQueue();
            }
            else
            {
                if (!instance.IsBusy)
                {
                    SimpleLogging.LogMessage("Warning!  Call to ShortTermJobQueue.Instance() returning a stopped background worker");
                }
            }
            return instance;
        }


        public static void Shutdown()
        {
            Instance().CancelAsync();
            SimpleLogging.LogMessage("ShortTermJobQueue shutting down..");
            instance = null;
        }


        private ShortTermJobQueue()
        {

            WorkerSupportsCancellation = true;

            this.DoWork += new DoWorkEventHandler(ShortTermJobQueue_DoWork);
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ShortTermJobQueue_RunWorkerCompleted);
        }

        void ShortTermJobQueue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SimpleLogging.LogMessage("Notice!  ShortTermJobQueue_RunWorkerCompleted");
        }

        void ShortTermJobQueue_DoWork(object sender, DoWorkEventArgs e)
        {
            SimpleLogging.LogMessage("ShortTermJobQueue starting......");
            try
            {
                while (!this.CancellationPending)
                {
                    StartJobs();
                    System.Threading.Thread.Sleep(500);  // Runs every 1/2.
                }
            }
            catch (Exception stjqe)
            {
                SimpleExceptionHandling.HandleException(stjqe, "Background");
                throw stjqe;
            }
        }



        void StartJobs()
        {

            if (jobQueue != null)
            {

                while (jobQueue.Count() > 0 && !this.CancellationPending)
                {

                    SessionJob job = null;
                    jobQueue.TryDequeue(out job);

                    if (job != null)
                    {

                        SimpleLogging.LogMessage("ShortTermJobQueue starting a:" + job.Worker.GetType().FullName);

                        job.Worker.RunWorkerAsync();
                    }

                }

            }



        }


    }


    public class SessionJob
    {
        public string SessionId { get; set; }

        public BackgroundWorker Worker { get; set; }

        public SessionJob() { }

    }

}