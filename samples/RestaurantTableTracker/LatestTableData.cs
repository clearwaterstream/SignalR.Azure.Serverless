using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantTableTracker
{
    public class LatestTableData : List<TableInfo>
    {
        public LatestTableData()
        {
            for(int i = 1; i < 11; i++)
            {
                Add(new TableInfo() { TableId = i, Status = "empty" });
            }

            this[1].Status = TableStatus.occupied;
            this[3].Status = TableStatus.occupied;
            this[4].Status = TableStatus.occupied;
            this[7].Status = TableStatus.occupied;
        }
    }
}
