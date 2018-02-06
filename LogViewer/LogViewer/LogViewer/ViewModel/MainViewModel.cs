using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using LogViewer.LogViewer.Model;
using Microsoft.Practices.ServiceLocation;
using WpfUtils.lib;
using LogViewer.LogViewer.Loader;
using Microsoft.Win32;

namespace LogViewer.LogViewer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public LogViewDataVM LogViewDataVM { get; }

        public SettingsVM SettingsVM { get; }

        public DelegateCommand OnExit { get; }

        public DelegateCommand OnLoadFile { get; set; }

        public DelegateCommand OnWatchFile { get; set; }

        public bool IsSettingsVisible { get; private set; } = true;
        public DelegateCommand ToggleSettings { get; }
        public string ToggleSettingsDesc { get; private set; }

        private SingleFileWatcher loader;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            // general
            OnExit = new DelegateCommand(_ =>
            {
                Application.Current.Shutdown();
            });

            // init
            LogView logView = ServiceLocator.Current.GetInstance<LogView>();
            LogViewDataVM = new LogViewDataVM(logView);
            SettingsVM = new SettingsVM(logView);

            // menu
            OnLoadFile = new DelegateCommand(_ =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string path = openFileDialog.FileName;
                    if (this.loader != null)
                    {
                        this.loader.disableWatcher();
                    }
                    logView.clear();
                    loader = new SingleFileWatcher(path, new MySimpleFileReader(logView), true);
                }
            });

            OnWatchFile = new DelegateCommand(_ =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string path = openFileDialog.FileName;
                    if (this.loader != null)
                    {
                        this.loader.disableWatcher();
                    }
                    logView.clear();
                    loader = new SingleFileWatcher(path, new MySimpleFileReader(logView), true);
                }
            });


            // toggle settings
            ToggleSettings = new DelegateCommand(_ =>
            {
                IsSettingsVisible = !IsSettingsVisible;
                setToggleButtonDesc(IsSettingsVisible);
            });
            setToggleButtonDesc(IsSettingsVisible);
        }

        private void setToggleButtonDesc(bool isSettingsVisible)
        {
            ToggleSettingsDesc = isSettingsVisible ? ">" : "<";
        }

        class MySimpleFileReader : IFileContentObserver
        {
            private readonly LogView logView;

            public MySimpleFileReader(LogView logView)
            {
                this.logView = logView;
            }

            public void onError(IOException error)
            {
                throw new NotImplementedException();
            }

            public bool onFileCleared()
            {
                logView.clear();
                return true;
            }

            public void onLinesAdded(string lines)
            {
                logView.parseAddLines(lines);
            }
        }
    }
}