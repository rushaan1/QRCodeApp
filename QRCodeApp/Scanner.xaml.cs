﻿using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using System.IO;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

namespace QRCodeApp
{
    /// <summary>
    /// Interaction logic for Scanner.xaml
    /// </summary>
    public partial class Scanner : Page
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        public Scanner()
        {
            InitializeComponent();
            InitializeCamera();
        }
        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No video capture devices found.", "No Camera");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {

                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    cameraImage.Source = BitmapToImageSource(bitmap);
                });

                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode(bitmap);

                if (result != null)
                {
                    string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string initialDirectory = System.IO.Path.Combine(downloadsPath, "QR Codes");

                    if (!Directory.Exists(initialDirectory))
                    {
                        Directory.CreateDirectory(initialDirectory);
                    }

                    DateTime currentDateTime = DateTime.Now;
                    string fdt = currentDateTime.ToString("yyyy-MM-dd HH_mm_ss");
                    string fileName = "QRCode " + fdt + ".png";

                    fileName = CleanFileName(fileName);

                    string path = System.IO.Path.Combine(initialDirectory, fileName);
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    videoSource.NewFrame -= VideoSource_NewFrame;
                    videoSource.SignalToStop();
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            cameraImage.Source = null;
                            videoSource = null;
                            videoDevices = null;
                            myframe.frame.Content = new Scanned(bitmap, path, false, false);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "An Exception occured");
            }
        }

        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        private string CleanFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }



        private void Back(object sender, RoutedEventArgs e)
        {
            videoSource.SignalToStop();
            cameraImage.Source = null;
            videoSource = null;
            videoDevices = null;
            myframe.frame.Content = new ScanSelection();
        }

    }
}