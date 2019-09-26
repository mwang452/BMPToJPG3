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
    public partial class Service1 : ServiceBase
    {
        string folderName = string.Empty;
        string command = string.Empty;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(() => BMPToJPGWatcher(args));
        }

        private void BMPToJPGWatcher(string[] args)
        {

            if (args.Count() == 0 || folderName == "-?" || folderName.ToLower() == "-help")
            {
                Console.WriteLine("Example to use: BMPtoJPGConverter C:\\foldername\\. Watch the folder. \nUse -ALL flag to convert all the bmp files in folder. Example: BMPtoJPGConverter C:\\foldername\\ -ALL \nUse -SINGLE to process a single file.  Example:  BMPtoJPGConverter C:\\foldername\\filename -SINGLE");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (args.Count() > 0)
            {
                folderName = args[0].ToString(); //Folder to be watched.  If ? or help is entered it will show instructions.  If the -SINGLE command is used this will be the file name fully qualified path.
            }
            if (args.Count() > 1)
            {
                command = args[1].ToString(); //-ALL - process the entire folder.  -SINGLE - process 1 file. 
            }


            if (command == "-ALL")
            {
                Console.WriteLine("This will encode all BMP files in folder and may take a while.  Type p to proceed.");

                if (Console.ReadKey().Key == ConsoleKey.P)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Encoding folder : " + folderName);
                    EncodeAll(folderName);
                }
            }
            else if (command == "-SINGLE")
            {
                EncodeSingle(folderName);
            }
            else
            {
                using (EventLog log = new EventLog())
                {
                    log.Source = "BMPtoJPGConverter Encoder";
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
            ISaveFile saveFile = SaveFile.CreateSaveFile(changedFullPath);
            saveFile.SaveToArchiveFolder();
        }

        private static void EncodeAll(string folderName)
        {
            string[] filePathsArray = Directory.GetFiles(folderName, "*.bmp");

            foreach (string path in filePathsArray)
            {
                Console.WriteLine(path);
                using (IEncoderEngine encoderEngine = EncoderEngine.CreateEncoderEngine(path))
                {
                    encoderEngine.EncodeToJPG();
                }
                ISaveFile saveFile = SaveFile.CreateSaveFile(path);
                saveFile.SaveToArchiveFolder();
            }

            Console.WriteLine("Done Encoding.  Click space to exit.");
            Console.ReadKey();

        }
        private static void EncodeSingle(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (IEncoderEngine encoderEngine = EncoderEngine.CreateEncoderEngine(fileName))
                {
                    encoderEngine.EncodeToJPG();
                }
                ISaveFile saveFile = SaveFile.CreateSaveFile(fileName);
                saveFile.SaveToArchiveFolder();
                Console.WriteLine("Done encoding file: " + fileName);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Could not find file: " + fileName);
                Console.ReadKey();
            }
        }
    }
}
