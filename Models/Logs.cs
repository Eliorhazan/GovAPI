using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public class Logs
    {
        [Key]
        public int Id { get; set; }
    
        public string Exeption { get; set; }

        public string TableName { get; set; }
        public string ActionName { get; set; }

        public DateTime? TimeStamp { get; set; }


        public int? TotalAddNewRow { get; set; }

        public int? TotalChange1 { get; set; }

        public int? TotalChange2 { get; set; }




    }

}