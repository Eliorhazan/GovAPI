﻿using Microsoft.VisualBasic.FileIO;
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
    class MOTRecallNoArriveAPI
    {

        public static List<MOTRecallNoArrive> DbMOTRecallNoArriveList;

        public static List<MOTRecallNoArrive> MOTRecallNoArriveNewList = new List<MOTRecallNoArrive>();

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
                    string CsvLink = ConfigurationManager.AppSettings.Get("MOTRecallNoArrive");

                    // כל הטבלה הקיימת כרגע
                    DbMOTRecallNoArriveList = Context.MOTRecallNoArrive.AsNoTracking().ToList();

                    var All = Context.MOTRecallNoArrive;
                    Context.MOTRecallNoArrive.RemoveRange(All);
                    Context.SaveChanges();

                    // מגיע מcsv  
                    if (!string.IsNullOrEmpty(CsvLink))
                    {

                        Logs log = new Logs();
                        log.TableName = "MOTRecallNoArrive";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTRecallNoArrive from csv";

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
                                    MOTRecallNoArrive MOT4WheelsFromCsv = GetMOT4WheelsObj(csvArray, colHeader);

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
                        log.TableName = "MOTRecallNoArrive";
                        log.TimeStamp = DateTime.Now;
                        log.ActionName = " Start Download MOTRecallNoArrive from gov...";

                        Context.Logs.Add(log);


                        //// Initilization.  
                        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
                        string requestParams = string.Empty;

                        // Converting Request Params to Key Value Pair.  
                        allIputParams.Add(new KeyValuePair<string, string>("resource_id", "36bf1404-0be4-49d2-82dc-2f1ead4a8b93"));
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
                    log.TableName = "MOTRecallNoArrive";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = "Exception";
                    log.Exeption = ex.Message;

                    Context.Logs.Add(log);



                }
                finally
                {

                    Logs log = new Logs();
                    log.TableName = "MOTRecallNoArrive";
                    log.TimeStamp = DateTime.Now;
                    log.ActionName = " End Download MOTRecallNoArrive";

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

                Context.MOTRecallNoArrive.AddRange(MOTRecallNoArriveNewList);
                Context.SaveChanges();
                MOTRecallNoArriveNewList.Clear();
            }

        }




        public MOTRecallNoArrive MOT4WheelsFromCsvTemp = new MOTRecallNoArrive();

        private MOTRecallNoArrive GetMOT4WheelsObj(string[] csvArray, string[] colHeader)
        {

            MOTRecallNoArrive MOT4WheelsFromCsv = MOT4WheelsFromCsvTemp;

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


                        MOTRecallNoArrive MOT4WheelsFromGov = JsonConvert.DeserializeObject<MOTRecallNoArrive>(x.ToString());

                        DBDeltaCheck(Context, MOT4WheelsFromGov);

                        CountScan++;


                    }



                }
            }
            return CountScan;

        }

        private static void DBDeltaCheck(Context Context, MOTRecallNoArrive MOTRecallNoArriveObj)
        {

            TotalRowOver++;

            //var CurrentCarInDB = DbMOTRecallNoArriveList.Where(m => m.RECALL_ID == MOT4WheelsObj.RECALL_ID && m.mispar_rechev == MOT4WheelsObj.mispar_rechev).FirstOrDefault();

            ////רכב חדש
            //if (CurrentCarInDB == null)
            //{
            // Context.MOTRecallNoArrive.Add(MOT4WheelsObj);


            MOTRecallNoArriveNewList.Add(MOTRecallNoArriveObj);
            TotalAddNewCar++;
            Console.WriteLine("4)" + TotalRowOver.ToString() + "." + " Add New - " + MOTRecallNoArriveObj.mispar_rechev);

            //Context.SaveChanges();
            //  }





        }
    }
}


