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
    class GovMishkunAPI
    {

        public static List<GovMishkun> DbGovMishkunList;

       public static Dictionary<int, GovMishkun> DictionaryMot = new Dictionary<int, GovMishkun>(); //

     //   public static List<int> DictionaryMotFromGovOrCsv = new List<int>(); //

        public static List<GovMishkun> GovMishkunNewList = new List<GovMishkun>();

        public static List<CarHoldingHistory> CarHoldingHistoryNewList = new List<CarHoldingHistory>();

        public static List<GovMishkun> GovMishkunChangeList = new List<GovMishkun>();



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
                    string CsvLink = ConfigurationManager.AppSettings.Get("GovMishkun");

                    // כל הטבלה הקיימת כרגע
                    DbGovMishkunList = Context.GovMishkun.AsNoTracking().ToList();

                    DictionaryMot = DbGovMishkunList.ToDictionary(x => x.MishkunId, x => x);

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "GovMishkun";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download GovMishkun from csv";

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
                                    GovMishkun GovMishkunFromCsv = GetGovMishkunObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, GovMishkunFromCsv);

                                }

                                //if (TotalRowOver > 0 && TotalRowOver % 100000 == 0)
                                //{
                                //    SaveGovMishkunNewList();
                                //}

                                if (TotalAddNewCar!=0 &&  TotalAddNewCar % 100000 == 0)
                                {
                                    SaveGovMishkunNewList();
                                }


                                if ((TotalChangeBaalut!=0 && TotalChangeBaalut % 50000 == 0) || (TotalChangeTokefDate != 0 && TotalChangeTokefDate % 50000 == 0))
                                {
                                    SaveGovMishkunChangeList();
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
                        log.TableName = "GovMishkun";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download GovMishkun from gov...";

                        Context.Logs.Add(log);

                      


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "e7266a9c-fed6-40e4-a28e-8cddc9f44842"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {


                            try
                            {

                                SaveGovMishkunNewList();
                                SaveGovMishkunChangeList();

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
                                logInner.TableName = "GovMishkun";
                                logInner.TimeStamp = DateTime.Now;
                                logInner.ActionName = "Exception";
                                logInner.Exeption = ex.Message + ex.InnerException;

                                Context.Logs.Add(logInner);


                            }

                        } while (responseObj > 0);



                    }

                    SaveGovMishkunNewList();
                    SaveGovMishkunChangeList();
                   // SaveMOTNOTActive();
                    




                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "GovMishkun";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message + ex.InnerException;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "GovMishkun";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download GovMishkun";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);


                    Context.SaveChanges();

                }

            }



        }

        //private void SaveMOTNOTActive()
        //{
         

        //    using (var Context = new Context())
        //    {


              
        //        Context.Configuration.AutoDetectChangesEnabled = false;
        //        Context.Configuration.ValidateOnSaveEnabled = false;

        //        foreach (var item in DictionaryMotFromGovOrCsv)
        //        {

        //            GovMishkun CurrentCarInDB;//   .Where(m => m.mispar_rechev == GovMishkunObj.mispar_rechev).FirstOrDefault();
        //            DictionaryMot.TryGetValue(item, out CurrentCarInDB);

        //            if (CurrentCarInDB == null)
        //            {
        //                CurrentCarInDB.Active = -1;
        //                Context.Entry(CurrentCarInDB).State = System.Data.Entity.EntityState.Modified;
        //            }
        //        }


        //        Context.SaveChanges();
               

        //    }

        //}

        private void SaveGovMishkunNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.GovMishkun.AddRange(GovMishkunNewList);
                Context.SaveChanges();
                GovMishkunNewList.Clear();
            }

        }

        private void SaveGovMishkunChangeList()
        {

            using (var Context = new Context())
            {


                Context.CarHoldingHistory.AddRange(CarHoldingHistoryNewList);

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                foreach (var item in GovMishkunChangeList)
                {
                    Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }


                Context.SaveChanges();
                CarHoldingHistoryNewList.Clear();
                GovMishkunChangeList.Clear();

            }
        }




        // public static GovMishkun GovMishkunFromCsvTemp = new GovMishkun();

        private static GovMishkun GetGovMishkunObj(string[] csvArray, string[] colHeader)
        {

            GovMishkun GovMishkunFromCsv = new GovMishkun();

            for (int i = 0; i < colHeader.Length; i++)
            {


                try
                {
                    var PropTypeName = Helper.GetTypeOfEntity(GovMishkunFromCsv, colHeader[i].ToString());

                    if (PropTypeName == "Int32")
                        GovMishkunFromCsv[colHeader[i].ToString()] = Helper.ConvertToInt(csvArray[i]);
                    else if (PropTypeName == "Nullable`1")
                        GovMishkunFromCsv[colHeader[i].ToString()] = Helper.ConvertToDatetime(csvArray[i]);

                    else
                        GovMishkunFromCsv[colHeader[i].ToString()] = csvArray[i].Replace("\"", "");

                }
                catch (Exception ex)
                {


                }

                // }
            }

            //GovMishkun GovMishkunFromCsv = new GovMishkun()
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




            return GovMishkunFromCsv;
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
                            GovMishkun GovMishkunFromGov = new GovMishkun();

                            GovMishkunFromGov.MishkunId = Helper.ConvertToInt(x["מזהה משכון"].ToString());
                            GovMishkunFromGov.DateRegister = Helper.ConvertToDatetime(x["תאריך רישום"].ToString());
                            GovMishkunFromGov.StatusRegister = x["סטטוס רישום"].ToString();
                            GovMishkunFromGov.DateStatusRegister = x["תאריך סיבת סטטוס"].ToString();
                            GovMishkunFromGov.DateStatus = Helper.ConvertToDatetime(x["תאריך סטטוס"].ToString());
                            GovMishkunFromGov.DateNekes = Helper.ConvertToDatetime(x["תאריך רישום נכס"].ToString());
                            GovMishkunFromGov.DateRemoveNekes = Helper.ConvertToDatetime(x["תאריך הסרת הנכס"].ToString());


                            DBDeltaCheck(Context, GovMishkunFromGov);

                            CountScan++;


                        }




                    }
                    else
                    {
                        Logs logInner = new Logs();
                        logInner.TableName = "GovMishkun";
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
                log.TableName = "GovMishkun";
                log.TimeStamp = DateTime.Now;
                log.ActionName = "Exception";
                log.Exeption = "sssss" + ex.Message + ex.InnerException;

                Context.Logs.Add(log);

                return -1;


            }


        }

        private static void DBDeltaCheck(Context Context, GovMishkun GovMishkunObj)
        {



          

            TotalRowOver++;

            GovMishkun CurrentCarInDB;//   .Where(m => m.mispar_rechev == GovMishkunObj.mispar_rechev).FirstOrDefault();


            DictionaryMot.TryGetValue(GovMishkunObj.MishkunId,out CurrentCarInDB);
           
            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.GovMishkun.Add(GovMishkunObj);
                GovMishkunNewList.Add(GovMishkunObj);
                TotalAddNewCar++;
                Console.WriteLine("13)" + TotalRowOver.ToString() + "." + " Add New - " + GovMishkunObj.MishkunId);

                return;
            }
            else
            {
             //   DictionaryMotFromGovOrCsv.Add(GovMishkunObj.mispar_rechev);

               

                //if (CurrentCarInDB.Active!=1)
                //{



                   
                //    CurrentCarInDB.Active = 1;

                //    GovMishkunChangeList.Add(CurrentCarInDB);

                //    Console.WriteLine("11)" + TotalRowOver.ToString() + "." + " Change  - ActiveCar -" + GovMishkunObj.mispar_rechev);


                //}

                //else
                //{
                    Console.WriteLine("13)" + TotalRowOver.ToString() + "." + " OverWithout Change - " + GovMishkunObj.MishkunId);

               // }

            }


           



        }
    }
}
