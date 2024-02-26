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
using ZXing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using QRCoder;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Scanned.xaml
    /// </summary>
    public partial class Scanned : Page
    {
        private Bitmap qr;
        private int index;
        private bool fromMain = false;
        private bool fromHistory = false;
        private string[] qrpaths;

        private string processed = "";
        private string raw;

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

        public Scanned(Bitmap qr, string fileLocation, bool fromMain, bool fromHistory)
        {
            InitializeComponent();
            this.qr = qr;
            this.fromMain = fromMain;
            this.fromHistory = fromHistory;
            BarcodeReader barcodeReader = new BarcodeReader();
            ZXing.Result result = barcodeReader.Decode(qr);
            if (result == null) 
            {
                myframe.frame.Content = new ScanSelection();
                MessageBox.Show("No QR Code Found in the provided image", "Error");
                return;
            }
            //TODO Tiny bug when the image had no qr code
            filename.Text = fileLocation;
            counter.Visibility = Visibility.Hidden;
            prev.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Hidden;
            showRaw.Visibility = Visibility.Visible;
            showMain.Visibility = Visibility.Visible;
            IdentifyQrCodeContent(result.Text);
            raw = result.Text;
            scannedQR.Source = B2BI(qr);
        }

        public Scanned(bool mismatch, string content, string fileLocation, bool fromHistory)
        {
            InitializeComponent();
            this.fromMain = true;
            this.fromHistory = fromHistory;
            filename.Text = "Either the file of this QR Code was deleted or renamed!\n"+fileLocation;
            filename.Foreground = System.Windows.Media.Brushes.Red;
            if (mismatch) 
            {
                filename.Text = "Issues while processing QR Code!\n"+fileLocation;
                filename.Foreground = System.Windows.Media.Brushes.Black;
            }
            warningImg1.Visibility = Visibility.Visible;
            warningImg2.Visibility = Visibility.Visible;
            counter.Visibility = Visibility.Hidden;
            prev.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Hidden;
            showRaw.Visibility = Visibility.Visible;
            showMain.Visibility = Visibility.Visible;
            IdentifyQrCodeContent(content);
            raw = content;

            System.Drawing.Color clr = System.Drawing.Color.White;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrBitmap = qrCode.GetGraphic(20, System.Drawing.Color.Black, clr, true);

            scannedQR.Source = B2BI(qrBitmap);
        }

        public Scanned(string[] qrpaths)  
        {
            InitializeComponent();
            this.qrpaths = qrpaths;
            Bitmap bmp = new Bitmap(qrpaths[0]);
            this.qr = bmp; 
            BarcodeReader barcodeReader = new BarcodeReader();
            ZXing.Result result = barcodeReader.Decode(bmp);
            if (result == null)
            {
                MessageBox.Show("No QR Code Found in this image", "Error");
                scannedQR.Source = B2BI(bmp);
                filename.Text = qrpaths[0];
                counter.Text = $"{index+1}/{qrpaths.Length}";
                index = qrpaths.Length;
                prev.Visibility = Visibility.Hidden;
                qrDetails.Text = "Invalid/Empty QR Code Data!";
                return;
            }
            index = 0;
            Trace.WriteLine(qrpaths.Length);
            prev.Visibility = Visibility.Hidden;
            filename.Text = qrpaths[0];
            counter.Text = $"{index+1}/{qrpaths.Length}";
            IdentifyQrCodeContent(result.Text);
            scannedQR.Source = B2BI(bmp);
        }

        private void Previous(object sender, RoutedEventArgs e) 
        {
            Trace.WriteLine(index+" when entering");
            if (index-1 == 0) 
            {
                prev.Visibility = Visibility.Hidden;
            }
            if (index + 1 == qrpaths.Length ) 
            {
                next.Visibility = Visibility.Visible;
            }
            Bitmap bmp = new Bitmap(qrpaths[index-1]);
            this.qr = bmp;
            BarcodeReader barcodeReader = new BarcodeReader();
            ZXing.Result result = barcodeReader.Decode(bmp);
            if (result == null)
            {
                MessageBox.Show("No QR Code Found in this image", "Error");
                scannedQR.Source = B2BI(bmp);
                index = index - 1;
                counter.Text = $"{index+1}/{qrpaths.Length}";
                filename.Text = qrpaths[index];
                qrDetails.Text = "Invalid/Empty QR Code Data!";
                return;
            }
            
            index = index - 1;
            Trace.WriteLine(index + " when leaving");
            filename.Text = qrpaths[index];
            counter.Text = $"{index+1}/{qrpaths.Length}";
            IdentifyQrCodeContent(result.Text);
            scannedQR.Source = B2BI(bmp);
        }
        
        private void Next(object sender, RoutedEventArgs e) 
        {
            Trace.WriteLine(index + " when entering");
            if (index + 1 == qrpaths.Length - 1)
            {
                Trace.WriteLine("Hiding it");
                next.Visibility = Visibility.Hidden;
            }
            if (index  == 0)
            {
                prev.Visibility = Visibility.Visible;
            }
            Bitmap bmp = new Bitmap(qrpaths[index + 1]);
            this.qr = bmp;
            BarcodeReader barcodeReader = new BarcodeReader();
            ZXing.Result result = barcodeReader.Decode(bmp);
            if (result == null)
            {
                MessageBox.Show("No QR Code Found in this image", "Error");
                scannedQR.Source = B2BI(bmp);
                index = index + 1;
                counter.Text = $"{index+1}/{qrpaths.Length}";
                filename.Text = qrpaths[index];
                qrDetails.Text = "Invalid/Empty QR Code Data!";
                return;
            }
            index = index + 1;
            Trace.WriteLine(index + " when leaving");
            filename.Text = qrpaths[index];
            counter.Text = $"{index+1}/{qrpaths.Length}";
            IdentifyQrCodeContent(result.Text);
            scannedQR.Source = B2BI(bmp);
        }

        private void ShowRaw(object sender, RoutedEventArgs e) 
        {
            if (processed == "") 
            {
                processed = qrDetails.Text;
            }
            qrDetails.Text = raw;
        }

        private void ShowMain(object sender, RoutedEventArgs e)
        {
            if (processed == "") 
            {
                return;
            }
            qrDetails.Text = processed;
        }

        private void Back(object sender, RoutedEventArgs e) 
        {
            qr = null;
            scannedQR.Source = null;
            if (fromHistory) 
            {
                myframe.frame.Content = new ExpandedPage();
                return;
            }
            if (fromMain) 
            {
                myframe.frame.Content = new MainPage();
                return;
            }
            myframe.frame.Content = new ScanSelection();
        }

        private void IdentifyQrCodeContent(string qrCodeText)
        {
            if (IsVCard(qrCodeText))
            {
                Trace.WriteLine("vCard detected");
                string stuff = qrCodeText;
                try
                {
                    stuff = ReplaceFirst(stuff, "BEGIN:VCARD", "");
                    stuff = ReplaceFirst(stuff, "VERSION:3.0", "********");
                    stuff = ReplaceFirst(stuff, "VERSION:4.0", "********");
                    stuff = ReplaceFirst(stuff, "VERSION:2.1", "********");
                    stuff = ReplaceFirst(stuff, "FN:", "Full Name:");
                    stuff = ReplaceFirst(stuff, "ORG:", "Company:");
                    stuff = ReplaceFirst(stuff, "TEL:", "Phone/Tel:");
                    stuff = ReplaceFirst(stuff, "EMAIL:", "Email:");
                    stuff = ReplaceFirst(stuff, "ADR:", "Address:");
                    stuff = ReplaceFirst(stuff, "END:VCARD", "********");
                    stuff = stuff.Replace("%20", " ");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    MessageBox.Show("An Error Occured while processing QR Code Text. Raw Data has been set Error Message " + ex.Message);
                    qrDetails.Text = "vCard" + stuff;
                    return;
                }
                qrDetails.Text = "vCard" + stuff;
            }
            else if (IsSms(qrCodeText))
            {
                string stuff = qrCodeText;
                Trace.WriteLine("SMS detected");
                try 
                {
                    stuff = ReplaceFirst(stuff, "smsto:", "Phone:");
                    stuff = ReplaceFirst(stuff, "?body=", "\nBody:");
                    stuff = stuff.Replace("%20", " ");
                }
                catch(Exception ex) 
                {
                    Trace.WriteLine(ex.Message);
                    MessageBox.Show("An Error Occured while processing QR Code Text. Raw Data has been set Error Message "+ex.Message);
                    qrDetails.Text = "SMS\n" + stuff;
                    return;
                }
                qrDetails.Text = "SMS\n" + stuff;
            }
            else if (IsEmail(qrCodeText))
            {
                Trace.WriteLine("Email detected");
                string stuff = qrCodeText;
                try
                {
                    stuff = ReplaceFirst(stuff, "mailto:", "Recipient:");
                    stuff = ReplaceFirst(stuff, "?subject=", "\nSubject:");
                    stuff = ReplaceFirst(stuff, "&body=", "\nBody:");
                    stuff = stuff.Replace("%20", " ");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    MessageBox.Show("An Error Occured while processing QR Code Text. Raw Data has been set Error Message " + ex.Message);
                    qrDetails.Text = "Email\n" + stuff;
                    return;
                }
                qrDetails.Text = "Email\n" + stuff;
            }
            else if (IsWiFi(qrCodeText))
            {
                Trace.WriteLine("Wi-Fi detected");
                string stuff = qrCodeText;
                try
                {
                    stuff = ReplaceFirst(stuff, "WIFI:T:", "Encryption:");
                    stuff = ReplaceFirst(stuff, ";S:", "\nSSID:");
                    stuff = ReplaceFirst(stuff, ";P:", "\nPassword:");
                    stuff = ReplaceFirst(stuff, ";;", "");
                    stuff = stuff.Replace("%20", " ");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    MessageBox.Show("An Error Occured while processing QR Code Text. Raw Data has been set Error Message " + ex.Message);
                    qrDetails.Text = "WiFi\n" + stuff;
                    return;
                }
                qrDetails.Text = "WiFi\n" + stuff;
            }
            else
            {
                Trace.WriteLine("None of the specified types detected");
                qrDetails.Text = qrCodeText;
            }
        }

        private string ReplaceFirst(string original, string search, string replacement)
        {
            try
            {
                Regex regex = new Regex(Regex.Escape(search));
                return regex.Replace(original, replacement, 1);
            }
            catch (Exception ex) 
            {
                return original;
            }
        }

        static bool IsVCard(string text)
        {
            // vCard pattern
            string vCardPattern = @"BEGIN:VCARD";

            return Regex.IsMatch(text, vCardPattern);
        }

        static bool IsSms(string text)
        {
            // SMS pattern
            string smsPattern = @"^smsto\:.*";

            return Regex.IsMatch(text, smsPattern, RegexOptions.IgnoreCase);
        }

        static bool IsEmail(string text)
        {
            // Email pattern
            string emailPattern = @"^mailto\:.*";

            return Regex.IsMatch(text, emailPattern, RegexOptions.IgnoreCase);
        }

        static bool IsWiFi(string text)
        {
            // Wi-Fi pattern
            string wifiPattern = @"^WIFI\:";

            return Regex.IsMatch(text, wifiPattern, RegexOptions.IgnoreCase);
        }
    }
}
