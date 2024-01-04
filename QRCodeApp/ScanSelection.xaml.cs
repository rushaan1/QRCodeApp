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
        private void Back(object sender, RoutedEventArgs e)
        {
            myframe.frame.Content = new MainPage();
        }
        private void Files(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                myframe.frame.Content = new Scanned(new System.Drawing.Bitmap(selectedImagePath), selectedImagePath.ToString());
            }
        }
        private void Camera(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "shell:AppsFolder\\Microsoft.WindowsCamera_8wekyb3d8bbwe!App");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error Occured: "+ex.Message, "Error");
            }
        }
    }
}
