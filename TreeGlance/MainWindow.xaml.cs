using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace TreeGlance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TreeGlanceApp treeGlance = new TreeGlanceApp();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            #region CommonOpenFileDialog
            //Install-Package WindowsAPICodePack-Core
            //Install-Package WindowsAPICodePack-ExtendedLinguisticServices
            //Install-Package WindowsAPICodePack-Sensors
            //Install-Package WindowsAPICodePack-Shell
            //Install-Package WindowsAPICodePack-ShellExtensions
            #endregion

            #region Receiving folders' and subfolders' paths
            try
            {
                CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
                folderDialog.IsFolderPicker = true;
                CommonFileDialogResult result = folderDialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                    treeGlance.WriteSubpathsSafe(folderDialog.FileName, true);
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}"); }
            #endregion
            try
            {
                treeGlance.WriteFilesData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
