<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Namespace>System.Configuration</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

#region KIMBONICS_IGNORE
void Main()
{
	// Write code to test your extensions here. Press F5 to compile and run.
	"Extensions compiles".Dump();
}
#endregion

public static class MyExtensions
{

	#region Simple ConfigurationManager.AppSettings 
	
	public static string Conf(this string appSettingName ){
	   return ConfigurationManager.AppSettings[appSettingName];
	}

	
	public static bool Configured(this string appSettingName, string appSettingValue ){
	
	   return ( ConfigurationManager.AppSettings[appSettingName] != null
	      && ConfigurationManager.AppSettings[appSettingName] == appSettingValue );
		  
	}
	
	// See of setting is true/yes/1/TRUE/YES etc.
	public static bool Configured( this string appSettingName ){
	
	   
	   var v = ConfigurationManager.AppSettings[appSettingName];
	   if ( string.IsNullOrEmpty( v ) ){
	   		return false;
	   }
		
	   return (new string[]{ "yes","true","1"}).Contains( v.ToLower() );
	}
	
	
	#endregion
  #region Generic Dicts Safe
   public static Tup Get<KeyTup,Tup>( this Dictionary<KeyTup,Tup> dict, KeyTup key ){
      
	  return Get<KeyTup,Tup>( dict, key, default(Tup) );
	}
	
	
	
	
	public static Tup GetCustomAttribute<Tup>( this Type t ){
	
		var ret = t.GetCustomAttributes( typeof(Tup), false );
		if ( ret != null && ret.Length > 0 ){
		   return (Tup)ret[0];
		}
		return default(Tup);
	}	
	
	   // Safe Dictionary Get with Default value when key isn't there..
   public static Tup Get<KeyTup, Tup>( this Dictionary<KeyTup,Tup> dict, KeyTup key, Tup defaultValue ){
   
      if( EqualityComparer<KeyTup>.Default.Equals(key, default(KeyTup)) ){

	        return defaultValue;
		
	  }
      if ( dict.ContainsKey( key ) ){
	     return dict[key];
	  } else {
	     return defaultValue;
	  }
      
   
   }
	#endregion
	
	#region StringBuilder shorthand
	public static StringBuilder A(this StringBuilder builder, string x ){
	   return builder.Append( x );
	}
	
	public static StringBuilder AL(this StringBuilder builder, string x ){
	   return builder.AppendLine( x );
	}
	
	
	#endregion
	
		#region String Split by single String *removes empties
	static public string[] Split( this string s, string splitBy ){
	
		return s.Split( new String[] { splitBy },StringSplitOptions.RemoveEmptyEntries );
	
	}
	
	
	#endregion
	
	
		
#region Make an HTTP Call to get a json string
 
 // Content type is css or json, etc.
public static string GetJSONHttp(this string httpReqeust, string contentType ){


StringBuilder sb = new StringBuilder();

string jsonData = null;

 // used on each read operation 
 byte[] buf = new byte[8192];

 // prepare the web page we will be asking for 
 HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(httpReqeust));
 request.Method = "GET";

 string timeout = ConfigurationManager.AppSettings["epim.service.TimeOut"].ToString();
 int iTo = int.Parse(timeout);
 // execute the request 
 request.ReadWriteTimeout = iTo;
 request.Timeout = iTo;

 request.ContentType = "text/"+contentType+";charset=\"utf-8\"";
 request.Accept = "text/"+contentType;

 HttpWebResponse response = (HttpWebResponse)request.GetResponse();

 // we will read data via the response stream 
 Stream resStream = response.GetResponseStream();

 string tempString = null;
 int count = 0;

 do
 {
     // fill the buffer with data 
     count = resStream.Read(buf, 0, buf.Length);

     // make sure we read some data 
     if (count != 0)
     {
         // translate from bytes to ASCII text 
         tempString = Encoding.ASCII.GetString(buf, 0, count);

         // continue building the string 
         sb.A(tempString);
     }
 } while (count > 0);

 jsonData = ((sb.ToString()));
   
   // Now as a hack, there may be comments in the java script like // BEGIN_PIMQL_IGNORE  ..... //  END_PIMQL_IGNORE
   // Delete all the lines....
   
   if ( jsonData.Contains("BEGIN_PIMQL_IGNORE") || jsonData.Contains("@import(") ){
    "BEGIN_PIQML_IGNORE".Dump();
	StringBuilder filtered = new StringBuilder();
	var reader = new StringReader( jsonData );
	bool hit = false;
	string line= null;
	while ( (line=reader.ReadLine() ) != null ){
	
	    if ( line.Contains("@import(") && contentType == "css" ){
		   continue;  // Assumption is you need to get @imports pulled in yourself.
		}
		if ( line.Contains("END_PIMQL_IGNORE")){
		    hit = false;
		
		}

		if ( hit ){
		   filtered.AL( "//" + line );
		} else {
		   filtered.AL( line );
		}
		 if ( line.Contains( "BEGIN_PIMQL_IGNORE") ){
	      hit = true;

		}

	}
	jsonData = filtered.ToString();
   
   }
   
   #region PIMQL_IGNORE
  // jsonData.Dump();
   #endregion

return jsonData;
      
}
 
 
 #endregion
 

	
}