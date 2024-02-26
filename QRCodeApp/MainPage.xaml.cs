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
using ZXing;

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


            List<Button> buttons = new List<Button>();

            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var countCommand = connection.CreateCommand();
                countCommand.CommandText = "SELECT COUNT(*) FROM QrCodes";

                int rowCount = Convert.ToInt32(countCommand.ExecuteScalar());

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM QrCodes";

                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        noQRFoundText.Visibility = Visibility.Hidden;
                        expandButton.Visibility = Visibility.Visible;
                        int i = rowCount;
                        while (reader.Read())
                        {
                            Button button = new Button
                            {
                                Tag = reader.GetString(1), // file_path
                                BorderBrush = System.Windows.Media.Brushes.Green,
                                Background = System.Windows.Media.Brushes.White,
                                Width = 264,
                                Height = 31,
                                Content = $"{i}.{reader.GetString(0)}", // name
                                Margin = new Thickness(-3, -1, 0, 0)
                            };
                            button.Click += RecentQRCodeButtonClick;

                            buttons.Add(button); // Add the button to the list in the original order
                            i--;
                        }
                    }
                    else
                    {
                        noQRFoundText.Visibility = Visibility.Visible;
                        expandButton.Visibility = Visibility.Hidden;
                    }
                }
            }

            // Reverse the list of buttons before adding them to the ListView
            buttons.Reverse();

            foreach (var button in buttons)
            {
                ListViewItem recentBtn = new ListViewItem();
                recentBtn.Content = button;
                recents.Items.Add(recentBtn);
            } //ik I should've just inserted items at the beginning of the list view which wouldn't have required list but whatever


        }

        private void RecentQRCodeButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string filePath = clickedButton.Tag as string;

            Trace.WriteLine("Recent QR Code Button Click Detected!");
            Trace.WriteLine("File Path: "+filePath);

            string content = "Error";

            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT content FROM QrCodes WHERE file_path='{filePath}'";

                object result = selectCommand.ExecuteScalar();

                if (result != null)
                {
                    content = result.ToString();
                }
            }

            if (File.Exists(filePath) && IsImageFile(filePath))
            {
                if (new BarcodeReader().Decode(new System.Drawing.Bitmap(filePath)) != null)
                {
                    if (new BarcodeReader().Decode(new System.Drawing.Bitmap(filePath)).Text == content)
                    {
                        myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(filePath), filePath, true, false);
                    }
                    else
                    {
                        myframe.frame.Content = new Scanned(true, content, filePath, false);
                    }
                }
                else 
                {
                    myframe.frame.Content = new Scanned(true, content, filePath, false);
                }
            }
            else 
            {
                myframe.frame.Content = new Scanned(false, content, filePath, false);
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

        private bool IsImageFile(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                    // Check if the decoder is created successfully, indicating that the file is a valid image
                    return decoder.Frames.Count > 0;
                }
            }
            catch (Exception)
            {
                // An exception occurred, indicating that the file is not a valid image or couldn't be opened
                return false;
            }
        }

    }
}
