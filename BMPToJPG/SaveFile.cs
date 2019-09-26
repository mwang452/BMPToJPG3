using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMPToJPG
{
    class SaveFile : ISaveFile
    {
        string _fileName;
        string _folderPath;
        string _archiveFolderPath;

        public SaveFile(string folderPath)
        {
            _folderPath = folderPath;
            _fileName = Path.GetFileName(folderPath);
            string test = Path.GetDirectoryName(_folderPath);
            _archiveFolderPath = Path.GetDirectoryName(_folderPath) + @"\Archive\";

        }

        public static SaveFile CreateSaveFile(string folderPath)
        {
            return new SaveFile(folderPath);
        }

        public void SaveToArchiveFolder()
        {
            if (Directory.Exists(_archiveFolderPath))
            {
                MoveFile();
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(_archiveFolderPath);
                }
                catch (Exception ex)
                {
                    using (EventLog log = new EventLog())
                    {
                        log.Source = "BMPtoJPGConverter Encoder";
                        log.WriteEntry("There was a problem creating the Archive folder " + _archiveFolderPath + " " + ex.Message, EventLogEntryType.Error);

                    }
                }
                MoveFile();
            }
        }

        private void MoveFile()
        {
            try
            {
                if (File.Exists(_archiveFolderPath + _fileName))
                {
                    File.Delete(_archiveFolderPath + _fileName);
                }
                File.Move(_folderPath, _archiveFolderPath + _fileName);
            }
            catch (Exception ex)
            {
                using (EventLog log = new EventLog())
                {
                    log.Source = "BMPtoJPGConverter Encoder";
                    log.WriteEntry("There was a problem moving the file to " + _archiveFolderPath + " " + ex.Message, EventLogEntryType.Error);
                }
            }
        }
    }
}
