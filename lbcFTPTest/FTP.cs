using Atp;
using Atp.Net;
using Atp.IO;
using System.Windows;
using System.Windows.Threading;
using System;

namespace lbcFTPTest
{
    class FTP
    {
        // ===============================================
        // Delegates
        // ===============================================
        public delegate void OnDownloadProgressUpdate(object sender, FileSystemProgressEventArgs e);
        public delegate void OnDownloadComplete(object sender, AsyncMethodCompletedEventArgs e);

        // ===============================================
        // Events
        // ===============================================
        public event OnDownloadProgressUpdate DownloadProgressUpdate;
        public event OnDownloadComplete DownloadComplete;

        // ===============================================
        // Private Variables
        // ===============================================
        private Ftp ftpClient = new Ftp();

        // ===============================================
        // Public Properties
        // ===============================================

        // ===============================================
        // Methods
        // ===============================================
        public FTP()
        {
            ftpClient.Progress += new FileSystemProgressEventHandler(ftpClient_Progress);
            ftpClient.DownloadFileCompleted += new System.EventHandler<AsyncMethodCompletedEventArgs>(ftpClient_DownloadFileCompleted);
        }

        private bool Connect(string server, string user, string pass)
        {
            try
            {
                //Check to see if we're already connected
                if (ftpClient.IsConnected)
                {
                    return true;
                }
                else
                {
                    //Try to connect to the FTP server
                    ftpClient.Connect(server);
                    ftpClient.Authenticate(user, pass);

                    if (ftpClient.IsConnected)
                    {
                        return true;
                    }             
                }

                //We couldn't connect to the FTP server
                return false;
            }
            catch
            {
                //An error occured while connecting to the server
                return false;
            }
        }

        public void BeginDownload(string source, string destination)
        {
            //TODO Change source and destination paths to read from parameters
            if (Connect("204.51.98.176", "lbc_app", "LbWv09"))
            {
                ftpClient.BeginDownloadFile("/034124/1a626806-0100-4ab0-a006-379af05ba9b4/Activity 17_3.pdf", "C:\\Users\\Will\\Desktop\\Activity 19_2.pdf");
            }
            else
            {
                //Failed to connect to FTP server
            }
        }

        public void CreateDirectory()
        {
            //TODO Change source and destination paths to read from parameters
            if (Connect("204.51.98.176", "lbc_app", "LbWv09"))
            {
                if (ftpClient.DirectoryExists("/222222/1a626806-0100-4ab0-a006-379af05ba9b4"))
                {
                    MessageBox.Show("Directory Exists!");
                }
                else
                {
                    ftpClient.CreateDirectory("/222222/1a626806-0100-4ab0-a006-379af05ba9b4");
                }
            }
            else
            {
                //Failed to connect to FTP server
            }
        }



        // ===============================================
        // Private Events
        // ===============================================
        void ftpClient_Progress(object sender, FileSystemProgressEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, new Action(() =>
                {
                    DownloadProgressUpdate(this, e);
                }));
            
        }

        void ftpClient_DownloadFileCompleted(object sender, AsyncMethodCompletedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, new Action(() =>
                {
                    DownloadComplete(this, e);
                    ftpClient.Disconnect();
                }));
            
        }
    }
}