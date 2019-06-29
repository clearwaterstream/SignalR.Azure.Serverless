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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string singalRConnectionString;

        public MainWindow()
        {
            InitializeComponent();
        }

        void InitSignalR()
        {
            if (string.IsNullOrEmpty(singalRConnectionString))
                throw new ArgumentNullException(nameof(singalRConnectionString), "copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)");
        }
    }
}
