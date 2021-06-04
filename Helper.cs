using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GovAPI
{
    public static class Helper
    {

        public static int ConvertToInt(string Value)
        {
            int res;
            bool IsOk = Int32.TryParse(Value.Replace("\"", ""), out res);
            if (!IsOk)
                return 0;

            else

                return res;
        }

        public static double ConvertToDoble(string Value)
        {
            double res;
            bool IsOk = double.TryParse(Value, out res);
            if (!IsOk)
                return 0;

            else

                return res;
        }



        public static DateTime? ConvertToDatetime(string Value)
        {
            DateTime res;
            bool IsOk = DateTime.TryParse(Value.Replace("\"", ""), out res);
            if (!IsOk)
                return null;

            else

                return res;
        }

        public static string GetTypeOfEntity(object data,string PropertyName)
        {

        

            string TypeOF = data.GetType().GetProperties().Where(x=>x.Name== PropertyName).FirstOrDefault().PropertyType.Name;

            //foreach (PropertyInfo propertyInfo in data.GetType().GetProperties())
            //{
            //    string res = propertyInfo.GetType().Name;



            //    //if (propertyInfo.PropertyType == typeof(string))
            //    //{
            //    //    string value = propertyInfo.GetValue(data, null);


            //    //}
            //}


            return TypeOF;
        }            

    


}
}
