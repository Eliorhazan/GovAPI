using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GovAPI
{
    public partial class MOTYevu
    {
        public int Id { get; set; }

        public int mispar_rechev { get; set; }
        public string shilda { get; set; }
        public string tozeret_cd { get; set; }
        public string tozeret_nm { get; set; }
        public string sug_rechev_cd { get; set; }
        public string sug_rechev_nm { get; set; }
        public string degem_nm { get; set; }
        public string mishkal_kolel { get; set; }
        public string shnat_yitzur { get; set; }
        public string nefach_manoa { get; set; }
        public string tozeret_eretz_nm { get; set; }
        public string degem_manoa { get; set; }
        public DateTime? mivchan_acharon_dt { get; set; }
        public DateTime? tokef_dt { get; set; }
        public string sug_yevu { get; set; }
        public string moed_aliya_lakvish { get; set; }
       

        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }


    }
}
