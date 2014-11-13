<Query Kind="Program">
  <Connection>
    <ID>6e3b1050-4b49-4c06-b64a-161ff8dfed19</ID>
    <Persist>true</Persist>
    <Driver>LinqToSql</Driver>
    <Server>kimballsam\KQL</Server>
    <CustomAssemblyPath>C:\Enterworks\PIMQL_Creative\pimqlbic_CreativeLinqpadCtx_0_0.dll</CustomAssemblyPath>
    <CustomTypeName>enablelib.pimqlbic.inmemory.PimqlBicCtx</CustomTypeName>
    <SqlSecurity>true</SqlSecurity>
    <Database>Enable72_creative</Database>
    <UserName>epimsys</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAFUeatgjYLEW+yh6ro1KQKAAAAAACAAAAAAAQZgAAAAEAACAAAAA7w/apu4KCwKhFkU4k99ColYUlNO+qNFlYPRVoMXApiwAAAAAOgAAAAAIAACAAAAChjyOpcQDvGPQeoLMCIvmvfM8v9rCZAOvlZUB0nh3W1BAAAADulULumXcgFAUdJZ6BgnS+QAAAAG5dmbqoSyuNOdEZvwGAjNLvCEm8v+9CeJEI4e1LaOeOVe8ScYNTG/tM9D1RhVj5oEqC96Th2Nt9z72agY2GtOo=</Password>
  </Connection>
  <Reference>C:\perforce\Services\HHT\Tools\PimqlRun\bin\Debug\Enterworks.Enable.Epim.dll</Reference>
  <Reference>C:\perforce\Services\HHT\Tools\PimqlRun\bin\Debug\PimqlRun.exe</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Linq.dll</Reference>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{


	var threads = new Thread[tCount];
	//dc = new DumpContainer().Dump("starting");
	dc = new DumpContainer[tCount];

	//sq.Start();
	
	for ( var i = 0; i < tCount; i++ ){
	
		var url = "";
	
	   threads[i] = new Thread( new ParameterizedThreadStart( RunScript));
																
	   if ( (i%2) == 0 ){
	    // url = "http://localhost:9010/ui/tabs/DemoTabs.js?id=Contacts"; 
		 // url = "http://localhost:9018/DemoTabs.js?id=Contacts"; 
	    url = "http://localhost:63679/LinqpadWWW/scriptServer.ashx?script=ui/tabs/DemoTabs.js&id=Contacts"; 
		  
		} else {
		
	  	//url = "http://localhost:9010/ui/menus/Menu.js" ;
	    // url = "http://localhost:9018/Menu.js" ;
	  	  url = "http://localhost:63679/LinqpadWWW/scriptServer.ashx?script=ui/menus/Menu.js" ;
		
		}
		
		dc[i] = new DumpContainer().Dump("Starting thread #"+i.ToString() + "   url: " + url );
		
	    threads[i].Start(new Object[]{ dc[i], url });
	}

	
}
	int tCount = 4;
	int finished = 0;

int runPerScript = 400;

[MethodImpl( MethodImplOptions.Synchronized )]
private void Done(){

   if ( ++finished >= tCount ){
      sq.Stop();
	  dcMain.Content = new {  Hits = hitCount,
	  						 MillisPerHit = sq.ElapsedMilliseconds / hitCount
	  						,sq};
   }

}

DumpContainer dcMain = new DumpContainer().Dump("Running");
int hitCount = 0;
[MethodImpl( MethodImplOptions.Synchronized )]
private void Hit(){
  if ( hitCount == 0){
     sq.Start();
  }

   dcMain.Content = "Hits: " + (++hitCount).ToString();

}



Stopwatch sq = new Stopwatch();
DumpContainer[] dc = null;




public void RunScript( object args ){

   var o = args as object[];
   var tdc = o[0] as DumpContainer;
   var url = o[1] as string;

 
	for ( var i = 0; i < runPerScript; i++ ){
	   var success = false;
	   while ( !success ){
		try {
			var result = url.GetJSONHttp("application/json");
			tdc.Content = (i+1).ToString() + "  " + result;
			Hit();
			success = true;
			Thread.Sleep(3);
		} catch ( Exception e ){
		  e.Dump();
		  System.Threading.Thread.Sleep(30);
		}
		
	}
		
		//dc.Content ="http://localhost:9017/DemoTabs?id=Contacts".GetJSONHttp("application/json");
		
	
	}
  
   Done();
}