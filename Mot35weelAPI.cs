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
    class Mot35weelAPI
    {

        public static List<MOT35Wheels> DbMOT35WheelsList;

        public static Dictionary<int, MOT35Wheels> DictionaryMot = new Dictionary<int, MOT35Wheels>(); //

        public static List<int> DictionaryMotFromGovOrCsv = new List<int>(); //

        public static List<MOT35Wheels> MOT35WheelsNewList = new List<MOT35Wheels>();

        public static List<CarHoldingHistory> CarHoldingHistoryNewList = new List<CarHoldingHistory>();

        public static List<MOT35Wheels> MOT35WheelsChangeList = new List<MOT35Wheels>();



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



                    // IsFirstUse = ConfigurationManager.AppSettings.Get("IsFirstUse");
                    // אם קיים קישור בקונפיג
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOT35Wheels");

                    // כל הטבלה הקיימת כרגע
                    DbMOT35WheelsList = Context.MOT35Wheels.AsNoTracking().ToList();

                    DictionaryMot = DbMOT35WheelsList.ToDictionary(x => x.mispar_rechev, x => x);

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOT35Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT35Wheels from csv";

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
                                    MOT35Wheels MOT35WheelsFromCsv = GetMOT35WheelsObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, MOT35WheelsFromCsv);

                                }

                                //if (TotalRowOver > 0 && TotalRowOver % 100000 == 0)
                                //{
                                //    SaveMOT35WheelsNewList();
                                //}

                                if (TotalAddNewCar!=0 &&  TotalAddNewCar % 100000 == 0)
                                {
                                    SaveMOT35WheelsNewList();
                                }


                                if ((TotalChangeBaalut!=0 && TotalChangeBaalut % 50000 == 0) || (TotalChangeTokefDate != 0 && TotalChangeTokefDate % 50000 == 0))
                                {
                                    SaveMOT35WheelsChangeList();
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
                        log.TableName = "MOT35Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT35Wheels from gov...";

                        Context.Logs.Add(log);

                      


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "cd3acc5c-03c3-4c89-9c54-d40f93c0d790"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {


                            try
                            {

                                SaveMOT35WheelsNewList();
                                SaveMOT35WheelsChangeList();

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
                                logInner.TableName = "MOT35Wheels";
                                logInner.TimeStamp = DateTime.Now;
                                logInner.ActionName = "Exception";
                                logInner.Exeption = ex.Message + ex.InnerException;

                                Context.Logs.Add(logInner);


                            }

                        } while (responseObj > 0);



                    }

                    SaveMOT35WheelsNewList();
                    SaveMOT35WheelsChangeList();
                    SaveMOTNOTActive();
                    




                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOT35Wheels";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message + ex.InnerException;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOT35Wheels";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOT35Wheels";

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


                var ListNotActive = DbMOT35WheelsList.Where(x => !DictionaryMotFromGovOrCsv.Contains(x.mispar_rechev)).ToList();

                foreach (var CurrentCarInDB in ListNotActive)
                {
                    CurrentCarInDB.Active = -1;
                    Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                }



                Context.SaveChanges();


            }


            //using (var Context = new Context())
            //{



            //    Context.Configuration.AutoDetectChangesEnabled = false;
            //    Context.Configuration.ValidateOnSaveEnabled = false;

            //    foreach (var item in DictionaryMotFromGovOrCsv)
            //    {

            //        MOT35Wheels CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOT35WheelsObj.mispar_rechev).FirstOrDefault();
            //        DictionaryMot.TryGetValue(item, out CurrentCarInDB);

            //        if (CurrentCarInDB == null)
            //        {
            //            CurrentCarInDB.Active = -1;
            //            Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;
            //        }
            //    }


            //    Context.SaveChanges();


            //}

        }

        private void SaveMOT35WheelsNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOT35Wheels.AddRange(MOT35WheelsNewList);
                Context.SaveChanges();
                MOT35WheelsNewList.Clear();
            }

        }

        private void SaveMOT35WheelsChangeList()
        {

            using (var Context = new Context())
            {


                Context.CarHoldingHistory.AddRange(CarHoldingHistoryNewList);

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var item in MOT35WheelsChangeList)
                {
                    Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                Context.SaveChanges();
                CarHoldingHistoryNewList.Clear();
                MOT35WheelsChangeList.Clear();

            }
        }




        // public static MOT35Wheels MOT35WheelsFromCsvTemp = new MOT35Wheels();

        private static MOT35Wheels GetMOT35WheelsObj(string[] csvArray, string[] colHeader)
        {

            MOT35Wheels MOT35WheelsFromCsv = new MOT35Wheels();

            for (int i = 0; i < colHeader.Length; i++)
            {


                try
                {
                    var PropTypeName = Helper.GetTypeOfEntity(MOT35WheelsFromCsv, colHeader[i].ToString());

                    if (PropTypeName == "Int32")
                        MOT35WheelsFromCsv[colHeader[i].ToString()] = Helper.ConvertToInt(csvArray[i]);
                    else if (PropTypeName == "Nullable`1")
                        MOT35WheelsFromCsv[colHeader[i].ToString()] = Helper.ConvertToDatetime(csvArray[i]);

                    else
                        MOT35WheelsFromCsv[colHeader[i].ToString()] = csvArray[i].Replace("\"", "");

                }
                catch (Exception ex)
                {


                }

                // }
            }

            //MOT35Wheels MOT35WheelsFromCsv = new MOT35Wheels()
            //{

            //        mispar_rechev = Helper.ConvertToInt(csvArray[0]),
            //        tozeret_cd = csvArray[1].Replace("\"", ""),
            //        sug_degem = csvArray[2].Replace("\"", ""),
            //        tozeret_nm = csvArray[3].Replace("\"", ""),
            //        degem_cd = csvArray[4].Replace("\"", ""),
            //        degem_nm = csvArray[5].Replace("\"", ""),
            //        ramat_gimur = csvArray[6].Replace("\"", ""),
            //        ramat_eivzur_betihuty = csvArray[7].Replace("\"", ""),
            //        kvutzat_zihum = csvArray[8].Replace("\"", ""),
            //        shnat_yitzur = csvArray[9].Replace("\"", ""),
            //        degem_manoa = csvArray[10].Replace("\"", ""),
            //        mivchan_acharon_dt = Helper.ConvertToDatetime(csvArray[11]),
            //        tokef_dt = Helper.ConvertToDatetime(csvArray[12]),
            //        baalut = csvArray[13].Replace("\"", ""),
            //        misgeret = csvArray[14].Replace("\"", ""),
            //        tzeva_cd = Helper.ConvertToInt(csvArray[15]), // אין בקבצים
            //        tzeva_rechev = csvArray[16].Replace("\"", ""),
            //        zmig_kidmi = csvArray[17].Replace("\"", ""),
            //        zmig_ahori = csvArray[18].Replace("\"", ""),
            //        sug_delek_nm = csvArray[19].Replace("\"", ""),
            //        horaat_rishum = csvArray[20].Replace("\"", ""),
            //        moed_aliya_lakvish = csvArray[21].Replace("\"", ""),// אין בקבצים
            //        kinuy_mishari = csvArray[22].Replace("\"", "")

            //};




            return MOT35WheelsFromCsv;
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


                            MOT35Wheels MOT35WheelsFromGov = JsonConvert.DeserializeObject<MOT35Wheels>(x.ToString());

                            DBDeltaCheck(Context, MOT35WheelsFromGov);

                            CountScan++;


                        }




                    }
                    else
                    {
                        Logs logInner = new Logs();
                        logInner.TableName = "MOT35Wheels";
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
                log.TableName = "MOT35Wheels";
                log.TimeStamp = DateTime.Now;
                log.ActionName = "Exception";
                log.Exeption = "sssss" + ex.Message + ex.InnerException;

                Context.Logs.Add(log);

                return -1;


            }


        }

        private static void DBDeltaCheck(Context Context, MOT35Wheels MOT35WheelsObj)
        {



          

            TotalRowOver++;

            MOT35Wheels CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOT35WheelsObj.mispar_rechev).FirstOrDefault();


            DictionaryMot.TryGetValue(MOT35WheelsObj.mispar_rechev,out CurrentCarInDB);
           
            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.MOT35Wheels.Add(MOT35WheelsObj);
                MOT35WheelsNewList.Add(MOT35WheelsObj);
                TotalAddNewCar++;
                Console.WriteLine("12)" +TotalRowOver.ToString() + "." + " Add New - " + MOT35WheelsObj.mispar_rechev);

                return;
            }
            else
            {
               
                
                
                    Console.WriteLine("12)" + TotalRowOver.ToString() + "." + " OverWithout Change - " + MOT35WheelsObj.mispar_rechev);

                

            }


            // Context.SaveChanges();



        }
    }
}
