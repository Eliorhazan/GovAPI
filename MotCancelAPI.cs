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
    class MotCancelAPI
    {

        public static List<MOTCancel> DbMOTCancelList;

        public static List<MOTCancel> MOTCancelNewList = new List<MOTCancel>();

        public static Dictionary<int, MOTCancel> DictionaryMOTCancel = new Dictionary<int, MOTCancel>(); //

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
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTCancel");

                    // כל הטבלה הקיימת כרגע
                    DbMOTCancelList = Context.MOTCancel.AsNoTracking().ToList();

                    DictionaryMOTCancel = DbMOTCancelList.ToDictionary(x => x.mispar_rechev, x => x);


                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOTCancel";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTCancel from csv";

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
                                    MOTCancel MOTCancelFromCsv = GetMOTCancelObj(csvArray, colHeader);

                                    DBDeltaCheck(Context, MOTCancelFromCsv);
                                  

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
                        log.TableName = "MOTCancel";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTCancel from gov...";

                        Context.Logs.Add(log);


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "851ecab1-0622-4dbe-a6c7-f950cf82abf9"));
                        allIputParams.Add(new KeyValuePair<string, string>("limit", "100000"));
                        allIputParams.Add(new KeyValuePair<string, string>("offset", "0"));
                        // URL Request Query parameters.  



                        int responseObj = 0;
                        int CountOffset = 0;

                        do
                        {
                            SaveMOTCancelNewList();
                          //  SaveMOTCancelChangeList();

                            allIputParams.RemoveAll(x => x.Key == "offset");
                            allIputParams.Add(new KeyValuePair<string, string>("offset", CountOffset.ToString()));

                            requestParams = new FormUrlEncodedContent(allIputParams).ReadAsStringAsync().Result;
                            // Call REST Web API with parameters.  
                            responseObj = GetInfo(requestParams, Context).Result;

                            if (responseObj == -1) responseObj = 2;

                             CountOffset = CountOffset + 100000;

                        } while (responseObj > 0);


                    }

                    SaveMOTCancelNewList();


                }
                catch (Exception ex)
                {
                    Logs log = new Logs();
                    log.TableName = "MOTCancel";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message  + ex.InnerException;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOTCancel";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOTCancel";

                    log.TotalAddNewRow = TotalAddNewCar;
                    log.TotalChange1 = TotalChangeBaalut;
                    log.TotalChange2 = TotalChangeTokefDate;

                    Context.Logs.Add(log);




                    Context.SaveChanges();

                }

            }



        }

       private void SaveMOTCancelNewList()
        {
            using (var Context = new Context())
            {

                Context.Configuration.AutoDetectChangesEnabled = false;
                Context.Configuration.ValidateOnSaveEnabled = false;

                Context.MOTCancel.AddRange(MOTCancelNewList);
                Context.SaveChanges();
                MOTCancelNewList.Clear();
            }

        }

       public MOTCancel MOTCancelFromCsvTemp = new MOTCancel();

       private MOTCancel GetMOTCancelObj(string[] csvArray, string[] colHeader)
        {

            MOTCancel MOTCancelFromCsv = MOTCancelFromCsvTemp;

            for (int i = 0; i < colHeader.Length; i++)
            {


                try
                {
                    var PropTypeName = Helper.GetTypeOfEntity(MOTCancelFromCsv, colHeader[i].ToString());

                    if (PropTypeName == "Int32")
                        MOTCancelFromCsv[colHeader[i].ToString()] = Helper.ConvertToInt(csvArray[i]);
                    else if (PropTypeName == "Nullable`1")
                        MOTCancelFromCsv[colHeader[i].ToString()] = Helper.ConvertToDatetime(csvArray[i]);

                    else
                        MOTCancelFromCsv[colHeader[i].ToString()] = csvArray[i].Replace("\"", "");

                }
                catch (Exception ex)
                {


                }

                // }
            }

            //MOTCancel MOTCancelFromCsv = new MOTCancel()
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




            return MOTCancelFromCsv;
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


                        // Console.WriteLine(TotalRowOver.ToString() + "." + " No New - ");

                        //try
                        //{

                        MOTCancel MOTCancelFromGov = JsonConvert.DeserializeObject<MOTCancel>(x.ToString());
                        DBDeltaCheck(Context, MOTCancelFromGov);
                        //}
                        //catch(Exception ex)
                        //{



                        //}


                        CountScan++;


                    }

                    
                    

                }
                else
                    return -1;
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, MOTCancel MOTCancelObj)
        {

            TotalRowOver++;

           // var CurrentCarInDB = DbMOTCancelList.Where(m => m.tozeret_nm == MOTCancelObj.tozeret_nm && m.degem_nm == MOTCancelObj.degem_nm && m.shnat_yitzur == MOTCancelObj.shnat_yitzur).FirstOrDefault();

            MOTCancel CurrentCarInDB;//   .Where(m => m.mispar_rechev == MOTCancelObj.mispar_rechev).FirstOrDefault();


            DictionaryMOTCancel.TryGetValue(MOTCancelObj.mispar_rechev, out CurrentCarInDB);


            //רכב חדש
            if (CurrentCarInDB == null)
            {
                // Context.MOTCancel.Add(MOTCancelObj);
                MOTCancelNewList.Add(MOTCancelObj);
                TotalAddNewCar++;
                Console.WriteLine("2)" + TotalRowOver.ToString() + "." + " Add New - " + MOTCancelObj.mispar_rechev);
              
               

            }
            else
            {

                Console.WriteLine("2)" + TotalRowOver.ToString() + "." + " No New - ");

            }





        }
    }
}
