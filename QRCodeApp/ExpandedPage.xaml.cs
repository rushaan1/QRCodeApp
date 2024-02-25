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

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for ExpandedPage.xaml
    /// </summary>
    public partial class ExpandedPage : Page
    {
        public ExpandedPage()
        {
            InitializeComponent();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Selection();
        }
    }
}
