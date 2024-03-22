using delLineDeath.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using RestSharp;

namespace delLineDeath.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string getGreet (string name, string lastName)
        {
            return $"Hello, {name} {lastName}";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> FetchTry(string searchQ)
        {
            var options = new RestClientOptions("https://api.dellin.ru/v2/public/kladr.json")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://api.dellin.ru/v2/public/kladr.json", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{ ""appkey"": ""FB925861-15BF-4A2B-BFBB-A6375EB2C5B3"", ""q"":"""+searchQ+@"""}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            //System.Diagnostics.Debug.WriteLine(response.Content);
            return Json(response.Content);
            //return Json(body);
        }

        [HttpPost]
        public async Task<String> FetchCalculator(string cityID)
        {
            var options = new RestClientOptions("https://api.dellin.ru/v1/public/request_terminals.json")
            {MaxTimeout = -1};
            var client = new RestClient(options);
            var request = new RestRequest("https://api.dellin.ru/v1/public/request_terminals.json", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""appkey"": ""FB925861-15BF-4A2B-BFBB-A6375EB2C5B3"",""cityID"": """ + cityID + @"""}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            var terminalID = 0;
            try
            {
                //parse recived terminal json 
                dynamic jsonObject = JsonConvert.DeserializeObject(response.Content);
                int firstId = jsonObject.terminals[0].id;

                var options1 = new RestClientOptions("https://api.dellin.ru/v2/calculator.json")
                {MaxTimeout = -1};
                var client1 = new RestClient(options1);
                var request1 = new RestRequest("https://api.dellin.ru/v2/calculator.json", Method.Post);
                request1.AddHeader("Content-Type", "application/json");
                request1.AddHeader("Cookie", "qrator_msid=1710975746.076.g6eYeLwDP4zLbADv-fojrk1sjdci6ko7pc3jdc33caneir2p7");
                var body1 = @"{""appkey"":""FB925861-15BF-4A2B-BFBB-A6375EB2C5B3"",""delivery"":{""deliveryType"":{""type"":""auto""},""arrival"":{""variant"":""terminal"",""terminalID"":"""+firstId+@"""},""derival"":{""produceDate"":""2024-04-08"",""variant"":""terminal"",""terminalID"":""104""}},""cargo"":{""quantity"":1,""length"":1,""width"":1,""height"":1,""weight"":200,""totalVolume"":1,""totalWeight"":200,""oversizedWeight"":0,""oversizedVolume"":0,""hazardClass"":0}}";

                request1.AddStringBody(body1, DataFormat.Json);
                RestResponse response1 = await client1.ExecuteAsync(request1);
                return response1.Content;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }




            //var options = new RestClientOptions("")
            //{
            //    MaxTimeout = -1,
            //};
            //var client = new RestClient(options);
            //var request = new RestRequest("https://api.dellin.ru/v2/calculator.json", Method.Post);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Cookie", "qrator_msid=1710975746.076.g6eYeLwDP4zLbADv-fojrk1sjdci6ko7pc3jdc33caneir2p7");
            //var body = @"{""appkey"":""FB925861-15BF-4A2B-BFBB-A6375EB2C5B3"",""delivery"":{""deliveryType"":{""type"":""express""},""arrival"":{""variant"":""terminal"",""terminalID"":""104""},""derival"":{""produceDate"":""2024-04-08"",""variant"":""terminal"",""terminalID"":""57""}},""cargo"":{""quantity"":1,""length"":1,""width"":1,""height"":1,""weight"":200,""totalVolume"":1,""totalWeight"":200,""oversizedWeight"":0,""oversizedVolume"":0,""hazardClass"":0}}";

            //request.AddStringBody(body, DataFormat.Json);
            //RestResponse response = await client.ExecuteAsync(request);
            //Console.WriteLine(response.Content);
        }
    }
}
