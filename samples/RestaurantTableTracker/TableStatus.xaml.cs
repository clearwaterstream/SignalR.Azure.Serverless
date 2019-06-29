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
    public partial class TableStatus : UserControl
    {
        readonly Brush defaultLblBg;
        readonly Brush defaultLblFg;

        public TableStatus()
        {
            InitializeComponent();

            defaultLblBg = lblTableName.Background;
            defaultLblFg = lblTableName.Foreground;
        }

        public string TableName
        {
            get
            {
                return (string)lblTableName.Content;
            }
            set
            {
                lblTableName.Content = value;
            }
        }

        private void BtnEmpty_Click(object sender, RoutedEventArgs e)
        {
            lblTableName.Background = Brushes.SeaGreen;
            lblTableName.Foreground = Brushes.White;
        }

        private void BtnOccupied_Click(object sender, RoutedEventArgs e)
        {
            lblTableName.Background = Brushes.IndianRed;
            lblTableName.Foreground = Brushes.White;
        }
    }
}
