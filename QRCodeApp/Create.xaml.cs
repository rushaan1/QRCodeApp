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
        private string type;

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
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
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

        private void AddText()
        {
            Paragraph dp = new Paragraph();
            Run textRun = new Run("\u00A0") { Name = "text", Foreground = System.Windows.Media.Brushes.Black };
            dp.Inlines.Add(textRun);
            textRuns["text"] = textRun;
            FlowDocument flowDocument = rich.Document;
            flowDocument.Blocks.Add(dp);
        }

        private void AddSms()
        {
            Paragraph dp = new Paragraph();
            Run phRun = new Run("\u00A0") { Name = "ph", Foreground = System.Windows.Media.Brushes.Black };
            Run msgRun = new Run("\u00A0") { Name = "msg", Foreground = System.Windows.Media.Brushes.Black };
            dp.Inlines.Add(new TextBlock() { Text = "Phone No:", Foreground = System.Windows.Media.Brushes.Green });
            dp.Inlines.Add(phRun);
            dp.Inlines.Add(new LineBreak());
            dp.Inlines.Add(new TextBlock() { Text = "Message:", Foreground = System.Windows.Media.Brushes.Green });
            dp.Inlines.Add(msgRun);
            smsRuns["ph"] = phRun;
            smsRuns["msg"] = msgRun;
            FlowDocument flowDocument = rich.Document;
            flowDocument.Blocks.Add(dp);
        }

        private void AddWiFi()
        {
            Paragraph dp = new Paragraph();
            Run ssidRun = new Run("\u00A0") { Name = "ssid", Foreground = System.Windows.Media.Brushes.Black };
            Run pwdRun = new Run("\u00A0") { Name = "pwd", Foreground = System.Windows.Media.Brushes.Black };
            dp.Inlines.Add(new TextBlock() { Text = "SSID:", Foreground = System.Windows.Media.Brushes.Green });
            dp.Inlines.Add(ssidRun);
            dp.Inlines.Add(new LineBreak());
            dp.Inlines.Add(new TextBlock() { Text = "Password:", Foreground = System.Windows.Media.Brushes.Green });
            dp.Inlines.Add(pwdRun);
            wifiRuns["ssid"] = ssidRun;
            wifiRuns["pwd"] = pwdRun;
            FlowDocument flowDocument = rich.Document;
            flowDocument.Blocks.Add(dp);
        }

        private void AddVCard()
        {
            FlowDocument flowDocument = rich.Document;
            Paragraph dp = new Paragraph(); 
            Run fnRun = new Run() {Foreground = System.Windows.Media.Brushes.Black };
            Run companyRun = new Run() {Foreground = System.Windows.Media.Brushes.Black };
            Run telRun = new Run() {Foreground = System.Windows.Media.Brushes.Black };
            Run emailRun = new Run() { Foreground = System.Windows.Media.Brushes.Black };
            Run adrRun = new Run() {Foreground = System.Windows.Media.Brushes.Black };
            
            InlineUIContainer fnTb = new InlineUIContainer(new TextBlock() {  Tag = "fn", Text = "First Name:", Foreground = System.Windows.Media.Brushes.Green });
            InlineUIContainer ctbh = new InlineUIContainer(new TextBlock() {  Tag = "company", Text = "Company:", Foreground = System.Windows.Media.Brushes.Green });
            InlineUIContainer teltb = new InlineUIContainer(new TextBlock() { Tag = "tel", Text = "Phone/Tel No:", Foreground = System.Windows.Media.Brushes.Green });
            InlineUIContainer emailtb = new InlineUIContainer(new TextBlock() {  Tag = "email", Text = "Email:", Foreground = System.Windows.Media.Brushes.Green });
            InlineUIContainer adrtb = new InlineUIContainer(new TextBlock() {  Tag = "adr", Text = "Address:", Foreground = System.Windows.Media.Brushes.Green });
            fnTb.Unloaded += InlineUIContainer_Unloaded_1;
            ctbh.Unloaded += InlineUIContainer_Unloaded_2;
            teltb.Unloaded += InlineUIContainer_Unloaded_3;
            emailtb.Unloaded += InlineUIContainer_Unloaded_4;
            adrtb.Unloaded += InlineUIContainer_Unloaded_5;
            dp.Inlines.Add(fnTb);
            //np.Inlines.Add(fnTb);

            dp.Inlines.Add(fnRun);
            dp.Inlines.Add(new LineBreak());

            dp.Inlines.Add(ctbh);
            //np.Inlines.Add(ctbh);

            dp.Inlines.Add(companyRun);
            dp.Inlines.Add(new LineBreak());

            dp.Inlines.Add(teltb);
            //np.Inlines.Add(teltb);

            dp.Inlines.Add(telRun);
            dp.Inlines.Add(new LineBreak());

            dp.Inlines.Add(emailtb);
            //np.Inlines.Add(emailtb);

            dp.Inlines.Add(emailRun);
            dp.Inlines.Add(new LineBreak());

            dp.Inlines.Add(adrtb);
            //np.Inlines.Add(adrtb);

            dp.Inlines.Add(adrRun);

            vcardRuns["fn"] = fnRun;
            vcardRuns["company"] = companyRun;
            vcardRuns["tel"] = telRun;
            vcardRuns["email"] = emailRun;
            vcardRuns["adr"] = adrRun;

            // Get the FlowDocument of the RichTextBox
            

            // Add the dynamicParagraph to the Blocks collection
            flowDocument.Blocks.Add(dp);
        }

        private void InlineUIContainer_Unloaded_1(object sender, RoutedEventArgs e)
        {
            (sender as InlineUIContainer).Unloaded -= new RoutedEventHandler(InlineUIContainer_Unloaded_1);

            TextBlock tb = new TextBlock();
            tb.Text = "Full Name:";
            tb.Foreground = System.Windows.Media.Brushes.Green; 

            TextPointer tp = rich.CaretPosition.GetInsertionPosition(LogicalDirection.Backward);
            //rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length+1);
            InlineUIContainer iuic = new InlineUIContainer(tb, tp);

            rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length + 1);
            iuic.Unloaded += new RoutedEventHandler(InlineUIContainer_Unloaded_1);
        }

        private void InlineUIContainer_Unloaded_2(object sender, RoutedEventArgs e)
        {
            (sender as InlineUIContainer).Unloaded -= new RoutedEventHandler(InlineUIContainer_Unloaded_2);

            TextBlock tb = new TextBlock();
            tb.Text = "Company:";
            tb.Foreground = System.Windows.Media.Brushes.Green;

            TextPointer tp = rich.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
            //rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length+1);
            InlineUIContainer iuic = new InlineUIContainer(tb, tp);
            iuic.Unloaded += new RoutedEventHandler(InlineUIContainer_Unloaded_2);
        }

        private void InlineUIContainer_Unloaded_3(object sender, RoutedEventArgs e)
        {
            (sender as InlineUIContainer).Unloaded -= new RoutedEventHandler(InlineUIContainer_Unloaded_3);

            TextBlock tb = new TextBlock();
            tb.Text = "Phone/Tel:";
            tb.Foreground = System.Windows.Media.Brushes.Green;

            TextPointer tp = rich.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
            //rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length+1);
            InlineUIContainer iuic = new InlineUIContainer(tb, tp);
            iuic.Unloaded += new RoutedEventHandler(InlineUIContainer_Unloaded_3);
        }

        private void InlineUIContainer_Unloaded_4(object sender, RoutedEventArgs e)
        {
            (sender as InlineUIContainer).Unloaded -= new RoutedEventHandler(InlineUIContainer_Unloaded_4);

            TextBlock tb = new TextBlock();
            tb.Text = "Email:";
            tb.Foreground = System.Windows.Media.Brushes.Green;

            TextPointer tp = rich.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
            //rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length+1);
            InlineUIContainer iuic = new InlineUIContainer(tb, tp);
            iuic.Unloaded += new RoutedEventHandler(InlineUIContainer_Unloaded_4);
        }

        private void InlineUIContainer_Unloaded_5(object sender, RoutedEventArgs e)
        {
            (sender as InlineUIContainer).Unloaded -= new RoutedEventHandler(InlineUIContainer_Unloaded_5);

            TextBlock tb = new TextBlock();
            tb.Text = "Address:";
            tb.Foreground = System.Windows.Media.Brushes.Green;

            TextPointer tp = rich.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
            //rich.CaretPosition = rich.CaretPosition.GetPositionAtOffset(tb.Text.Length+1);
            InlineUIContainer iuic = new InlineUIContainer(tb, tp);
            iuic.Unloaded += new RoutedEventHandler(InlineUIContainer_Unloaded_5);
        }

        private void richTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Back || e.Key == Key.Delete)
            //{
            //    TextPointer caretPosition = rich.CaretPosition;
            //    if (caretPosition != null)
            //    {
            //        TextPointer prevCaretPosition = caretPosition.GetPositionAtOffset(0);
            //        if (prevCaretPosition != null)
            //        {
            //            Paragraph paragraph = prevCaretPosition.Paragraph;
            //            if (paragraph != null)
            //            {
            //                // Get the Inline elements within the Paragraph
            //                InlineCollection inlines = paragraph.Inlines;

            //                // Iterate through the Inline elements
            //                Inline inlineBeforeCaret = null;
            //                foreach (Inline inline in inlines)
            //                {
            //                    // Check if the Inline ends just before the caret position
            //                    if (inline.ElementEnd.CompareTo(prevCaretPosition) == 0)
            //                    {
            //                        e.Handled = true;
            //                        //inlineBeforeCaret = inline;
            //                        //Trace.WriteLine("color: "+ inlineBeforeCaret.Foreground.ToString());
            //                        //if (inlineBeforeCaret.Foreground.ToString() == "#FF000000") 
            //                        //{
            //                        //    Trace.WriteLine("texto: " + inlineBeforeCaret.Foreground.ToString());
            //                        //    e.Handled = true;
            //                        //}
            //                        break;
            //                    }
            //                }

            //                // Now you have the Inline just before the caret
            //            }
            //        }
            //    }
            //}


        }






        private bool ShouldPreventDeletion()
        {
            // Implement your condition for preventing deletion here
            // For example, check if the current selection contains a specific type of inline element

            // Replace this condition with your specific logic
            // For example, preventing deletion if a TextBlock is selected

            TextSelection selection = rich.Selection;
            if (selection != null && selection.Start.Parent is TextBlock)
            {
                Trace.WriteLine("Its true");
                return true;
            }

            return false;
        }

        /** For reference purposes
         *  FN:John Doe
            ORG:Company Inc.
            TEL:+123456789
            EMAIL:john.doe@example.com
            ADR:123 Main Street, Cityville, State, 12345, USA
            END:VCARD
         */
        private string GetQRText() 
        {
            string myText = "";
            
            switch (type)
            {
                case "text":
                    myText = textRuns["text"].Text; 
                    break;
                case "vcard":
                    myText = $"BEGIN:VCARD\nVERSION: 3.0\nFN:{vcardRuns["fn"].Text}" +
                        $"\nORG:{vcardRuns["company"].Text}\nTEL:{vcardRuns["tel"].Text}\nEMAIL:{vcardRuns["email"].Text}\nADR:{vcardRuns["adr"].Text}\nEND:VCARD";
                    break;
                case "url":
                    myText = textRuns["text"].Text;
                    break;
                case "sms":
                    myText = $"smsto:{smsRuns["ph"].Text}?body=${smsRuns["msg"].Text.Replace(" ", "%20")}";
                    break;
                case "wifi":
                    myText = $"WIFI:T:WPA;S:{wifiRuns["ssid"].Text};P:{wifiRuns["pwd"].Text};;";
                    break;
            }
            return myText;
        }


        public Create(string type)
        {
            InitializeComponent();
            this.type = type;
            comboBox.SelectionChanged += ComboBox_Selected;
            switch (type) 
            {
                case "text":
                    title.Content = "Text";
                    AddText();
                    break;
                case "vcard":
                    title.Content = "vCard";
                    AddVCard();
                    break;
                case "url":
                    title.Content = "URL";
                    AddText();
                    break;
                case "sms":
                    title.Content = "SMS";
                    AddSms();
                    break;
                case "wifi":
                    title.Content = "WiFi";
                    AddWiFi();
                    break;
            }
            //System.Threading.Thread thread = new System.Threading.Thread(CheckForInlines);
            //thread.Start();
        }

        //private void CheckForInlines()
        //{
        //    while (true)
        //    {
        //        int count = 0;

        //        foreach (Inline line in dp.Inlines)
        //        {
        //            this.Dispatcher.Invoke(() =>
        //            {
        //                Trace.WriteLine(dp.Inlines.LastInline.Name);
        //                if (line.Name == "fn" || line.Name == "tel" || line.Name == "company" || line.Name == "adr" || line.Name == "email")
        //                {
        //                    count++;
        //                }
        //            });
        //        }
        //        if (count != 5)
        //        {
        //            this.Dispatcher.Invoke(() =>
        //            {
        //                AddVCard(true, vcardRuns["fn"].Text, vcardRuns["company"].Text, vcardRuns["tel"].Text, vcardRuns["email"].Text, vcardRuns["adr"].Text);
        //            });
        //        }Trace.WriteLine(count);
        //        System.Threading.Thread.Sleep(100);
        //    }
        //}

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
                //SaveQRCode(mainQR, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedQRs", $"{name.Text}.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            string qrText = GetQRText();
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
            myframe.frame.Content = new MainPage();
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
