using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class MOTTags
    {
        public int Id { get; set; }
        public double? MISPAR_RECHEV { get; set; }
        public double? TAARICH_HAFAKAT_TAG { get; set; }
        public double? SUG_TAV { get; set; }
    }
}
