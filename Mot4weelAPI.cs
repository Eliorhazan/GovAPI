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
    class Mot4weelAPI
    {

        public static List<MOT4Wheels> DbMOT4WheelsList;

        public static Dictionary<int, MOT4Wheels> DictionaryMot = new Dictionary<int, MOT4Wheels>(); //

        public static List<MOT4Wheels> MOT4WheelsNewList = new List<MOT4Wheels>();

        public static List<CarHoldingHistory> CarHoldingHistoryNewList = new List<CarHoldingHistory>();

        public static List<MOT4Wheels> MOT4WheelsChangeList = new List<MOT4Wheels>();



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
                    string CsvLink = ConfigurationManager.AppSettings.Get("Mot4wheels");

                    // כל הטבלה הקיימת כרגע
                    DbMOT4WheelsList = Context.MOT4Wheels.AsNoTracking().ToList();

                    DictionaryMot = DbMOT4WheelsList.ToDictionary(x => x.mispar_rechev, x => x);

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOT4Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT4Wheels from csv";

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
                                    MOT4Wheels MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, MOT4WheelsFromCsv);

                                }

                                //if (TotalRowOver > 0 && TotalRowOver % 100000 == 0)
                                //{
                                //    SaveMOT4WheelsNewList();
                                //}

                                if (TotalAddNewCar!=0 &&  TotalAddNewCar % 100000 == 0)
                                {
                                    SaveMOT4WheelsNewList();
                                }


                                if ((TotalChangeBaalut!=0 && TotalChangeBaalut % 50000 == 0) || (TotalChangeTokefDate != 0 && TotalChangeTokefDate % 50000 == 0))
                                {
                                    SaveMOT4WheelsChangeList();
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
                        log.TableName = "MOT4Wheels";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOT4Wheels from gov...";

                        Context.Logs.Add(log);

                      


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "053cea08-09bc-40ec-8f7a-156f0677aff3"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {


                            try
                            {

                                SaveMOT4WheelsNewList();
                                SaveMOT4WheelsChangeList();

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
                                logInner.TableName = "MOT4Wheels";
                                logInner.TimeStamp = DateTime.Now;
                                logInner.ActionName = "Exception";
                                logInner.Exeption = ex.Message + ex.InnerException;

                                Context.Logs.Add(logInner);


                            }

                        } while (responseObj > 0);



                    }

                    SaveMOT4WheelsNewList();
                    SaveMOT4WheelsChangeList();
                    // Context.MOT4Wheels.AddRange(MOT4WheelsNewList);




                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOT4Wheels";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message + ex.InnerException;

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

        private void SaveMOT4WheelsNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOT4Wheels.AddRange(MOT4WheelsNewList);
                Context.SaveChanges();
                MOT4WheelsNewList.Clear();
            }

        }

        private void SaveMOT4WheelsChangeList()
        {

            using (var Context = new Context())
            {


                Context.CarHoldingHistory.AddRange(CarHoldingHistoryNewList);

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var item in MOT4WheelsChangeList)
                {
                    Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                Context.SaveChanges();
                CarHoldingHistoryNewList.Clear();
                MOT4WheelsChangeList.Clear();

            }
        }




        // public static MOT4Wheels MOT4WheelsFromCsvTemp = new MOT4Wheels();

        private static MOT4Wheels GetMOT4WheelsObj(string[] csvArray, string[] colHeader)
        {

            MOT4Wheels MOT4WheelsFromCsv = new MOT4Wheels();

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

            //MOT4Wheels MOT4WheelsFromCsv = new MOT4Wheels()
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




            return MOT4WheelsFromCsv;
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


                            MOT4Wheels MOT4WheelsFromGov = JsonConvert.DeserializeObject<MOT4Wheels>(x.ToString());

                            DBDeltaCheck(Context, MOT4WheelsFromGov);

                            CountScan++;


                        }




                    }
                    else
                    {
                        Logs logInner = new Logs();
                        logInner.TableName = "MOT4Wheels";
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
                log.TableName = "MOT4Wheels";
                log.TimeStamp = DateTime.Now;
                log.ActionName = "Exception";
                log.Exeption = "sssss" + ex.Message + ex.InnerException;

                Context.Logs.Add(log);

                return -1;


            }


        }

        private static void DBDeltaCheck(Context Context, MOT4Wheels MOT4WheelsObj)
        {

           
            TotalRowOver++;

            MOT4Wheels CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOT4WheelsObj.mispar_rechev).FirstOrDefault();


            DictionaryMot.TryGetValue(MOT4WheelsObj.mispar_rechev,out CurrentCarInDB);
            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.MOT4Wheels.Add(MOT4WheelsObj);
                MOT4WheelsNewList.Add(MOT4WheelsObj);
                TotalAddNewCar++;
                Console.WriteLine(TotalRowOver.ToString() + "." + " Add New - " + MOT4WheelsObj.mispar_rechev);

                return;
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
                    CarHoldingHistoryNewList.Add(ch);
                    // Context.CarHoldingHistory.Add(ch);


                    // עדכון בעלות לחדש
                    CurrentCarInDB.baalut = MOT4WheelsObj.baalut;
                    //  Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                    MOT4WheelsChangeList.Add(CurrentCarInDB);

                    TotalChangeBaalut++;
                    Console.WriteLine(TotalRowOver.ToString() + "." + " Change Baalut - " + MOT4WheelsObj.mispar_rechev);

                }


                else if (MOT4WheelsObj.tokef_dt != CurrentCarInDB.tokef_dt)
                {


                    CarHoldingHistory ch = new CarHoldingHistory()
                    {
                        mispar_rechev = CurrentCarInDB.mispar_rechev,
                        tokef_dt = CurrentCarInDB.tokef_dt,
                        mivchan_acharon_dt = CurrentCarInDB.mivchan_acharon_dt,
                        LastScanDate = DateTime.Now
                    };


                    CarHoldingHistoryNewList.Add(ch);
                    // Context.CarHoldingHistory.Add(ch);


                    // עדכון תוקף לחדש
                    CurrentCarInDB.tokef_dt = MOT4WheelsObj.tokef_dt;
                    CurrentCarInDB.mivchan_acharon_dt = MOT4WheelsObj.mivchan_acharon_dt;
                    // Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;

                    MOT4WheelsChangeList.Add(CurrentCarInDB);



                    TotalChangeTokefDate++;
                    Console.WriteLine(TotalRowOver.ToString() + "." + " Change TokefDate - " + MOT4WheelsObj.mispar_rechev);


                }
                else
                {

                    Console.WriteLine(TotalRowOver.ToString() + "." + " OverWithout Change - " + MOT4WheelsObj.mispar_rechev);


                }

            }


            // Context.SaveChanges();



        }
    }
}
