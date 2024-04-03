using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CabinetBilder_UI
{
    public class PopUp
    {
        public static string OpenFolderDialog()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select the save folder";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string exportFolder = folderBrowser.SelectedPath;
                if (!System.IO.Directory.Exists(exportFolder))
                {
                    System.IO.Directory.CreateDirectory(exportFolder);
                }
                return exportFolder;
            }
            return string.Empty;
        }

        public static string OpenFolderDialogFilename(string fileName, string extension)
        {
            string folderName = OpenFolderDialog();
            if (folderName != null && folderName != "")
            {
                return System.IO.Path.Combine(folderName, $"{fileName}.{extension}");
            }
            return string.Empty;
        }
    }
}
