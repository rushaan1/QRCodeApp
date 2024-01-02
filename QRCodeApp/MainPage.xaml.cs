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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            abc.Source = new BitmapImage(new Uri("/Images/sampleQR.png", UriKind.Relative)); 
            abc.Source = new BitmapImage(new Uri("/Images/scannn.png", UriKind.Relative)); 
        }

        private void CreateNew(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Create();
        }
    }
}
