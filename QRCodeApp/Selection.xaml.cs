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
using System.Diagnostics;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : Page
    {
        public Selection()
        {
            InitializeComponent();
        }

        private void Back(object sender, RoutedEventArgs e) 
        {
            myframe.frame.Content = new MainPage();
        }

        private void t(object sender, RoutedEventArgs e) 
        {

            myframe.frame.Content = new Create(); 
        }
        private void u(object sender, RoutedEventArgs e)
        {

            myframe.frame.Content = new CreateUrl();
        }
        private void v(object sender, RoutedEventArgs e)
        {

            myframe.frame.Content = new CreateVcard();
        }
        private void w(object sender, RoutedEventArgs e)
        {

            myframe.frame.Content = new CreateWifi();
        }
        private void s(object sender, RoutedEventArgs e)
        {

            myframe.frame.Content = new CreateSms();
        }
    }
}
