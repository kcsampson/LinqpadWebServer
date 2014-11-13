<Query Kind="Program">
  <Connection>
    <ID>662cae12-7657-49c6-8b2b-57e7f98b0380</ID>
    <Persist>true</Persist>
    <Server>kimballsam\KQL</Server>
    <SqlSecurity>true</SqlSecurity>
    <NoPluralization>true</NoPluralization>
    <NoCapitalization>true</NoCapitalization>
    <ExcludeRoutines>true</ExcludeRoutines>
    <UserName>epimsys</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAWZ/cmoE63kSrFr9GBp+9YgAAAAACAAAAAAAQZgAAAAEAACAAAABL3WG8o3cQIFa0uH97Qn+AXfQYgQjgAaLSo5y5j4RYrAAAAAAOgAAAAAIAACAAAADzhiOcRmhm9BfZbik0yjR0mqOo8OG9icGSWD0VRmiTPhAAAACmxH1A7HbLD9dlwBrhL5pfQAAAAFr64GAbbyZKYsS1PpFFwPBQq9fGYiOOMjKASV67yerRT0VY8nT4S2k4InU2LlyE+NMTotnt/6kwRdTXYEhnc5s=</Password>
    <Database>Enable72_Creative</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Extensions.dll</Reference>
  <Namespace>System.Web.Script.Serialization</Namespace>
</Query>

Dictionary<string,string> Main(Dictionary<string,string> inProps = null )
{




   var outProps = new Dictionary<string,string>();

   if ( inProps == null ){
      inProps = new Dictionary<string,string>();
	  inProps["id"]="Contacts";
   }
   
   
   var tabs = new List<object>(){
   			new { tabName="Hello", id="Hello" },
			new {tabName="About", id="About"},
			new{ tabName="Contacts", id="Contacts"}
   
   }; 
   var tabData = new Dictionary<string, List<string>>(){
   
     { "Hello",new List<string>(){  "How are you", "I am fine", "Nice talking to you"}},
   { "About",new List<string>(){"kimbonics is web development made simple", "scripting"}},
    { "Contacts",  B_USER.Where( bu=> bu.LOGIN.Contains("test")).Select( bu => bu.LOGIN + " - " + bu.LAST_NAME ).ToList()
	}
   
   };
   
  
 
 //tabData["Contacts"].Add("xxxx");
   
   var sworker = new System.Web.Script.Serialization.JavaScriptSerializer();
   
   if ( inProps.ContainsKey("id") ){
   
   	outProps["ResultJSON"] = sworker.Serialize( tabData[inProps["id"]] );
	//tabData[inProps["id"]].Dump();
   } else {
  // tabs.Dump();
      
   	outProps["ResultJSON"] = sworker.Serialize( tabs );
   }
   
   outProps["ResultJSON"].Dump();
   
   return outProps;
	
}