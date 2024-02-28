using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ScanSelection.xaml
    /// </summary>
    public partial class ScanSelection : Page
    {
        public ScanSelection()
        {
            InitializeComponent();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            // Change the window color when files are dragged over it
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && AreAllFilesImages((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                e.Effects = DragDropEffects.Copy;
                ChangeWindowColor(Colors.LightGray);
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            // Change the window color while files are being dragged over it
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && AreAllFilesImages((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                e.Effects = DragDropEffects.Copy;
                ChangeWindowColor(Colors.LightGray);
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            // Reset the window color when the drag operation leaves the window
            ChangeWindowColor(Colors.White);
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            // Process dropped files only if they are images
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && AreAllFilesImages((string[])e.Data.GetData(DataFormats.FileDrop)))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ProcessDroppedFiles(files);
            }

            // Reset the window color
            ChangeWindowColor(Colors.White);

            e.Handled = true;
        }

        private void ProcessDroppedFiles(string[] files)
        {
            if (files.Length == 1)
            {
                myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(files[0]), files[0].ToString(), false, false);
            }
            else if (files.Length > 1)
            {
                myframe.frame.Content = new Scanned(files);
            }
            // Iterate through the dropped files and do something with them
            foreach (string file in files)
            {
                Console.WriteLine($"Dropped file: {file}");
                // Add your file processing logic here
            }
        }

        private bool AreAllFilesImages(string[] files)
        {
            // Check if all dropped files are image files
            return files.All(file => IsImageFile(file));
        }

        private bool IsImageFile(string filePath)
        {
            // Check if the file has a valid image extension
            string extension = System.IO.Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) &&
                   (extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase));
        }

        private void ChangeWindowColor(Color color)
        {
            // Change the background color of the window
            this.Background = new SolidColorBrush(color);
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new MainPage();
        }
        private void Files(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedImagePath = openFileDialog.FileNames;
                if (selectedImagePath.Length == 1)
                {
                    myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(selectedImagePath[0]), selectedImagePath[0].ToString(), false, false);
                }
                else if (selectedImagePath.Length > 1) 
                {
                    myframe.frame.Content = new Scanned(selectedImagePath); 
                }
            }
        }
        private void Camera(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new Scanner();
            //try
            //{
            //    Process.Start("explorer.exe", "shell:AppsFolder\\Microsoft.WindowsCamera_8wekyb3d8bbwe!App");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("An Error Occured: "+ex.Message, "Error");
            //}
        }
    }
}
