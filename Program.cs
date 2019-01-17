using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.Security;
using System.Runtime.CompilerServices;
using ExCSS;

namespace MMCSS {
    class Program {

        static void Main (string[] args) {
            restore ();
            Environment.Exit (0);

            List<FileInformation> AllCssFiles = getFileList ();

            backup (AllCssFiles);
            Console.WriteLine ("目前一共" + AllCssFiles.Count.ToString () + "个主题样式!");
            foreach (FileInformation item in AllCssFiles) {
                Console.WriteLine ("正在处理:" + item.FullName);
                ExCSS.Stylesheet css = new ExCSS.StylesheetParser ().Parse (item.Content);
                StyleRule stagestyle = css.Children.Single (x => ((StyleRule) x).SelectorText == "stage") as StyleRule;
                if (stagestyle != null) {
                    string aa = stagestyle.Style.FontFamily;
                    string pp = stagestyle.Style.FontSize;
                    stagestyle.Style.SetPropertyValue ("font-family", "Microsoft YaHei");
                    stagestyle.Style.SetPropertyValue ("font-size", "12pt");

                    try {
                        using (TextWriter writer = File.CreateText (item.FullName)) {
                            writer.Write (css.ToCss ());
                        }
                    } catch (System.Exception e) {
                        if (e is System.UnauthorizedAccessException) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine ("修改:" + item.FullName + " 时,执行权限不够,请提权后重试!");
                            System.Environment.Exit (0);
                        }
                    }

                }

            }

        }
        #region 
        //获取所有的CSS文件
        public static List<FileInformation> getFileList () {
            String usrpath = "/usr/share/themes";
            String userpath = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/.themes";
            DirectoryInfo di = new DirectoryInfo (usrpath);
            DirectoryInfo di2 = new DirectoryInfo (userpath);
            FileInfo[] fs = di.GetFiles ("cinnamon.css", SearchOption.AllDirectories);
            FileInfo[] fs2 = di2.GetFiles ("cinnamon.css", SearchOption.AllDirectories);
            List<FileInformation> Files = new List<FileInformation> ();
            foreach (var item in fs) {
                Files.Add (new FileInformation (item.DirectoryName, item.Name, item.OpenText ().ReadToEnd ()));
            }
            foreach (var item in fs2) {
                Files.Add (new FileInformation (item.DirectoryName, item.Name, item.OpenText ().ReadToEnd ()));
            }
            return Files;
        }

        static void backup (List<FileInformation> sourceFileList) {

            String NewName = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss.backup");
            using (TextWriter writer = File.CreateText (NewName)) {
                writer.Write (Newtonsoft.Json.JsonConvert.SerializeObject (sourceFileList));
            }
        }

        static void restore () {

            DirectoryInfo Di = new DirectoryInfo (Directory.GetCurrentDirectory ());
            FileInfo[] BackupFiles = Di.GetFiles ("*.backup").OrderBy (x => x.CreationTime).ToArray ();

            if (BackupFiles.Length == 0) {
                Console.WriteLine ("没有备份");
            } else {
                bool done = false;
                ConsoleColor curreColor = Console.ForegroundColor;
                do {
                    Console.ForegroundColor = curreColor;
                    Console.WriteLine ("备份文件列表");
                    for (int i = 0; i < BackupFiles.Length; i++) {
                        Console.WriteLine ((i + 1).ToString () + ") " + BackupFiles[i].Name);
                    }
                    Console.WriteLine ("请输入序号 (0 to exit): ");
                    string strSelection = Console.ReadLine ();
                    int iSel;
                    try {
                        iSel = int.Parse (strSelection);
                    } catch (FormatException) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine ("输入错误!");
                        continue;
                    }
                    switch (iSel) {
                        case 0:
                            done = true;
                            break;
                        default:
                            if (iSel <= BackupFiles.Length) {
                                FileInfo selectedFile = BackupFiles[iSel - 1];
                                Console.WriteLine ("确定使用{0}进行还原?(Y/N)", selectedFile.Name);
                                string iresponse = Console.ReadLine ();
                                if (iresponse.ToUpper () == "Y") {
                                    Console.WriteLine ("执行还原...");
                                    string jsonstring = selectedFile.OpenText ().ReadToEnd ();
                                    List<FileInformation> backlists = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileInformation>> (jsonstring);
                                    foreach (var item in backlists) {
                                        Console.Write (item.Content);
                                    }

                                }
                            } else {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine ("输入错误!");
                            }
                            continue;
                    }
                    Console.WriteLine ();
                } while (!done);

            }

        }

        #endregion

    }
}