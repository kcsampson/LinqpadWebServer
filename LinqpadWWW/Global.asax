<%@ Application Language="C#" %>
 <%@ Import Namespace="System.Collections.Generic" %>
 <%@ Import Namespace="System.Linq" %>
 <%@ Import Namespace="scriptserver" %>
 

<script runat="server">
    
    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

        // Get the exception object.
        Exception exc = Server.GetLastError();

        if (exc is HttpException)
        {
            return;
        }

        SimpleLogging.LogMessage(">>>>> BEGIN ---------------------------   Uncaught Error ------------------->>>>");
        SimpleExceptionHandling.HandleException(exc, "APP");
        SimpleLogging.LogMessage("<<<<< END -----------------------------   Uncaught Error -------------------<<<<");

      //  MailError.SendErrorMail(exc.ToString());
    }

    void Application_Start(object sender, EventArgs e) 
    {

        ShortTermJobQueue.Instance().RunWorkerAsync();  // Only starts up if no errors.


    }

    void Application_End(object sender, EventArgs e) 
    {
        ShortTermJobQueue.Shutdown();

    }
        
/*
    void Session_Start(object sender, EventArgs e) 
    {
       

    }

    void Session_End(object sender, EventArgs e) 
    {


    }
 */
       
</script>
