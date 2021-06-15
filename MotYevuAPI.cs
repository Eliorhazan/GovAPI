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
    class MotYevuAPI
    {

        public static List<MOTYevu> DbMOTYevuList;

        public static List<MOTYevu> MOTYevuNewList = new List<MOTYevu>();

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
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTYevu");

                    // כל הטבלה הקיימת כרגע
                    DbMOTYevuList = Context.MOTYevu.AsNoTracking().ToList();

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOTYevu";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTYevu from csv";

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
                                    MOTYevu MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray, colHeader);

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
                        log.TableName = "MOTYevu";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTYevu from gov...";

                        Context.Logs.Add(log);


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "03adc637-b6fe-402b-9937-7c3d3afc9140"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {

                            SaveMOT4WheelsNewList();

                            allIputParams.RemoveAll(x => x.Key == "offset");
                            allIputParams.Add(new KeyValuePair<string, string>("offset", CountOffset.ToString()));

                            requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                            // Call REST Web API with parameters.  
                            responseObj = GetInfo(requestParams, Context).Result;

                            CountOffset = CountOffset + 100000;

                        } while (responseObj > 0);


                    }

                    SaveMOT4WheelsNewList();


                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOTYevu";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOTYevu";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOTYevu";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);




                    Context.SaveChanges();

                }

            }



        }

        private void SaveMOT4WheelsNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOTYevu.AddRange(MOTYevuNewList);
                Context.SaveChanges();
                MOTYevuNewList.Clear();
            }

        }




        public MOTYevu MOT4WheelsFromCsvTemp = new MOTYevu();

        private MOTYevu GetMOT4WheelsObj(string[] csvArray, string[] colHeader)
        {

            MOTYevu MOT4WheelsFromCsv = MOT4WheelsFromCsvTemp;

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


                        MOTYevu MOT4WheelsFromGov = JsonConvert.DeserializeObject<MOTYevu>(x.ToString());

                        DBDeltaCheck(Context, MOT4WheelsFromGov);

                        CountScan++;


                    }



                }
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, MOTYevu MOTYevuObj)
        {

            TotalRowOver++;

            var CurrentCarInDB = DbMOTYevuList.Where(m => m.mispar_rechev == MOTYevuObj.mispar_rechev).FirstOrDefault();

            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.MOTYevu.Add(MOT4WheelsObj);


                MOTYevuNewList.Add(MOTYevuObj);
                TotalAddNewCar++;
                Console.WriteLine(TotalRowOver.ToString() + "." + " Add New - " + MOTYevuObj.mispar_rechev);

                //Context.SaveChanges();
            }
            else
            {

                Console.WriteLine("8)" + TotalRowOver.ToString() + "." + " No New - ");

            }





        }
    }
}




