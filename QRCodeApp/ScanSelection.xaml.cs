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
    /// Interaction logic for ScanSelection.xaml
    /// </summary>
    public partial class ScanSelection : Page
    {
        public ScanSelection()
        {
            InitializeComponent();
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new MainPage();
        }
        private void Files(object sender, RoutedEventArgs e)
        {
            //myframe.frame.Content = new MainPage();
        }
        private void Camera(object sender, RoutedEventArgs e)
        {
            //myframe.frame.Content = new MainPage();
        }
    }
}
