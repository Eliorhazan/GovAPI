using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public class GovMishkun
    {
        [Key]
        public int Id { get; set; }
        public int _id { get; set; }
        public int MishkunId { get; set; }

        public DateTime? DateRegister { get; set; }
        public string StatusRegister { get; set; }
        public string DateStatusRegister { get; set; }
        public DateTime? DateStatus { get; set; }
        public DateTime? DateNekes { get; set; }

        public DateTime? DateRemoveNekes { get; set; }
        public bool Active { get; set; } = true;

        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

    }

}