using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public class MOTCancel
    {
        [Key]
        public int Id { get; set; }
        public int mispar_rechev { get; set; }
       
        public string tozeret_cd { get; set; }
        public string tozeret_nm { get; set; }
    
        public string degem_cd { get; set; }
        public string degem_nm { get; set; }

        public int? sug_rechev_cd { get; set; }
        public string sug_rechev_nm { get; set; }
        public string moed_aliya_lakvish { get; set; }
        public DateTime? bitul_dt { get; set; }
        public string misgeret { get; set; }


        public string tozar_manoa { get; set; }
        public string degem_manoa { get; set; }
        public string mispar_manoa { get; set; }

        public string mishkal_kolel { get; set; }

        public string ramat_gimur { get; set; }
        public string ramat_eivzur_betihuty { get; set; }
        public string kvutzat_zihum { get; set; }
        public string shnat_yitzur { get; set; }
      
      
        public string baalut { get; set; }
      
        public string tzeva_rechev { get; set; }

        public int? tzeva_cd { get; set; }

        public string zmig_kidmi { get; set; }
        public string zmig_ahori { get; set; }
        public string sug_delek_nm { get; set; }
        public string horaat_rishum { get; set; }
        public string kinuy_mishari { get; set; }
     






        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }

}