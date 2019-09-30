using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BMPToJPG
{
    public partial class SignatureEncoder : ServiceBase
    {
        string folderName = string.Empty;

        public SignatureEncoder()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            folderName = args[0].ToString();
            Task.Run(() => BMPToJPGWatcher(args));
            
        }

        private void BMPToJPGWatcher(string[] args)
        {
            using (EventLog log = new EventLog())
            {
                log.Source = "Signature Encoder";
                log.WriteEntry("Starting watch on path: " + folderName);
            }
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = folderName;
                watcher.NotifyFilter = NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.CreationTime
                    | NotifyFilters.Attributes;
                watcher.Filter = "*.bmp";
                watcher.Created += OnChanged;
                watcher.EnableRaisingEvents = true;
                while (true) ;
            }    
        }

        protected override void OnStop()
        {
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            string changedFullPath = e.FullPath;

            using (IEncoderEngine encoderEngine = EncoderEngine.CreateEncoderEngine(changedFullPath))
            {
                encoderEngine.EncodeToJPG();
            }
            //ISaveFile saveFile = SaveFile.CreateSaveFile(changedFullPath);
            //saveFile.SaveToArchiveFolder();
        }

    }
}
