using System.Windows;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using ZXing;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        List<string> filePaths = new List<string>();
        public ConfirmationDialog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            topLabel.Content = "Are you sure you want to delete ALL the QR Codes from History?";
        }

        public ConfirmationDialog(List<string> filePaths)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.filePaths = filePaths;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            DbManager dbm = new DbManager();
            if (deleteImgCheckbox.IsChecked.Value == true && filePaths.Count <= 0)
            {
                using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT file_path, content FROM QrCodes";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string path = reader.GetString(0);
                            string content = reader.GetString(1);
                            if (File.Exists(path))
                            {
                                using (var bitmap = new System.Drawing.Bitmap(path)) 
                                {
                                    var result = new BarcodeReader().Decode(bitmap);
                                    if (result != null)
                                    {
                                        if (result.Text == content) 
                                        {
                                            bitmap.Dispose();
                                            File.Delete(path);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                dbm.DeleteAllQRCodes();
            }
            else if (deleteImgCheckbox.IsChecked.Value == true && filePaths.Count > 0)
            {
                foreach (string path in filePaths)
                {
                    using (var connection = new SqliteConnection("Data Source=QrCodeDatabase.db"))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"SELECT content FROM QrCodes WHERE file_path='{path}'";
                        string content = command.ExecuteScalar() as string;

                        if (File.Exists(path))
                        {
                            using (var bitmap = new System.Drawing.Bitmap(path))
                            {
                                var result = new BarcodeReader().Decode(bitmap);

                                if (result != null)
                                {
                                    if (result.Text == content) 
                                    {
                                        bitmap.Dispose();
                                        File.Delete(path);
                                    }
                                }
                            }
                        }
                    }
                }
                dbm.DeleteQRCodes(filePaths);
            }
            else if (deleteImgCheckbox.IsChecked.Value == false && filePaths.Count > 0)
            {
                dbm.DeleteQRCodes(filePaths);
            }
            else if (deleteImgCheckbox.IsChecked.Value == false && filePaths.Count <= 0) 
            {
                dbm.DeleteAllQRCodes();
            }
            RefreshExpandedPage();
            Close();
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

        private void RefreshExpandedPage() 
        {
            myframe.frame.Content = null;
            myframe.frame.Content = new ExpandedPage();
        }
    }
}
