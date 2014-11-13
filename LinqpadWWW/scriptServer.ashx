<%@ WebHandler Language="C#" Class="scriptServer" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;


using scriptserver;


public class scriptServer : IHttpHandler, IRequiresSessionState {
    
    [MethodImpl( MethodImplOptions.Synchronized )] 
    public static ScriptCacheItem AcquireCompiledScript( HttpContext context, string scriptFile)
    {
        ScriptCacheItem script = null;
      // scriptServer.ScriptRunner.CompileScript(scriptFile, ref compiledScript);
        IDisposable compiled = null;

       if (context.Application[scriptFile] != null)
       {
           var cachedScript = (ScriptCacheItem)context.Application[scriptFile];

           if (new FileInfo(scriptFile).LastWriteTime <= cachedScript.ScriptDate)
           {
               script = cachedScript;
           }
           else
           {
               context.Application.Remove(scriptFile);

               ShortTermJobQueue.AddJob(new ScriptDisposeBackgroundWorker(cachedScript), context.Session);
               
                               
           
               scriptserver.ScriptRunner.CompileScript(scriptFile, ref compiled);

               script = new ScriptCacheItem() { CompiledScript = compiled, ScriptFile = scriptFile };
               context.Application[scriptFile] = script;
           }


       }
       else
       {
           
           scriptserver.ScriptRunner.CompileScript(scriptFile, ref compiled);
           script = new ScriptCacheItem() { CompiledScript = compiled , ScriptFile = scriptFile};
           context.Application[scriptFile] = script;

       }

       return script;
        
    }
    

    public void ProcessRequest (HttpContext context) {
        try
        {
            context.Response.ContentType = "application/json";

            var script = context.Request.QueryString.Get("script");




            if (string.IsNullOrEmpty(script))
            {
                context.Response.Write("{ error: 'no script'}");
                return;
            }
            var scriptFile = FindScript(script);
            if (string.IsNullOrEmpty(scriptFile))
            {
                context.Response.Write("{ error: 'script not found'}");
                return;
            }

            #region Run the script via Linqpad's Util.compile/Util.run
            var inProps = new Dictionary<string, string>();
            inProps = context.Request.QueryString.AllKeys
                        .ToList()
                        .ToDictionary(a => a, a => context.Request.QueryString.Get(a));

            context.Request.Headers.AllKeys
                .ToList()
                .ForEach(hk => inProps[hk] = context.Request.Headers.Get(hk));



            if (context.Request.RequestType.ToLower() == "post")
            {

                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    // This will equal to "charset = UTF-8 & param1 = val1 & param2 = val2 & param3 = val3 & param4 = val4"
                    string values = reader.ReadToEnd();

                    inProps["HTTP_POST_DATA"] = values;
                }

            }


            ScriptCacheItem scriptCached = null;

            bool acqsuccess = false;
            while (!acqsuccess)
            {
                try
                {
                    scriptCached = AcquireCompiledScript(context, scriptFile);
                    acqsuccess = true;
                }
                catch (System.Threading.AbandonedMutexException e)
                {
                    SimpleExceptionHandling.HandleException(e);
                    System.Threading.Thread.Sleep(100);
                    // Probably some script became dirty in the middle of chaos...
                }
            }
                



            object resultObj = null;

            int attemps = 5;
            bool success = false;

            while (--attemps > 0 && !success)
            {

                try
                {
                    resultObj = scriptserver.ScriptRunner.ReRunScript(new object[] { inProps }, scriptCached);
                    
                    success = true;
                }
                catch (Exception e)
                {
                    if (scriptCached == null)
                    {
                        SimpleLogging.LogMessage(" Rerun (null) threw unhandled exception.  Will try again after 100 millis");
                    }
                    else if (scriptCached.ScriptFile == null)
                    {
                        SimpleLogging.LogMessage(" Rerun script (null) threw unhandled exception.  Will try again after 100 millis");
                    }
                    else
                    {
                        SimpleLogging.LogMessage(scriptCached.ScriptFile + " Rerun script threw unhandled exception.  Will try again after 100 millis");
                    }
                    SimpleExceptionHandling.HandleException(e);
                    System.Threading.Thread.Sleep(100);
                        
                }
            }






            #endregion

            var sWorker = new System.Web.Script.Serialization.JavaScriptSerializer();
            if (resultObj is Dictionary<string, string>)
            {
                var outProps = resultObj as Dictionary<string, string>;

                if (outProps.ContainsKey("ResultJSON"))
                {

                    context.Response.Write(outProps["ResultJSON"]);
                }


            }
            else
            {

                context.Response.Write(sWorker.Serialize(resultObj));
            }


        }
        catch (Exception e)
        {
            SimpleLogging.LogMessage("scriptServer.ashx general Exception ");
            SimpleExceptionHandling.HandleException(e);

            context.Response.Write("{  error : '" + e.Message + "' }");
        }
    }




 
    public bool IsReusable {
        get {
            return true;
        }
    }


    public static string FindScript(string scriptName)
    {

        var scriptFile = "";
        foreach (var scriptPathOuter in ConfigurationManager.AppSettings["scriptPath"].Split(','))
        {

            var scriptPaths = new List<string>() { scriptPathOuter };

            scriptPaths.AddRange(Directory.GetDirectories(scriptPathOuter, "*.*", SearchOption.AllDirectories));


            
            foreach (var scriptPath in scriptPaths)
            {
                scriptFile = Path.Combine(scriptPath.Trim(), scriptName) + ".linq";
                if (File.Exists(scriptFile))
                {

                    goto doneLoop;
                }

            }

        }
       

    doneLoop:


        return scriptFile;



    }

}