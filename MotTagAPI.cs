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
    class MotTagAPI
    {

        public static List<MOTTags> DbMOTTagsList;

        public static List<int> MOTTagsNewListAll = new List<int>(); //

        public static List<MOTTags> MOTTagsNewList = new List<MOTTags>();

        public static Dictionary<int, MOTTags> DictionaryMot = new Dictionary<int, MOTTags>(); //

        public static List<int> DictionaryMotFromGovOrCsv = new List<int>(); //

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
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTTags");

                    // כל הטבלה הקיימת כרגע
                    DbMOTTagsList = Context.MOTTags.AsNoTracking().ToList();

                    DictionaryMot = DbMOTTagsList.ToDictionary(x => x.MISPAR_RECHEV, x => x);
                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOTTags";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTTags from csv";

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
                                    MOTTags MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray, colHeader);

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
                        log.TableName = "MOTTags";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTTags from gov...";

                        Context.Logs.Add(log);


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "c8b9f9c8-4612-4068-934f-d4acd2e3c06e"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {

                            SaveMOT4WheelsNewList();
                            //רק לפיתוח
                            //  SaveMOTNOTActive();

                            allIputParams.RemoveAll(x => x.Key == "offset");
                            allIputParams.Add(new KeyValuePair<string, string>("offset", CountOffset.ToString()));

                            requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                            // Call REST Web API with parameters.  
                            responseObj = GetInfo(requestParams, Context).Result;

                            CountOffset = CountOffset + 100000;

                        } while (responseObj > 0);


                    }

                    SaveMOT4WheelsNewList();
                    SaveMOTNOTActive();

                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOTTags";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOTTags";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOTTags";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);




                    Context.SaveChanges();

                }

            }



        }
        private void SaveMOTNOTActive()
        {


            using (var Context = new Context())
            {



                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;


                var ListNotActive = DbMOTTagsList.Where(x => !DictionaryMotFromGovOrCsv.Contains(x.MISPAR_RECHEV)).ToList();

                foreach (var CurrentCarInDB in ListNotActive)
                {
                    CurrentCarInDB.Active = false;
                    Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                }
                //foreach (var item in DictionaryMotFromGovOrCsv)
                //{

                //    MOTTags CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOT4WheelsObj.mispar_rechev).FirstOrDefault();

                //    DictionaryMot.TryGetValue(item, out CurrentCarInDB);

                //    if (CurrentCarInDB == null)
                //    {
                //        CurrentCarInDB.Active = false;
                //        Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;
                //    }
                //}


                Context.SaveChanges();


            }

        }

        private void SaveMOT4WheelsNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOTTags.AddRange(MOTTagsNewList);


                Context.SaveChanges();
                MOTTagsNewList.Clear();
            }

        }




        public MOTTags MOT4WheelsFromCsvTemp = new MOTTags();

        private MOTTags GetMOT4WheelsObj(string[] csvArray, string[] colHeader)
        {

            MOTTags MOT4WheelsFromCsv = MOT4WheelsFromCsvTemp;

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


                        MOTTags MOT4WheelsFromGov = new MOTTags();//
                        MOT4WheelsFromGov.MISPAR_RECHEV = Helper.ConvertToInt(x["MISPAR RECHEV"].ToString());
                        MOT4WheelsFromGov.TAARICH_HAFAKAT_TAG = x["TAARICH HAFAKAT TAG"].ToString();
                        MOT4WheelsFromGov.SUG_TAV = x["SUG TAV"].ToString();
                        //JsonConvert.DeserializeObject<MOTTags>(x.ToString());

                        DBDeltaCheck(Context, MOT4WheelsFromGov);

                        CountScan++;


                    }



                }
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, MOTTags MOTTagsObj)
        {

            TotalRowOver++;



            MOTTags CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOT2WheelsObj.mispar_rechev).FirstOrDefault();


            DictionaryMot.TryGetValue(MOTTagsObj.MISPAR_RECHEV, out CurrentCarInDB);



            //var CurrentCarInDB = Context.MOTTags.Where(m => m.MISPAR_RECHEV == MOTTagsObj.MISPAR_RECHEV).FirstOrDefault();

            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.MOTTags.Add(MOT4WheelsObj);

                if (!MOTTagsNewListAll.Any(x => x == MOTTagsObj.MISPAR_RECHEV))
                {

                    MOTTagsNewList.Add(MOTTagsObj);
                    TotalAddNewCar++;
                    MOTTagsNewListAll.Add(MOTTagsObj.MISPAR_RECHEV);
                    Console.WriteLine("6)" + TotalRowOver.ToString() + "." + " Add New - " + MOTTagsObj.MISPAR_RECHEV);

                }

                //Context.SaveChanges();
            }
            else
            {


                DictionaryMotFromGovOrCsv.Add(MOTTagsObj.MISPAR_RECHEV);

                Console.WriteLine("6)" + TotalRowOver.ToString() + "." + " No New - ");

            }





        }
    }
}




