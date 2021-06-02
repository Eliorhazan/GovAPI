using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class CarDealers
    {
        [Key]
        public int IDCarDealers { get; set; }
        public string Name { get; set; }
        public string yishuv { get; set; }
        public string mikud { get; set; }
        public string ktovet { get; set; }
        public string PersonalID { get; set; }
        public string CompanyID { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Comments { get; set; }
    }
}
