using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class MOTRecall
    {
        public int Id { get; set; }

        public string RECALL_ID { get; set; }
        public string TOZAR_CD { get; set; }
        public string TOZAR_TEUR { get; set; }
        public string DEGEM { get; set; }
        public string SHNAT_RECALL { get; set; }
        public DateTime? BUILD_BEGIN_A { get; set; }
        public DateTime? BUILD_END_A { get; set; }
        public string SUG_RECALL { get; set; }
        public string SUG_TAKALA { get; set; }
        public string TEUR_TAKALA { get; set; }
        public string OFEN_TIKUN { get; set; }
        public string TKINA_EU { get; set; }
        public string YEVUAN_TEUR { get; set; }
        public string TELEPHONE { get; set; }
        public string WEBSITE { get; set; }

        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
