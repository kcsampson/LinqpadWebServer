<Query Kind="Program">
  <Reference>C:\perforce\Services\HHT\Tools\PimqlRun\bin\Debug\Enterworks.Enable.Epim.dll</Reference>
  <Reference>C:\perforce\Services\HHT\Tools\PimqlRun\bin\Debug\PimqlRun.exe</Reference>
</Query>

Dictionary<string,string> Main(Dictionary<string,string> inProps = null )
{
	
	// Test code....
	if ( inProps == null ){
	
	   inProps = new Dictionary<string,string>();
		
	}
		
	var sworker = new System.Web.Script.Serialization.JavaScriptSerializer();
	
	var menus = new List<object>(){
		new { Title = "Articles", Descript = "List of the latest articles, etc." },
		new { Title = "Feedback", Descript = "Form to provide feedback to the web master" },
		new { Title = "Organization", Descript = "Organization information, founding partners, etc." },
		
	var subMenus = new Dictionary<string, List<object>>(){
	
		{ "Articles", new List<object>() { new { Category = "Sports", Descript="Sporting in general"} 
										, new { Category = "Finance", Descript="Financial articles from top experts." }
										, new { Category = "Health", Descript="Updates on the latest Ebola scare." }
										} },
		{ "Feedback", new List<object>() { new { Category = "Performance", Descript ="Feedback form on site's performance" }
										  ,new { Category = "Bugs", Descript= "Submit a bug report" }
										  ,new { Category = "Style", Descript="Feedback on the site's layout" } } },
		{ "Organization", new List<object>() {
		} }
	
	
	}}
	
	
}


