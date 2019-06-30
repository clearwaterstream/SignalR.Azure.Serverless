using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestaurantTableTracker
{
    /// <summary>
    /// Interaction logic for TableStatus.xaml
    /// </summary>
    public partial class TableStatusControl : UserControl
    {
        readonly Brush defaultLblBg;
        readonly Brush defaultLblFg;

        string status;
        int tableId;

        public event Func<int, string, Task> StatusChanged;

        public TableStatusControl()
        {
            InitializeComponent();

            defaultLblBg = lblTableName.Background;
            defaultLblFg = lblTableName.Foreground;
        }

        public int TableId
        {
            get
            {
                return tableId;
            }
            set
            {
                tableId = value;

                lblTableName.Content = $"Table {tableId}";
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;

                if (TableStatus.empty.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    lblTableName.Background = Brushes.SeaGreen;
                    lblTableName.Foreground = Brushes.White;
                }
                else if(TableStatus.occupied.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    lblTableName.Background = Brushes.IndianRed;
                    lblTableName.Foreground = Brushes.White;
                }
            }
        }

        private void BtnEmpty_Click(object sender, RoutedEventArgs e)
        {
            StatusChanged?.Invoke(tableId, TableStatus.empty);
        }

        private void BtnOccupied_Click(object sender, RoutedEventArgs e)
        {
            StatusChanged?.Invoke(tableId, TableStatus.occupied);
        }
    }
}
