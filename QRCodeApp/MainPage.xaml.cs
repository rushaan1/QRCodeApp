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
using Microsoft.Data.Sqlite;
using System.IO;

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
            //for (int i = 0; i < 20; i++) 
            //{
            //    Button button = new Button
            //    {
            //        BorderBrush = System.Windows.Media.Brushes.Green,
            //        Background = System.Windows.Media.Brushes.White,
            //        Width = 264,
            //        Height = 31,
            //        Content = $"{i+1}.QrCodeName >34",
            //        Margin = new Thickness(-3, -1, 0, 0)
            //    };

            //    ListViewItem recentBtn = new ListViewItem();
            //    recentBtn.Content = button;
            //    recents.Items.Add(recentBtn);
            //}


            int i = 0;

            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM QrCodes";

                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        noQRFoundText.Visibility = Visibility.Hidden;
                        expandButton.Visibility = Visibility.Visible;
                        while (reader.Read())
                        {
                            Button button = new Button
                            {
                                Tag = reader.GetString(1), // file_path
                                BorderBrush = System.Windows.Media.Brushes.Green,
                                Background = System.Windows.Media.Brushes.White,
                                Width = 264,
                                Height = 31,
                                Content = $"{i + 1}.{reader.GetString(0)}", //name
                                Margin = new Thickness(-3, -1, 0, 0)
                            };
                            button.Click += RecentQRCodeButtonClick;


                            ListViewItem recentBtn = new ListViewItem();
                            recentBtn.Content = button;
                            recents.Items.Add(recentBtn);
                            i++;
                        }
                    }
                    else
                    {
                        noQRFoundText.Visibility = Visibility.Visible;
                        expandButton.Visibility = Visibility.Hidden;
                    }
                }
            }

        }

        private void RecentQRCodeButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string filePath = clickedButton.Tag as string;

            Trace.WriteLine("Recent QR Code Button Click Detected!");
            Trace.WriteLine("File Path: "+filePath);

            if (File.Exists(filePath))
            {
                myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(filePath), filePath, true);
            }
            else 
            {
                using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
                {
                    connection.Open();

                    var selectCommand = connection.CreateCommand();
                    selectCommand.CommandText = $"SELECT content FROM QrCodes WHERE file_path='{filePath}'";

                    object result = selectCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string content = result.ToString();
                        myframe.frame.Content = new Scanned(content, filePath);
                    }
                }
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
        }

        private void Expand(object sender, RoutedEventArgs e) 
        {
            myframe.frame.Content = new ExpandedPage();
        }

    }
}
