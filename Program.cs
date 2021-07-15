using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovAPI
{
    class Program
    {
        static void Main(string[] args)
        {


            string IDSAPI = ConfigurationManager.AppSettings.Get("IDSAPI");
            string[] IDSAPIArray = IDSAPI.Split(',');
            if (string.IsNullOrEmpty(IDSAPI))
            {
                IDSAPIArray = args[0].Split(',');
            }


            if (IDSAPIArray.Contains("1"))
            {
                Mot4weelAPI ma1 = new Mot4weelAPI();
                ma1.RunAPI();
                ma1 = null;

            }
            if (IDSAPIArray.Contains("2"))
            {
                MotCancelAPI ma2 = new MotCancelAPI();
                ma2.RunAPI();
                ma2 = null;

            }

            if (IDSAPIArray.Contains("3"))
            {
                MotRecallAPI ma3 = new MotRecallAPI();
                ma3.RunAPI();
                ma3 = null;

            }

            if (IDSAPIArray.Contains("4"))
            {
                MOTRecallNoArriveAPI ma4 = new MOTRecallNoArriveAPI();
                ma4.RunAPI();
                ma4 = null;
            }

            if (IDSAPIArray.Contains("5"))
            {
                MotModelAPI ma5 = new MotModelAPI();
                ma5.RunAPI();
                ma5 = null;

            }
            if (IDSAPIArray.Contains("6"))
            {
                MotTagAPI ma6 = new MotTagAPI();
                ma6.RunAPI();
                ma6 = null;

            }
            if (IDSAPIArray.Contains("7"))
            {
                MotDealerAPI ma7 = new MotDealerAPI();
                ma7.RunAPI();
                ma7 = null;
            }
            if (IDSAPIArray.Contains("8"))
            {
                MotYevuAPI ma8 = new MotYevuAPI();
                ma8.RunAPI();
                ma8 = null;
            }

            if (IDSAPIArray.Contains("9"))
            {
                System.Threading.Thread.Sleep(5000);

                Console.WriteLine("Start Mot4weelNoActiveWithDegemAPI ---------------------");
                Mot4weelNoActiveWithDegemAPI ma9 = new Mot4weelNoActiveWithDegemAPI();
                ma9.RunAPI();
                ma9 = null;
                Console.WriteLine("End Mot4weelNoActiveWithDegemAPI ---------------------");
            }

            if (IDSAPIArray.Contains("10"))
            {
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Start Mot4weelNoActiveWithOutDegemAPI ---------------------");
                Mot4weelNoActiveWithOutDegemAPI ma10 = new Mot4weelNoActiveWithOutDegemAPI();
                ma10.RunAPI();
                ma10 = null;
                Console.WriteLine("End Mot4weelNoActiveWithOutDegemAPI ---------------------");
            }

            if (IDSAPIArray.Contains("11"))
            {
                Mot2weelAPI ma11 = new Mot2weelAPI();
                ma11.RunAPI();
                ma11 = null;
            }

            if (IDSAPIArray.Contains("12"))
            {
                Mot35weelAPI ma12 = new Mot35weelAPI();
                ma12.RunAPI();
                ma12 = null;
            }
            if (IDSAPIArray.Contains("13"))
            {
                GovMishkunAPI ma13 = new GovMishkunAPI();
                ma13.RunAPI();
                ma13 = null;
            }
            //Console.WriteLine("Stay Open");
            //Console.ReadLine();
        }
    }
}
