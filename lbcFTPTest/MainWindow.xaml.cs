using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Atp.Net;
using Atp.IO;
using System.Diagnostics;

namespace lbcFTPTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FTP ftpClient = new FTP();

        public MainWindow()
        {
            InitializeComponent();
            ftpClient.DownloadProgressUpdate += new FTP.OnDownloadProgressUpdate(ftpClient_DownloadProgressUpdate);
            ftpClient.DownloadComplete += new FTP.OnDownloadComplete(ftpClient_DownloadComplete);
        }

        void ftpClient_DownloadComplete(object sender, Atp.AsyncMethodCompletedEventArgs e)
        {
            lblProgress.Content = "Download Complete!";
        }

        void ftpClient_DownloadProgressUpdate(object sender, FileSystemProgressEventArgs e)
        {
            lblProgress.Content = e.Percentage + "%";
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            ftpClient.CreateDirectory();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            ftpClient.BeginDownload("Test", "test");
        }
    }
}
