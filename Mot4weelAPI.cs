using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static List<MOT4Wheels> DbMOT4WheelsList;

        public static int TotalRowOver = 0;

        public static int TotalAddNewCar = 0;

        public static int TotalChangeBaalut = 0;

        public static int TotalChangeTokefDate = 0;

        public void RunAPI()
        {
            using (var Context = new Context())
            {

                try
                {
                   
                    
                    // אם קיים קישור בקונפיג
                    string CsvLink = ConfigurationManager.AppSettings.Get("Mot4wheels");

                    // כל הטבלה הקיימת כרגע
                    DbMOT4WheelsList = Context.MOT4Wheels.AsNoTracking().ToList();

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOT4Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT4Wheels from csv";

                        Context.Logs.Add(log);


                        StreamReader csvreader = new StreamReader(CsvLink, Encoding.Default, true);
                        string inputLine = "";
                        while ((inputLine = csvreader.ReadLine()) != null)
                        {

                            string[] csvArray = inputLine.Split(new char[] { '|' });
                            MOT4Wheels MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray);

                            DBDeltaCheck(Context, MOT4WheelsFromCsv);
                            TotalRowOver++;

                        }






                    }
                    // הורדה מגוב
                    else
                    {
                        Logs log = new Logs();
                        log.TableName = "MOT4Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT4Wheels from gov...";

                        Context.Logs.Add(log);


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
                            responseObj = GetInfo(requestParams, Context).Result;

                            CountOffset = CountOffset + 50000;

                        } while (responseObj > 50);


                    }


                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOT4Wheels";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOT4Wheels";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOT4Wheels";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);




                    Context.SaveChanges();

                }

            }



        }

        private MOT4Wheels GetMOT4WheelsObj(string[] csvArray)
        {



            MOT4Wheels MOT4WheelsFromCsv = new MOT4Wheels()
            {

                    mispar_rechev = Helper.ConvertToInt(csvArray[0]),
                    tozeret_cd = csvArray[1].Replace("\"", ""),
                    sug_degem = csvArray[2].Replace("\"", ""),
                    tozeret_nm = csvArray[3].Replace("\"", ""),
                    degem_cd = csvArray[4].Replace("\"", ""),
                    degem_nm = csvArray[5].Replace("\"", ""),
                    ramat_gimur = csvArray[6].Replace("\"", ""),
                    ramat_eivzur_betihuty = csvArray[7].Replace("\"", ""),
                    kvutzat_zihum = csvArray[8].Replace("\"", ""),
                    shnat_yitzur = csvArray[9].Replace("\"", ""),
                    degem_manoa = csvArray[10].Replace("\"", ""),
                    mivchan_acharon_dt = Helper.ConvertToDatetime(csvArray[11]),
                    tokef_dt = Helper.ConvertToDatetime(csvArray[12]),
                    baalut = csvArray[13].Replace("\"", ""),
                    misgeret = csvArray[14].Replace("\"", ""),
                    tzeva_cd = Helper.ConvertToInt(csvArray[15]),
                    tzeva_rechev = csvArray[16].Replace("\"", ""),
                    zmig_kidmi = csvArray[17].Replace("\"", ""),
                    zmig_ahori = csvArray[18].Replace("\"", ""),
                    sug_delek_nm = csvArray[19].Replace("\"", ""),
                    horaat_rishum = csvArray[20].Replace("\"", ""),
                    moed_aliya_lakvish = csvArray[21].Replace("\"", ""),
                    kinuy_mishari = csvArray[22].Replace("\"", "")

            };


            return MOT4WheelsFromCsv;
        }

        public static async Task<int> GetInfo(string requestParams, Context Context)
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


                        MOT4Wheels MOT4WheelsFromGov = JsonConvert.DeserializeObject<MOT4Wheels>(x.ToString());

                        DBDeltaCheck(Context, MOT4WheelsFromGov);

                        CountScan++;


                    }



                }
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, MOT4Wheels MOT4WheelsObj)
        {
            var CurrentCarInDB = DbMOT4WheelsList.Where(m => m.mispar_rechev == MOT4WheelsObj.mispar_rechev).FirstOrDefault();

            //רכב חדש
            if (CurrentCarInDB == null)
            {
                Context.MOT4Wheels.Add(MOT4WheelsObj);
                TotalAddNewCar++;

                Console.WriteLine(TotalRowOver.ToString() + "." + " Add New - " + MOT4WheelsObj.mispar_rechev);
            }
            else
            {

                if (MOT4WheelsObj.baalut != CurrentCarInDB.baalut)
                {

                    CarHoldingHistory ch = new CarHoldingHistory()
                    {
                        mispar_rechev = CurrentCarInDB.mispar_rechev,
                        baalut = CurrentCarInDB.baalut,
                        LastScanDate = DateTime.Now
                    };

                    Context.CarHoldingHistory.Add(ch);


                    // עדכון בעלות לחדש
                    CurrentCarInDB.baalut = MOT4WheelsObj.baalut;
                    Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                    TotalChangeBaalut++;
                    Console.WriteLine(TotalRowOver.ToString() + "." + " Change Baalut - " + MOT4WheelsObj.mispar_rechev);

                }


                if (MOT4WheelsObj.tokef_dt != CurrentCarInDB.tokef_dt)
                {


                    CarHoldingHistory ch = new CarHoldingHistory()
                    {
                        tokef_dt = CurrentCarInDB.tokef_dt,
                        mivchan_acharon_dt = CurrentCarInDB.mivchan_acharon_dt,
                        LastScanDate = DateTime.Now
                    };

                    Context.CarHoldingHistory.Add(ch);


                    // עדכון תוקף לחדש
                    CurrentCarInDB.tokef_dt = MOT4WheelsObj.tokef_dt;
                    CurrentCarInDB.mivchan_acharon_dt = MOT4WheelsObj.mivchan_acharon_dt;
                    Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                    TotalChangeTokefDate++;
                    Console.WriteLine(TotalRowOver.ToString() + "." + " Change TokefDate - " + MOT4WheelsObj.mispar_rechev);


                }



            }

          
            TotalRowOver++;
        }
    }




}
