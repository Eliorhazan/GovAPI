using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GovAPI
{
    public partial class MOT35Wheels
    {
        public int Id { get; set; }
        public int mispar_rechev { get; set; }
        public string mispar_shilda { get; set; }
        public string tkina_EU { get; set; }
        public string tozeret_cd { get; set; }
        public string tozeret_nm { get; set; }
        public string shnat_yitzur { get; set; }
        public string tozeret_eretz_nm { get; set; }
        public int? sug_delek_cd { get; set; }
        public string sug_delek_nm { get; set; }
        public int? mishkal_kolel { get; set; }
        public int? mishkal_azmi { get; set; }

        public string degem_nm { get; set; }

        public string nefach_manoa { get; set; }

        public string degem_manoa { get; set; }
        public string hanaa_cd { get; set; }

        public string hanaa_nm { get; set; }

        public int? mishkal_mitan_harama { get; set; }
        public int? horaat_rishum { get; set; }



        public int Active { get; set; } =  1;
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }


    }
}
