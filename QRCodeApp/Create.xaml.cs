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
using QRCoder;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging; 

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page 
    {
        private Bitmap mainQR = null;
        private string format = "PNG";
        private Frame frame;

        private System.Drawing.Imaging.ImageFormat GetFormat(string format) 
        {
            System.Drawing.Imaging.ImageFormat myformat = null;
            switch (format) 
            {
                case "PNG":
                    myformat =  System.Drawing.Imaging.ImageFormat.Png;
                    break;
                case "EXIF":
                    myformat = System.Drawing.Imaging.ImageFormat.Exif;
                    break;
                case "JPG":
                    myformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "BMP":
                    myformat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
            }
            return myformat;
        }

        public static Bitmap GenerateQRCode(string data, System.Drawing.Color qrColor)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrBitmap = qrCode.GetGraphic(20, qrColor, System.Drawing.Color.Transparent, true);

            return qrBitmap;
        }

        public static Bitmap AddOverlay(Bitmap baseImage, Bitmap overlayImage)
        {
            using (Graphics g = Graphics.FromImage(baseImage))
            {
                g.DrawImage(overlayImage, new System.Drawing.Point((baseImage.Width - overlayImage.Width) / 2, (baseImage.Height - overlayImage.Height) / 2));
            }

            return baseImage;
        }

        public static void SaveQRCode(Bitmap qrCode, string filePath, System.Drawing.Imaging.ImageFormat format)
        {
            qrCode.Save(filePath, format); 
        }


        public Create()
        {
            InitializeComponent();
            //comboBox.SelectionChanged += ComboBox_Selected;
        }

        private void AddIcon(object sender, RoutedEventArgs e)
        {
            if (mainQR == null) 
            {
                MessageBox.Show("First generate a qr code","Error");
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // User selected an image file
                string selectedImagePath = openFileDialog.FileName;

                // Load the image and display it (replace "ImageControl" with the actual name of your Image control)
                Bitmap bitmapImage = new Bitmap(selectedImagePath);
                Bitmap img = AddOverlay(mainQR, bitmapImage);
                mainQR = img;
                SaveQRCode(img, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr.png"), System.Drawing.Imaging.ImageFormat.Png);
                qrcode.Source = new BitmapImage(new Uri("/GeneratedQRs/generatedqr.png", UriKind.Relative)); 
                //SaveQRCode(img, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr."+format.ToLower()), GetFormat(format));

            }
        }

        // More like download
        private void Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set initial directory to be inside "Downloads" directory
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string initialDirectory = System.IO.Path.Combine(downloadsPath, "QR Codes");

            // Ensure the directory exists; create it if not
            if (!Directory.Exists(initialDirectory))
            {
                Directory.CreateDirectory(initialDirectory);
            }

            saveFileDialog.InitialDirectory = initialDirectory;
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FileName = "NewFile.txt"; // Default file name

            if (saveFileDialog.ShowDialog() == true)
            {
                // User selected a location to save the file
                string filePath = saveFileDialog.FileName;
                SaveQRCode(mainQR, filePath, GetFormat(this.format));
                //SaveQRCode(mainQR, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedQRs", $"{name.Text}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            //if (name.Text == "" || text.Text == "") 
            {
                MessageBox.Show("Name and Text textboxes cannot be empty!", "Error");
                return;
            }

            //mainQR = GenerateQRCode(text.Text, System.Drawing.Color.FromArgb(colorPicker.SelectedColor.Value.A, colorPicker.SelectedColor.Value.R, colorPicker.SelectedColor.Value.G, colorPicker.SelectedColor.Value.B));
            SaveQRCode(mainQR, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr.png"), System.Drawing.Imaging.ImageFormat.Png);
            qrcode.Source = new BitmapImage(new Uri("/GeneratedQRs/generatedqr.png", UriKind.Relative));
            qrcode.Opacity = 1;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                

            }
        }

        //Format Selected
        private void ComboBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            //ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            //// Check if an item is selected (to handle the case when the ComboBox is cleared or not yet initialized)
            //if (selectedItem != null)
            //{
            //    // Access the content of the selected item
            //    string selectedContent = selectedItem.Content.ToString();
            //    format = selectedContent;
            //    // Display a message (you can replace this with your own logic)
            //}
        }

        //Size changed
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (e.NewValue == null || size == null) 
            //{
            //    return;
            //}
            //size.Content = e.NewValue.ToString() + "x" + e.NewValue.ToString();
            //if (mainQR == null) 
            //{
            //    return;
            //}
            //qrcode.Width = e.NewValue;
            //qrcode.Height = e.NewValue;
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
