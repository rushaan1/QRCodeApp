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
using Microsoft.Data.Sqlite;
using System.IO;
using System.Diagnostics;
using ZXing;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for ExpandedPage.xaml
    /// </summary>
    public partial class ExpandedPage : Page
    {
        List<string> selectedItems = new List<string>();
        public ExpandedPage()
        {
            InitializeComponent();

            using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db")) 
            {
                connection.Open();

                var countCommand = connection.CreateCommand();
                countCommand.CommandText = "SELECT COUNT(*) FROM QrCodes";

                int rowCount = Convert.ToInt32(countCommand.ExecuteScalar());


                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM QrCodes";

                int i = rowCount;
                using (var reader = selectCommand.ExecuteReader()) 
                {
                    if (reader.HasRows) 
                    {
                        while (reader.Read()) 
                        {
                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Orientation = Orientation.Horizontal;

                            // Label
                            Label label = new Label();
                            label.Width = 63;
                            label.Content = i+".";
                            stackPanel.Children.Add(label);

                            // Button
                            Button button = new Button();
                            button.BorderBrush = System.Windows.Media.Brushes.Green;
                            button.Background = System.Windows.Media.Brushes.White;
                            button.Width = 381;
                            button.Height = 34;
                            button.Content = reader.GetString(0);
                            button.Margin = new System.Windows.Thickness(-26, -1, 0, 0);
                            button.FontSize = 16;
                            button.Tag = reader.GetString(1);
                            button.Click += RecentQRCodeButtonClick;
                            stackPanel.Children.Add(button);

                            // Label for time
                            Label timeLabel = new Label();
                            timeLabel.Content = reader.GetString(3);
                            stackPanel.Children.Add(timeLabel);

                            // Image
                            Image image = new Image();
                            if (File.Exists(reader.GetString(1)) && IsImageFile(reader.GetString(1)))
                            {
                                image.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else 
                            {
                                image.Visibility = System.Windows.Visibility.Visible;
                            }
                            image.ToolTip = "Either the file of this QR Code was deleted or renamed!";
                            image.Source = new BitmapImage(new Uri("/Images/warning.png", UriKind.Relative));
                            image.Width = 41;
                            image.Height = 25;
                            stackPanel.Children.Add(image);

                            // CheckBox
                            CheckBox checkBox = new CheckBox();
                            checkBox.Height = 16;
                            checkBox.Width = 15;
                            checkBox.Tag = reader.GetString(1);
                            checkBox.Margin = new System.Windows.Thickness(33, 0, 0, 0);
                            ScaleTransform scaleTransform = new ScaleTransform(1.7, 1.7);
                            checkBox.LayoutTransform = scaleTransform;
                            checkBox.Click += CheckBoxClicked;
                            stackPanel.Children.Add(checkBox);
                            // Create ListViewItem and add StackPanel as its content
                            ListViewItem listViewItem = new ListViewItem();
                            listViewItem.Content = stackPanel;

                            // Add ListViewItem to the ListView named "history"
                            history.Items.Insert(0, listViewItem);
                            i--;
                        }
                    }
                }
            }
        }


        private void CheckBoxClicked(object sender, RoutedEventArgs e) 
        {
            CheckBox cb = (CheckBox)sender;
            string filePath = cb.Tag as string;
            if (selectedItems.Contains(filePath))
            {
                selectedItems.Remove(filePath);
            }
            else 
            {
                selectedItems.Add(filePath);
            }

            if (selectedItems.Count > 0)
            {
                remover.Opacity = 0.5;
            }
            else 
            {
                remover.Opacity = 0.22;
            }
        }


        private void RemoveItems(object sender, RoutedEventArgs e) 
        {
            if (selectedItems.Count <= 0) 
            {
                return;
            }
            ConfirmationDialog cd = new ConfirmationDialog(selectedItems);
            cd.ShowDialog();
        }

        private void RemoveAllItems(object sender, RoutedEventArgs e)
        {
            ConfirmationDialog cd = new ConfirmationDialog();
            cd.ShowDialog();
        }

        private void RecentQRCodeButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string filePath = clickedButton.Tag as string;

            Trace.WriteLine("Recent QR Code Button Click Detected!");
            Trace.WriteLine("File Path: " + filePath);

            string content = "Error";

            using (var connection = new SqliteConnection($"Data Source=QrCodeDatabase.db"))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = $"SELECT content FROM QrCodes WHERE file_path=@item";
                selectCommand.Parameters.AddWithValue("@item", filePath);
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
                        myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(filePath), filePath, true, true);
                    }
                    else
                    {
                        myframe.frame.Content = new Scanned(true, content, filePath, true);
                    }
                }
                else
                {
                    myframe.frame.Content = new Scanned(true, content, filePath, true);
                }
            }
            else
            {
                myframe.frame.Content = new Scanned(false, content, filePath, true);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new MainPage();
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
