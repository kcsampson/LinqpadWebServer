using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.SessionState;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;



/// <summary>
/// Summary description for DamDeleteBackgroundWorker
/// </summary>

namespace scriptserver
{
    public class ScriptDisposeBackgroundWorker : BackgroundWorker
    {
        public int RequestingThreadId { get; set; }
        public ScriptCacheItem CachedScript { get; set; }

        public ScriptDisposeBackgroundWorker(ScriptCacheItem cachedScript)
            : base()
        {
            RequestingThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            this.CachedScript = cachedScript;
            this.DoWork += new DoWorkEventHandler(ScriptDisposeBackgroundWorker_DoWork);
        }

        void ScriptDisposeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            
            // Going to wait 4 second to let all other threads finish running the script.
            System.Threading.Thread.Sleep(4000);


            if (ConfigurationManager.AppSettings["debug"] != null && ConfigurationManager.AppSettings["debug"] == "true")
            {
                SimpleLogging.LogMessage("ScriptDisposeBackgroundWorker disposing: " + CachedScript.ScriptFile + " requesting thread="+this.RequestingThreadId.ToString());
            }

            try
            {

                CachedScript.CompiledScript.Dispose();

                CachedScript.CompiledScript = null;
            }
            catch (System.Threading.AbandonedMutexException ex)
            {
                // Oh well.....  Just eat it.
                SimpleLogging.LogMessage("Exception displosing script.");
                SimpleExceptionHandling.HandleException(ex);

            }

        }




    }
}
