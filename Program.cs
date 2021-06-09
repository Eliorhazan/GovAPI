using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovAPI
{
    class Program
    {
        static void Main(string[] args)
        {

            Mot4weelAPI ma = new Mot4weelAPI();
            ma.RunAPI();

            //MotCancelAPI ma = new MotCancelAPI();
            //ma.RunAPI();

            //MotRecallAPI ma = new MotRecallAPI();
            //ma.RunAPI();

            //MOTRecallNoArriveAPI ma = new MOTRecallNoArriveAPI();
            //ma.RunAPI();

            //MotModelAPI ma = new MotModelAPI();
            //ma.RunAPI();

            //MotTagAPI ma = new MotTagAPI();
            //ma.RunAPI();


            Console.WriteLine("Stay Open");
            Console.ReadLine();
        }
    }
}
