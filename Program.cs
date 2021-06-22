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

            if (IDSAPIArray.Contains("1"))
            {
                Mot4weelAPI ma1 = new Mot4weelAPI();
                ma1.RunAPI();

            }
            if (IDSAPIArray.Contains("2"))
            {
                MotCancelAPI ma2 = new MotCancelAPI();
                ma2.RunAPI();

            }

            if (IDSAPIArray.Contains("3"))
            {
                MotRecallAPI ma3 = new MotRecallAPI();
                ma3.RunAPI();

            }

            if (IDSAPIArray.Contains("4"))
            {
                MOTRecallNoArriveAPI ma4 = new MOTRecallNoArriveAPI();
                ma4.RunAPI();
            }

            if (IDSAPIArray.Contains("5"))
            {
                MotModelAPI ma5 = new MotModelAPI();
                ma5.RunAPI();

            }
            if (IDSAPIArray.Contains("6"))
            {
                MotTagAPI ma6 = new MotTagAPI();
                ma6.RunAPI();

            }
            if (IDSAPIArray.Contains("7"))
            {
                MotDealerAPI ma7 = new MotDealerAPI();
                ma7.RunAPI();
            }
            if (IDSAPIArray.Contains("8"))
            {
                MotYevuAPI ma8 = new MotYevuAPI();
                ma8.RunAPI();
            }

            if (IDSAPIArray.Contains("9"))
            {
                System.Threading.Thread.Sleep(5000);

                Console.WriteLine("Start Mot4weelNoActiveWithDegemAPI ---------------------");
                Mot4weelNoActiveWithDegemAPI ma9 = new Mot4weelNoActiveWithDegemAPI();
                ma9.RunAPI();
                Console.WriteLine("End Mot4weelNoActiveWithDegemAPI ---------------------");
            }

            if (IDSAPIArray.Contains("10"))
            {
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Start Mot4weelNoActiveWithOutDegemAPI ---------------------");
                Mot4weelNoActiveWithOutDegemAPI ma10 = new Mot4weelNoActiveWithOutDegemAPI();
                ma10.RunAPI();
                Console.WriteLine("End Mot4weelNoActiveWithOutDegemAPI ---------------------");
            }
            //Console.WriteLine("Stay Open");
            //Console.ReadLine();
        }
    }
}
