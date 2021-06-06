﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class MOTTags
    {
        public int Id { get; set; }

        [Column("MISPAR RECHEV")]
        public int MISPAR_RECHEV { get; set; }
        [Column("TAARICH HAFAKAT TAG")]
        public string TAARICH_HAFAKAT_TAG { get; set; }
        [Column("SUG TAV")]
        public string SUG_TAV { get; set; }

        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
