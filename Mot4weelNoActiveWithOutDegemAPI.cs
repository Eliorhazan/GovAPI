using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;



namespace GovAPI
{
    class Mot4weelNoActiveWithOutDegemAPI
    {

        public static List<MOTNotActiveWithOutDegem> DbMOTNotActiveWithOutDegemList;

        public static Dictionary<int, MOTNotActiveWithOutDegem> DictionaryMot = new Dictionary<int, MOTNotActiveWithOutDegem>(); //

      //  public static List<int> DictionaryMotFromGovOrCsv = new List<int>(); //

        public static List<MOTNotActiveWithOutDegem> MOTNotActiveWithOutDegemNewList = new List<MOTNotActiveWithOutDegem>();

        public static List<CarHoldingHistory> CarHoldingHistoryNewList = new List<CarHoldingHistory>();

        public static List<MOTNotActiveWithOutDegem> MOTNotActiveWithOutDegemChangeList = new List<MOTNotActiveWithOutDegem>();



        public static int TotalRowOver = 0;

        public static int TotalAddNewCar = 0;

        public static int TotalChangeBaalut = 0;

        public static int TotalChangeTokefDate = 0;



        //public static string IsFirstUse = "";
        public void RunAPI()
        {
            using (var Context = new Context())
            {

                try
                {



                    // אם קיים קישור בקונפיג
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTNotActiveWithOutDegemNoActiveWithDegem");

                    // כל הטבלה הקיימת כרגע
                    DbMOTNotActiveWithOutDegemList = Context.MOTNotActiveWithOutDegem.AsNoTracking().ToList();

                    DictionaryMot = DbMOTNotActiveWithOutDegemList.ToDictionary(x => x.mispar_rechev, x => x);

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOTNotActiveWithOutDegem NoActiveWithDegem";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTNotActiveWithOutDegem  csv";

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
                                    MOTNotActiveWithOutDegem MOTNotActiveWithOutDegemFromCsv = GetMOTNotActiveWithOutDegemObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, MOTNotActiveWithOutDegemFromCsv);

                                }

                             

                                if (TotalAddNewCar!=0 &&  TotalAddNewCar % 100000 == 0)
                                {
                                    SaveMOTNotActiveWithOutDegemNewList();
                                }


                                if ((TotalChangeBaalut!=0 && TotalChangeBaalut % 50000 == 0) || (TotalChangeTokefDate != 0 && TotalChangeTokefDate % 50000 == 0))
                                {
                                    SaveMOTNotActiveWithOutDegemChangeList();
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
                        log.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = "Download MOTNotActiveWithOutDegem gov";

                        Context.Logs.Add(log);

                      


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "6f6acd03-f351-4a8f-8ecf-df792f4f573a"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {


                            try
                            {

                                SaveMOTNotActiveWithOutDegemNewList();
                                SaveMOTNotActiveWithOutDegemChangeList();

                                allIputParams.RemoveAll(x => x.Key == "offset");
                                allIputParams.Add(new KeyValuePair<string, string>("offset", CountOffset.ToString()));

                                requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                                // Call REST Web API with parameters.  
                                responseObj = GetInfo(requestParams, Context).Result;

                                if (responseObj == -1)
                                {
                                    responseObj = 2;

                                }
                                else
                                {
                                    CountOffset = CountOffset + 100000;

                                }



                            }
                            catch (Exception ex)
                            {
                                Logs logInner = new Logs();
                                logInner.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                                logInner.TimeStamp = DateTime.Now;
                                logInner.ActionName = "Exception";
                                logInner.Exeption = ex.Message + ex.InnerException;

                                Context.Logs.Add(logInner);


                            }

                        } while (responseObj > 0);



                    }

                    SaveMOTNotActiveWithOutDegemNewList();
                    SaveMOTNotActiveWithOutDegemChangeList();
                 




                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message + ex.InnerException;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOTNotActiveWithOutDegemNoActiveWithDegem";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);


                    Context.SaveChanges();

                }

            }



        }

      

        private void SaveMOTNotActiveWithOutDegemNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOTNotActiveWithOutDegem.AddRange(MOTNotActiveWithOutDegemNewList);
                Context.SaveChanges();
                MOTNotActiveWithOutDegemNewList.Clear();
            }




           

        }

        private void SaveMOTNotActiveWithOutDegemChangeList()
        {

            using (var Context = new Context())
            {


            

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var item in MOTNotActiveWithOutDegemChangeList)
                {
                    Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                Context.SaveChanges();
               
                MOTNotActiveWithOutDegemChangeList.Clear();

            }
        }




      

        private static MOTNotActiveWithOutDegem GetMOTNotActiveWithOutDegemObj(string[] csvArray, string[] colHeader)
        {

            MOTNotActiveWithOutDegem MOTNotActiveWithOutDegemFromCsv = new MOTNotActiveWithOutDegem();

            for (int i = 0; i < colHeader.Length; i++)
            {


                try
                {
                    var PropTypeName = Helper.GetTypeOfEntity(MOTNotActiveWithOutDegemFromCsv, colHeader[i].ToString());

                    if (PropTypeName == "Int32")
                        MOTNotActiveWithOutDegemFromCsv[colHeader[i].ToString()] = Helper.ConvertToInt(csvArray[i]);
                    else if (PropTypeName == "Nullable`1")
                        MOTNotActiveWithOutDegemFromCsv[colHeader[i].ToString()] = Helper.ConvertToDatetime(csvArray[i]);

                    else
                        MOTNotActiveWithOutDegemFromCsv[colHeader[i].ToString()] = csvArray[i].Replace("\"", "");

                }
                catch (Exception ex)
                {


                }

                // }
            }

          

            return MOTNotActiveWithOutDegemFromCsv;
        }

        public static async Task<int> GetInfo(string requestParams, Context Context)
        {

            try
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


                            MOTNotActiveWithOutDegem MOTNotActiveWithOutDegemFromGov = JsonConvert.DeserializeObject<MOTNotActiveWithOutDegem>(x.ToString());

                            DBDeltaCheck(Context, MOTNotActiveWithOutDegemFromGov);

                            CountScan++;


                        }




                    }
                    else
                    {
                        Logs logInner = new Logs();
                        logInner.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                        logInner.TimeStamp = DateTime.Now;
                        logInner.ActionName = "Exception";
                        logInner.Exeption = "DataError Tzahi";
                        Context.Logs.Add(logInner);

                        return -1;


                    }
                }
                return CountScan;

            }
            catch (Exception ex)
            {
                Logs log = new Logs();
                log.TableName = "MOTNotActiveWithOutDegemNoActiveWithDegem";
                log.TimeStamp = DateTime.Now;
                log.ActionName = "Exception";
                log.Exeption = "sssss" + ex.Message + ex.InnerException;

                Context.Logs.Add(log);

                return -1;


            }


        }

        private static void DBDeltaCheck(Context Context, MOTNotActiveWithOutDegem MOTNotActiveWithOutDegemObj)
        {



          

            TotalRowOver++;

            MOTNotActiveWithOutDegem CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOTNotActiveWithOutDegemObj.mispar_rechev).FirstOrDefault();


            DictionaryMot.TryGetValue(MOTNotActiveWithOutDegemObj.mispar_rechev,out CurrentCarInDB);
           
            //רכב חדש
            if (CurrentCarInDB == null)
            {
               // MOTNotActiveWithOutDegemObj.Active = false;
                // Context.MOTNotActiveWithOutDegem.Add(MOTNotActiveWithOutDegemObj);
                MOTNotActiveWithOutDegemNewList.Add(MOTNotActiveWithOutDegemObj);
                TotalAddNewCar++;
                Console.WriteLine("10)" + TotalRowOver.ToString() + "." + " Add New Not ActiveWithout Degem - " + MOTNotActiveWithOutDegemObj.mispar_rechev);

                return;
            }
            else
            {
              

              

                                
                    Console.WriteLine("10)" + TotalRowOver.ToString() + "." + " OverWithout Change - " + MOTNotActiveWithOutDegemObj.mispar_rechev);
                

            }

        }
    }
}
