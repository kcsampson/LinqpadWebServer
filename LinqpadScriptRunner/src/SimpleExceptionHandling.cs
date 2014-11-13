using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace scriptserver {
    public class SimpleExceptionHandling
    {



    
        public static void HandleException(Exception ex, string handlerPolicy = "unused" )
        {

            SimpleLogging.LogMessage("Exception type=" + ex.GetType().FullName);
            SimpleLogging.LogMessage("Excption stacktrace: " +ex.StackTrace + "\r\nEx Message:" + ex.Message );
            
        }
    }
}
