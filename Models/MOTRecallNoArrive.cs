using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class MOTRecallNoArrive
    {
        public int Id { get; set; }

        public string RECALL_ID { get; set; }

        public int mispar_rechev { get; set; }

        
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
