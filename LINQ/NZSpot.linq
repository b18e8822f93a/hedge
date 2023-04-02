<Query Kind="Program">
  <NuGetReference>Hyperlinq</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>WriteableBitmapEx</NuGetReference>
  <Namespace>Hyperlinq</Namespace>
  <Namespace>static Hyperlinq.H</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

void Main()
{
	//Download();
	Parse();
}



static void Parse() 
{
	var bankHolidays = new[] {"26/10/2020", "25/12/2020", "28/12/2020",  "01/01/2021", "04/01/2021", "08/02/2021", "02/04/2021","05/04/2021", "26/04/2021" , "07/06/2021"};
	var txt = File.ReadAllText(@"C:\disk.win\tmp\nz\spot.txt");
	var lines = txt.Split(new[] { Environment.NewLine },
									 StringSplitOptions.RemoveEmptyEntries)
									
									 .Skip(9)
									 
									 .Select(o => o.Split(',' ))
									
									 ;
									 
	var objs = lines.Select(o => new Spot { Start = Convert.ToDateTime( o[0]), StartString = o[0], RegionId = o[2], Region= o[3], Price = Convert.ToDouble( o[4]) });
	//objs.Dump();

	var data = new[] { new { name = "spot", data = objs.Where(o => o.RegionId == "OTA2201")
	.Where(o => o .Start.DayOfWeek != DayOfWeek.Saturday && o.Start.DayOfWeek != DayOfWeek.Sunday)
	
	//.Where(o => !bankHolidays.Contains(o.StartString))
	.Dump()
	.Select(o => new { o.StartString, o.Price })
	.Dump()
	.Select(o =>  o.Price )
	.ToArray() }};
	data.Dump();

	var json = "const series = " + JsonConvert.SerializeObject(data).Replace("\"", "\'");
	

	File.WriteAllText(@"C:\disk.win\hub\HTML\Hedge\" + "spots.js", json);
}
static void Download()
{
	using (WebClient client = new WebClient())
	{
			var url = @"https://www.emi.ea.govt.nz/Wholesale/Download/DataReport/CSV/W_P_C?DateFrom=20201007&DateTo=20210930&RegionType=POC_UNI&TimeScale=DAY&WeightedBy=SIMPLE&_si=v|4";
			var arr = client.DownloadString(url);
			
			File.WriteAllText(@"C:\disk.win\tmp\nz\spot.txt", arr);
	
	}
}

public class Spot 
{
	public DateTime Start; public string StartString; public string RegionId; public string Region; public double Price;
}