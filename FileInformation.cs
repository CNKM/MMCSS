using System;
namespace MMCSS {
    public class FileInformation {
        String _FileName;
        String _Content;

        String _PathName;

        String _FullName;

        public FileInformation (string directoryName, string filename, string content) {
            this.PathName = directoryName;
            this.FileName = filename;
            this.Content = content;
            this.FullName = directoryName + "/" + this.FileName;
        }

        public String Content { get => _Content; set => _Content = value; }
        public String FileName { get => _FileName; set => _FileName = value; }
        public String PathName { get => _PathName; set => _PathName = value; }
        public string FullName { get => _FullName; set => _FullName = value; }
    }
}