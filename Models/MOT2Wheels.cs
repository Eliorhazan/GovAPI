using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GovAPI
{
    public partial class MOT2Wheels
    {
        public int Id { get; set; }
        public int mispar_rechev { get; set; }
        public string tozeret_cd { get; set; }
        public string tozeret_nm { get; set; }
        public string tozeret_eretz_nm { get; set; }
        public string degem_nm { get; set; }
        public string shnat_yitzur { get; set; }
        public int? sug_delek_cd { get; set; }
        public string sug_delek_nm { get; set; }
        public int? mishkal_kolel { get; set; }
        public string mida_zmig_kidmi { get; set; }
        public string mida_zmig_ahori { get; set; }
        public int? kod_omes_zmig_kidmi { get; set; }
        public int? kod_omes_zmig_ahori { get; set; }
        public string kod_mehirut_zmig_kidmi { get; set; }
        public string kod_mehirut_zmig_ahori { get; set; }
        public int? nefach_manoa { get; set; }
        public double? hespek { get; set; }
        public string misgeret { get; set; }

        public int Active { get; set; } =  1;
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }


    }
}
