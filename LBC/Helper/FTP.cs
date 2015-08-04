using Atp;
using Atp.Net;
using Atp.IO;
using System.Windows;
using System.Windows.Threading;
using System;

namespace LBC.Helper
{
    class FTP
    {
        // ===============================================
        // Delegates
        // ===============================================
        public delegate void OnProgressUpdate(object sender, FileSystemProgressEventArgs e);
        public delegate void OnDownloadComplete(object sender, AsyncMethodCompletedEventArgs e);
        public delegate void OnUploadComplete(object sender, AsyncMethodCompletedEventArgs e);

        // ===============================================
        // Events
        // ===============================================
        public event OnProgressUpdate ProgressUpdate;
        public event OnDownloadComplete DownloadComplete;
        public event OnUploadComplete UploadComplete;

        // ===============================================
        // Private Variables
        // ===============================================
        private DispatcherTimer tmrProgress = new DispatcherTimer();
        private Ftp ftpClient = new Ftp();
        private bool reportProgress = true;

        // ===============================================
        // Public Properties
        // ===============================================
        public bool IsBusy
        {
            get
            {
                return ftpClient.IsBusy;
            }
        }


        // ===============================================
        // Static Methods
        // ===============================================
        public static string GetFileExtension(string file)
        {
            return file.Substring(file.LastIndexOf("."));
        }

        public static string BuildDialogExtension(string file)
        {
            string extension = file.Substring(file.LastIndexOf("."));

            return String.Format("{0}|*{0}", extension);
        }

        public static string GetRemotePath(string file)
        {
            return file.Substring(0, file.LastIndexOf("/"));
        }

        // ===============================================
        // Methods
        // ===============================================
        public FTP()
        {
            tmrProgress.IsEnabled = false;
            tmrProgress.Interval = new TimeSpan(0, 0, 1);
            tmrProgress.Tick += new EventHandler(tmrProgress_Tick);

            ftpClient.Progress += new FileSystemProgressEventHandler(ftpClient_Progress);
            ftpClient.DownloadFileCompleted += new System.EventHandler<AsyncMethodCompletedEventArgs>(ftpClient_DownloadFileCompleted);
            ftpClient.UploadFileCompleted += new EventHandler<AsyncMethodCompletedEventArgs>(ftpClient_UploadFileCompleted);
            ftpClient.TransferConfirm += new TransferConfirmEventHandler(ftpClient_TransferConfirm);
        }

        /// <summary>
        /// The file has successfully been uploaded to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ftpClient_UploadFileCompleted(object sender, AsyncMethodCompletedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, new Action(() =>
                {
                    UploadComplete(this, e);
                    tmrProgress.IsEnabled = false;
                    ftpClient.Disconnect();
                }));
        }

        /// <summary>
        /// Confirm what to do in case of an error while transferring files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ftpClient_TransferConfirm(object sender, TransferConfirmEventArgs e)
        {
            switch (e.ConfirmReason)
            {
                case TransferConfirmReason.FileAlreadyExists:
                    //The file already exists on the server
                    if (MessageBox.Show("The file already exists, overwrite?", "File already exists", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        e.NextAction = TransferConfirmNextActions.Overwrite;
                    }
                    else
                    {
                        //Skip uploading the file
                        e.NextAction = TransferConfirmNextActions.Skip;
                    }
                    break;
            }
        }

        void tmrProgress_Tick(object sender, EventArgs e)
        {
            reportProgress = true;
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

        public bool BeginDownload(string source, string destination)
        {
            if (Connect(Session.CurDistrict.FtpServer, Session.CurDistrict.FtpUser, Session.CurDistrict.FtpPass))
            {
                //Make sure the file exists on the ftp server
                if (ftpClient.FileExists(source) == false)
                {
                    return false;
                }

                ftpClient.BeginDownloadFile(source, destination);
                tmrProgress.IsEnabled = true;
                return true;
            }
            else
            {
                //Failed to connect to FTP server
                return false;
            }
        }

        public bool BeginUpload(string source, string destination)
        {
            if (Connect(Session.CurDistrict.FtpServer, Session.CurDistrict.FtpUser, Session.CurDistrict.FtpPass))
            {
                //Make sure the directory exists
                if (ftpClient.DirectoryExists(GetRemotePath(destination)) == false)
                {
                    ftpClient.CreateDirectory(GetRemotePath(destination));
                }

                //Duplicate files will be handled in the transfer confirm event handler
                ftpClient.BeginUploadFile(source, destination);
                tmrProgress.IsEnabled = true;
                return true;
            }
            else
            {
                //Failed to connect to the FTP server
                return false;
            }
        }

        public bool DeleteFile(string file)
        {
            if (Connect(Session.CurDistrict.FtpServer, Session.CurDistrict.FtpUser, Session.CurDistrict.FtpPass))
            {
                //Make sure the file exists
                if (ftpClient.FileExists(file))
                {
                    ftpClient.DeleteFile(file);
                }

                return true;
            }
            else
            {
                //Failed to connect to the FTP server
                return false;
            }
        }



        // ===============================================
        // Private Events
        // ===============================================
        void ftpClient_Progress(object sender, FileSystemProgressEventArgs e)
        {
            if (reportProgress == true)
            {
                reportProgress = false;
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, new Action(() =>
                    {
                        ProgressUpdate(this, e);
                    }));
            }
        }

        void ftpClient_DownloadFileCompleted(object sender, AsyncMethodCompletedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, new Action(() =>
                {
                    DownloadComplete(this, e);
                    tmrProgress.IsEnabled = false;
                    ftpClient.Disconnect();
                }));
        }
    }
}