using System;
using SQLite;

namespace WarrantyWarden.Models
{
    public class Warranty
    {
        [PrimaryKey, AutoIncrement] 
        public int ID { get; set; }
        public string Product { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; }
        public string LargestUnit { get; set; }
        public int LargestUnitRemaining { get; set; }
        public int Priority { get; set; }

    }
}
