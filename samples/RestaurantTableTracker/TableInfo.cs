using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantTableTracker
{
    public class TableInfo
    {
        public int TableId { get; set; }
        public string Status { get; set; }
    }

    public class TableStatus
    {
        public static readonly string empty = nameof(empty);
        public static readonly string occupied = nameof(occupied);
    }
}
