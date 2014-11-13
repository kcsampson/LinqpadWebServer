using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace scriptserver
{
    public class ScriptRunner
    {
        public static void CompileScript(string scriptPath, ref IDisposable compiled)
        {
            SimpleLogging.LogMessage("scriptserver compiling linkpad script: " + scriptPath);
            try
            {
                var dbCtx = LINQPad.Util.Compile(scriptPath, false);

                compiled = dbCtx;

                
            }
            catch (Exception e)
            {
                SimpleExceptionHandling.HandleException(e);
            }

        }



        public static object ReRunScript(object[] args, ScriptCacheItem script)
        {

           var result = (script.CompiledScript as LINQPad.ObjectModel.QueryCompilation).Run(LINQPad.QueryResultFormat.Text, args);
           if (result.Exception != null)
           {
               SimpleLogging.LogMessage(script.ScriptFile + " run returned with exception.");
               SimpleExceptionHandling.HandleException(result.Exception);
               return result.Exception;
           }
           else
           {
               return result.ReturnValue;
           }

        }
    }


    public class ScriptCacheItem
    {
        public string ScriptFile { get; set; }

        public DateTime stamp = DateTime.Now;

        public IDisposable CompiledScript { get; set; }

        public DateTime ScriptDate { get { return stamp; } }

    }
    
    
}
