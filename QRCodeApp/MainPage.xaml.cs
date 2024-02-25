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
            DbManager dbm = new DbManager();
            dbm.InitializeDatabase();
            abc.Source = new BitmapImage(new Uri("/Images/sampleQR.png", UriKind.Relative)); 
            abc.Source = new BitmapImage(new Uri("/Images/scannn.png", UriKind.Relative));

            //Recents
            for (int i = 0; i < 20; i++) 
            {
                Button button = new Button
                {
                    BorderBrush = System.Windows.Media.Brushes.Green,
                    Background = System.Windows.Media.Brushes.White,
                    Width = 264,
                    Height = 31,
                    Content = $"{i+2}.QrCodeName >34",
                    Margin = new Thickness(-3, -1, 0, 0)
                };

                ListViewItem recentBtn = new ListViewItem();
                recentBtn.Content = button;
                recents.Items.Add(recentBtn);
            }
        }

        private void CreateNew(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Selection();
        }

        private void Scan(object sender, RoutedEventArgs e) 
        {
            myframe.frame.Content = new ScanSelection();
        }

        private void OpenCredits(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Credits();
            DbManager dbm = new DbManager();
            dbm.PrintQRCodes();
        }

    }
}
