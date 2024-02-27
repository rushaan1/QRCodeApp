using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace QRCodeApp
{
    class CheckIconHandler
    {
        private Grid grid;
        private Dispatcher dispatcher;
        public CheckIconHandler(Grid grid, Dispatcher dispatcher) 
        {
            this.grid = grid;
            this.dispatcher = dispatcher;
            Thread checkIconThread = new Thread(CheckIconManage);
            checkIconThread.Start();
        }

        private void CheckIconManage() 
        {
            dispatcher.Invoke(() =>
            {
                Image checkImage = new Image();
                checkImage.Source = new BitmapImage(new Uri("/Images/check.png", UriKind.Relative));
                checkImage.SetValue(Panel.ZIndexProperty, 1);
                checkImage.SetValue(Grid.ColumnProperty, 1);
                checkImage.Margin = new Thickness(335, 391, 38, 1);
                grid.Children.Add(checkImage);
                Trace.WriteLine("Added Image");

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2.5);
                timer.Tick += (sender, e) =>
                {
                    grid.Children.Remove(checkImage);
                    Trace.WriteLine("Removed Image");
                    timer.Stop();
                };

                timer.Start();
            });
        }
    }
}
