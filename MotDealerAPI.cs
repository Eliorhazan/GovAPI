using Microsoft.VisualBasic.FileIO;
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
    class MotDealerAPI
    {

        public static List<CarDealers> DbCarDealersList;

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
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTCarDealers");

                    // כל הטבלה הקיימת כרגע
                    DbCarDealersList = Context.CarDealers.AsNoTracking().ToList();

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "CarDealers";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download CarDealers from csv";

                        Context.Logs.Add(log);



                        string[] colHeader = null;

                        bool IsFirst = true;


                        StreamReader csvreader = new StreamReader(CsvLink, Encoding.Default, true);
                        string inputLine = "";
                        while ((inputLine = csvreader.ReadLine()) != null)
                        {

                            try
                            {
                                if (IsFirst)
                                {
                                    colHeader = inputLine.Split(new char[] { '|' });
                                    IsFirst = false;

                                }
                                else
                                {

                                    string[] csvArray = inputLine.Split(new char[] { '|' });
                                    CarDealers MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, MOT4WheelsFromCsv);


                                }

                            }
                            catch (Exception ex)
                            {

                            }



                        }






                    }
                    // הורדה מגוב
                    else
                    {
                        Logs log = new Logs();
                        log.TableName = "CarDealers";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download CarDealers from gov...";

                        Context.Logs.Add(log);


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "eb74ad8c-ffcd-43bb-949c-2244fc8a8651"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "50000"));
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

                        } while (responseObj > 0);


                    }


                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "CarDealers";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "CarDealers";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download CarDealers";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);




                    Context.SaveChanges();

                }

            }



        }

        public CarDealers MOT4WheelsFromCsvTemp = new CarDealers();

        private CarDealers GetMOT4WheelsObj(string[] csvArray, string[] colHeader)
        {

            CarDealers MOT4WheelsFromCsv = MOT4WheelsFromCsvTemp;

            for (int i = 0; i < colHeader.Length; i++)
            {


                try
                {
                    var PropTypeName = Helper.GetTypeOfEntity(MOT4WheelsFromCsv, colHeader[i].ToString());

                    if (PropTypeName == "Int32")
                        MOT4WheelsFromCsv[colHeader[i].ToString()] = Helper.ConvertToInt(csvArray[i]);
                    else if (PropTypeName == "Nullable`1")
                        MOT4WheelsFromCsv[colHeader[i].ToString()] = Helper.ConvertToDatetime(csvArray[i]);

                    else
                        MOT4WheelsFromCsv[colHeader[i].ToString()] = csvArray[i].Replace("\"", "");

                }
                catch (Exception ex)
                {


                }

                // }
            }

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


                        CarDealers MOT4WheelsFromGov = JsonConvert.DeserializeObject<CarDealers>(x.ToString());

                        if (MOT4WheelsFromGov.ktovet != null && MOT4WheelsFromGov.ktovet.Length > 55)
                        {
                            MOT4WheelsFromGov.ktovet = MOT4WheelsFromGov.ktovet.Substring(0, 54);

                        }

                        if (MOT4WheelsFromGov.shem != null && MOT4WheelsFromGov.shem.Length > 55)
                        {
                            MOT4WheelsFromGov.shem = MOT4WheelsFromGov.shem.Substring(0, 54);

                        }

                        DBDeltaCheck(Context, MOT4WheelsFromGov);

                        CountScan++;


                    }



                }
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, CarDealers MOT4WheelsObj)
        {
            try
            {
                TotalRowOver++;

                var CurrentCarInDB = DbCarDealersList.Where(m => m.shem == MOT4WheelsObj.shem && m.mikud == MOT4WheelsObj.mikud && m.ktovet == MOT4WheelsObj.ktovet).FirstOrDefault();

                //רכב חדש
                if (CurrentCarInDB == null)
                {
                    Context.CarDealers.Add(MOT4WheelsObj);
                    TotalAddNewCar++;
                    Console.WriteLine(TotalRowOver.ToString() + "." + " Add New - ");

                    Context.SaveChanges();
                }

            }
            catch (Exception ex)
            {


            }




        }
    }
}
