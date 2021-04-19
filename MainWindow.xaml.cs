﻿using LibGit2Sharp;
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

namespace ForguncyAutoTestTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CollaborationTab.DataContext = new CollaborationViewModel();
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
            EnsureFolderClean();

            ZipFile.ExtractToDirectory(ZipFullPath, ConfigurationManager.DefaultTempFolder);
        }

        private void EnsureFolderClean()
        {
            Directory.Delete(ConfigurationManager.DefaultTempFolder, true);
            Directory.CreateDirectory(ConfigurationManager.DefaultTempFolder);
        }

        public void InitBare()
        {
            EnsureFolderClean();

            Repository.Init(TempFolder, true);
        }

        public void ZipBack()
        {
            ZipFile.CreateFromDirectory(TempFolder, ZipFullPath);
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
