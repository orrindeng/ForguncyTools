using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Path = System.IO.Path;

namespace ForguncyTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            CollaborationTab.DataContext = new CollaborationViewModel();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            MessageBox.Show(exception.Message, "Forguncy Tools", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public CollaborationViewModel CollaborationViewModel => CollaborationTab.DataContext as CollaborationViewModel;

        private void CollaborationOpen_Click(object sender, RoutedEventArgs e)
        {
            CollaborationViewModel.Open();
        }

        private void CollaborationZipBack_Click(object sender, RoutedEventArgs e)
        {
            CollaborationViewModel.ZipBack();
        }

        private void CollaborationInitBare_Click(object sender, RoutedEventArgs e)
        {
            CollaborationViewModel.InitBare();
        }

        private void CollaborationClearLocalRepo_Click(object sender, RoutedEventArgs e)
        {
            CollaborationViewModel.ClearLocalRepo();
        }
    }

    public class CollaborationViewModel : ViewModelBase
    {
        private string _zipFilePath;
        public string ZipFilePath
        {
            get
            {
                return _zipFilePath;
            }
            set
            {
                if (_zipFilePath != value)
                {
                    _zipFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _tempFolder = ConfigurationManager.DefaultTempFolder;
        public string TempFolder
        {
            get
            {
                return _tempFolder;
            }
            set
            {
                if (_tempFolder != value)
                {
                    _tempFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ZipFullPath
        {
            get
            {
                var zipFile = ZipFilePath;
                if (!ZipFilePath.Contains(@":"))
                {
                    zipFile = Path.Combine(ConfigurationManager.AutoTestFilesPath, ZipFilePath);
                }
                return zipFile;
            }
        }

        public void Open()
        {
            EnsureFolderClean(ConfigurationManager.DefaultTempFolder);

            ZipFile.ExtractToDirectory(ZipFullPath, ConfigurationManager.DefaultTempFolder);
        }

        private void EnsureFolderClean(string folder)
        {
            Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);
        }

        public void InitBare()
        {
            EnsureFolderClean(ConfigurationManager.DefaultTempFolder);

            Repository.Init(TempFolder, true);
        }

        public void ZipBack()
        {
            ZipFile.CreateFromDirectory(TempFolder, ZipFullPath);
        }

        public void ClearLocalRepo()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ForguncyCollaboration", Path.GetFileName(TempFolder));
            Directory.Delete(folder, true);
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ConfigurationManager
    {
        private static string GetValue([CallerMemberName] string key = "")
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static string DefaultTempFolder => GetValue();
        public static string AutoTestFilesPath => GetValue();
    }
}
