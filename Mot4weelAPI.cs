using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GovAPI
{
    class Mot4weelAPI
    {

        public void RunAPI()
        {
            //using (var Context = new Context())
            //{
            //   MOT4Wheels la = Context.MOT4Wheels.FirstOrDefault();

            //}

               


          

            //// Initilization.  
            List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
            string requestParams = string.Empty;

            // Converting Request Params to Key Value Pair.  
            allIputParams.Add(new KeyValuePair<string, string>("resource_id", "053cea08-09bc-40ec-8f7a-156f0677aff3"));
            allIputParams.Add(new KeyValuePair<string, string>("limit", "5"));
            allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
            // URL Request Query parameters.  



            int responseObj = 0;
            int CountOffset = 0;

            do
            {
                allIputParams.RemoveAll(x => x.Key == "offset");
                allIputParams.Add(new KeyValuePair<string, string>("offset", CountOffset.ToString()));

                requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                // Call REST Web API with parameters.  
                responseObj = GetInfo(requestParams).Result;

                CountOffset = CountOffset + 5000000;

            } while (responseObj > 0);

            //List<string> splitted = new List<string>();

            //Task fileList = HttpGetForLargeFileInWrongWay("https://data.gov.il/dataset/private-and-commercial-vehicles/resource/053cea08-09bc-40ec-8f7a-156f0677aff3/download/053cea08-09bc-40ec-8f7a-156f0677aff3.csv");

            //string[] tempStr;

            //tempStr = fileList.Split(',');

            //foreach (string item in tempStr)
            //{
            //    if (!string.IsNullOrWhiteSpace(item))
            //    {
            //        splitted.Add(item);
            //    }
            //}
        }

        public async Task<string> GetCSVAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //  HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();

                return results;
            }


        }


        static async Task HttpGetForLargeFileInWrongWay(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                //const string url = "https://github.com/tugberkugurlu/ASPNETWebAPISamples/archive/master.zip";
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    string fileToWriteTo = Path.GetTempFileName();
                    using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }

                    response.Content = null;
                }
            }
        }


        public static async Task<int> GetInfo(string requestParams)
        {


           
            int CountScan = 0;

            // HTTP GET.  
            using (var client = new HttpClient())
            {
                // Setting Base address.  
                client.BaseAddress = new Uri("https://data.gov.il/");

                // Setting content type.  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.  
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP GET  
                response = await client.GetAsync("api/3/action/datastore_search?" + requestParams).ConfigureAwait(false);

                // Verification  
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.  
                    string result = response.Content.ReadAsStringAsync().Result;
                    JObject parent = JObject.Parse(result);
                    var records = parent["result"]["records"];
                    foreach (var x in records)
                    {
                        CountScan++;
                        x["_id"].ToString();
                    }

                 

                }
            }
            return CountScan;

        }



    }




}
