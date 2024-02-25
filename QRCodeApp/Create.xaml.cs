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
using System.Diagnostics;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page 
    {
        private Bitmap mainQR = null;
        private Bitmap overlay = null;
        private string contentWhenGenerated = "";

        private string format = "PNG"; 
        private Frame frame;

        private Dictionary<string, Run> textRuns = new Dictionary<string, Run>();
        private Dictionary<string, Run> smsRuns = new Dictionary<string, Run>();
        private Dictionary<string, Run> wifiRuns = new Dictionary<string, Run>();
        private Dictionary<string, Run> vcardRuns = new Dictionary<string, Run>();

        private Paragraph dp;

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
                case "JPEG":
                    myformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "BMP":
                    myformat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
            }
            return myformat;
        }

        public Bitmap GenerateQRCode(string data, System.Drawing.Color qrColor)
        {
            System.Drawing.Color clr = System.Drawing.Color.White;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrBitmap = qrCode.GetGraphic(20, qrColor, clr , true);
            if (overlay != null) 
            {
                qrBitmap = AddOverlay(qrBitmap, overlay); 
            }
            contentWhenGenerated = data;
            return qrBitmap; 
        }

        public Bitmap AddOverlay(Bitmap baseImage, Bitmap overlayImage)
        {
            overlay = overlayImage;
            // Calculate the dimensions of the overlay (center 20% of the base image)
            int overlayWidth = (int)(baseImage.Width * 0.2);
            int overlayHeight = (int)(baseImage.Height * 0.2);

            // Calculate the position to center the overlay
            int overlayX = (baseImage.Width - overlayWidth) / 2;
            int overlayY = (baseImage.Height - overlayHeight) / 2;

            using (Graphics g = Graphics.FromImage(baseImage))
            {
                g.DrawImage(overlayImage, new System.Drawing.Rectangle(overlayX, overlayY, overlayWidth, overlayHeight));
            }

            return baseImage;
        }


        public void SaveQRCode(bool update, Bitmap qrCode, string filePath, System.Drawing.Imaging.ImageFormat format)
        {
            qrCode.Save(filePath, format);
            if (update) 
            {
                setImg();
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage B2BI(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // Save the bitmap to the memory stream in a specified format (e.g., Bmp)
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Create a new BitmapImage and set its stream source to the memory stream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new System.IO.MemoryStream(memoryStream.ToArray());
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void setImg() 
        {
            qrcode.Source = B2BI(mainQR);
        }

        /** For reference purposes
         *  FN:John Doe
            ORG:Company Inc.
            TEL:+123456789
            EMAIL:john.doe@example.com
            ADR:123 Main Street, Cityville, State, 12345, USA
            END:VCARD
         */


        public Create()
        {
            InitializeComponent();
            comboBox.SelectionChanged += ComboBox_Selected;
            //System.Threading.Thread thread = new System.Threading.Thread(CheckForInlines);
            //thread.Start();
        }

        private void AddIcon(object sender, RoutedEventArgs e)
        {
            if ((e.Source as Button).Content == "Remove Icon") 
            {
                overlay = null;
                mainQR = GenerateQRCode(contentWhenGenerated, System.Drawing.Color.FromArgb(colorPicker.SelectedColor.Value.A, colorPicker.SelectedColor.Value.R, colorPicker.SelectedColor.Value.G, colorPicker.SelectedColor.Value.B));
                setImg();
                (e.Source as Button).Content = "Add Icon";
                return;
            }
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
                filename.Content = openFileDialog.Title;  

                // Load the image and display it (replace "ImageControl" with the actual name of your Image control)
                Bitmap bitmapImage = new Bitmap(selectedImagePath);
                Bitmap img = AddOverlay(mainQR, bitmapImage);
                mainQR = img;
                SaveQRCode(true, img, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr.png"), System.Drawing.Imaging.ImageFormat.Png);
                //qrcode.Source = new BitmapImage(new Uri("/GeneratedQRs/generatedqr.png", UriKind.Relative)); 
                //SaveQRCode(img, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr."+format.ToLower()), GetFormat(format));
                (e.Source as Button).Content = "Remove Icon"; 
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
            saveFileDialog.Filter = $"Text Files (*.{format.ToLower()})|*.{format.ToLower()}|All files (*.*)|*.*";
            saveFileDialog.FileName = name.Text+"."+format.ToLower(); // Default file name

            if (saveFileDialog.ShowDialog() == true)
            {
                // User selected a location to save the file
                string filePath = saveFileDialog.FileName;
                Trace.WriteLine($"Filepath: {filePath} \n format: {this.format} ");
                SaveQRCode(false, mainQR, filePath, GetFormat(this.format));
                DbManager dbm = new DbManager();
                dbm.InsertQRCode(name.Text, filePath, contentWhenGenerated);
                //SaveQRCode(mainQR, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedQRs", $"{name.Text}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            string qrText = text.Text;
            if (name.Text == "" || qrText == "") 
            {
                MessageBox.Show("Name and Text textboxes cannot be empty!", "Error");
                return;
            }

            mainQR = GenerateQRCode(qrText, System.Drawing.Color.FromArgb(colorPicker.SelectedColor.Value.A, colorPicker.SelectedColor.Value.R, colorPicker.SelectedColor.Value.G, colorPicker.SelectedColor.Value.B));
            SaveQRCode(true, mainQR, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedQRs", "generatedqr.png"), System.Drawing.Imaging.ImageFormat.Png);
            //qrcode.Source = new BitmapImage(new Uri("/GeneratedQRs/generatedqr.png", UriKind.Relative));
            qrcode.Opacity = 1.0;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Selection();
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
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            // Check if an item is selected (to handle the case when the ComboBox is cleared or not yet initialized)
            if (selectedItem != null)
            {
                // Access the content of the selected item
                string selectedContent = selectedItem.Content.ToString();
                format = selectedContent;
                // Display a message (you can replace this with your own logic)
            }
        }

        //Size changed
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == null || size == null)
            {
                return;
            }
            size.Content = ( (int) e.NewValue).ToString() + "x" + ((int)e.NewValue).ToString();
            if (mainQR == null)
            {
                pako.Value = 200; 
                return;
            }
            qrcode.Width = e.NewValue;
            qrcode.Height = e.NewValue;
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
