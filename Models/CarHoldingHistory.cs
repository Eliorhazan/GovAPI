using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class CarHoldingHistory
    {
        public int Id { get; set; }

        public int mispar_rechev { get; set; }

        public DateTime LastScanDate { get; set; }
        public DateTime? mivchan_acharon_dt { get; set; }
        public DateTime? tokef_dt { get; set; }
        public string baalut { get; set; }
    }
}
