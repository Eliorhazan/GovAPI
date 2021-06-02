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
            
            
            Console.WriteLine("Stay Open");
            Console.ReadLine();
        }
    }
}
