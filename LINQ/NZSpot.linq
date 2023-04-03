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

public class Settlement 
{
	public double[][] data;
}

public class HedgeType 
{
	public int PurchaseIndx;	
}

void Main()
{
	//Download();
	var spot = Parse();
	
	spot.Dump();
	
	var settlement = ParseSettlement();
settlement.data.Dump();

	var q = from a in spot
		join b in settlement.data on a[0] equals b[0]
		select (a,b);
		
		q.Dump();

	var obj2 = new { name = "spot", data = spot };
	var json3 = "const spotSeries = " + JsonConvert.SerializeObject(obj2).Replace("\"", "\'");


	File.WriteAllText(@"C:\disk.win\hub\HTML\Hedge\json\" + "spots.js", json3);


var obj = new { name = "settlement", data = settlement.data };
	settlement.Dump();

	var json2 = "const settlementSeries = " + JsonConvert.SerializeObject(obj).Replace("\"", "\'");
	
	

	File.WriteAllText(@"C:\disk.win\hub\HTML\Hedge\json\" + "settlements.js", json2);
	var hedgeType = new HedgeType() { PurchaseIndx = 100 };

	var purchasePrice = settlement.data.Skip(hedgeType.PurchaseIndx).First();
	var purchasePriceVector = settlement.data.Skip(hedgeType.PurchaseIndx).Select(o => new[] { o[0], o[1]}).ToArray();
	
	foreach (var element in purchasePriceVector)
	{
		element[1] = purchasePrice[1];
	}
	
	
	
	var obj3 = new { name = "purchasePrice", data = purchasePriceVector };
	var series = new [] {obj2, obj, obj3};
	
	settlement.Dump();
	var json = "const series = " + JsonConvert.SerializeObject(series).Replace("\"", "\'") + ";const vLine = " + purchasePrice[0];


	File.WriteAllText(@"C:\disk.win\hub\HTML\Hedge\json\" + "hedgeType.js", json);

	
}

static Settlement ParseSettlement()
{
	var txt = File.ReadAllText(@"C:\disk.win\tmp\nz\settlement.txt");
	var settlement = JsonConvert.DeserializeObject<Settlement>(txt);

	return settlement;
}

static long ToUnixLong(DateTime dt)
{
	TimeSpan t = dt - new DateTime(1970, 1, 1);
	int secondsSinceEpoch = (int)t.TotalSeconds ;
	return secondsSinceEpoch ;
}

static double[][] Parse()
{
	var bankHolidays = new[] {

	"25/12/2020",
	"28/12/2020",
	"01/01/2021",
	"04/01/2021", 
	//"08/02/2021", 
	"02/04/2021",
	"05/04/2021", 
	//"26/04/2021" ,
	//"07/06/2021"
	};
	var txt = File.ReadAllText(@"C:\disk.win\tmp\nz\spot.txt");
	var lines = txt.Split(new[] { Environment.NewLine },
									 StringSplitOptions.RemoveEmptyEntries)

									 .Skip(9)

									 .Select(o => o.Split(','))

									 ;

	var objs = lines.Select(o => new Spot { Start = Convert.ToDateTime(o[0]), StartString = o[0], RegionId = o[2], Region = o[3], Price = Convert.ToDouble(o[4]) });
	//objs.Dump();
	// new[] { new { name = "spot",
	var data = objs.Where(o => o.RegionId == "OTA2201")
	.Where(o => o.Start.DayOfWeek != DayOfWeek.Saturday && o.Start.DayOfWeek != DayOfWeek.Sunday)

	.Where(o => !bankHolidays.Contains(o.StartString))
	//.Dump()
	.Select(o => new { Unix = ToUnixLong(o.Start) * 1000, o.StartString, o.Price })
	//.Dump()
	.Select(o => new[] { o.Unix, o.Price })
	.ToArray();

	return data;
	data.Dump();

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