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
using System.Drawing;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Scanned.xaml
    /// </summary>
    public partial class Scanned : Page
    {
        private Bitmap qr;
        public Scanned(Bitmap qr)
        {
            InitializeComponent();
            this.qr = qr;
        }
        private void Back(object sender, RoutedEventArgs e) 
        {
            myframe.frame.Content = new ScanSelection();
        }
    }
}
