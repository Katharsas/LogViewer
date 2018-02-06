using GalaSoft.MvvmLight.Threading;
using System;
using System.IO;
using System.Text;

namespace LogViewer.LogViewer.Loader
{
    /// <summary>
    /// A collection of callbacks that an observer must provide for file content loaders to call.
    /// </summary>
    interface IFileContentObserver
    {
        /// <summary>
        /// Gets called once to present content of file(s). If watcher is enabled, gets additionally called whenever lines are added to the file(s).
        /// </summary>
        /// <param name="lines">The file(s) (additional) contents.</param>
        void onLinesAdded(string lines);

        /// <summary>
        /// Gets called when a watched files content's removal has been detected.
        /// </summary>
        /// <returns>True if the watcher should continue to run and report newly added content since removal, false otherwise.</returns>
        bool onFileCleared();

        /// <summary>
        /// Gets called when an IOException occurs during any file reads. Occuring IOExceptions automatically disable watcher.
        /// </summary>
        /// <param name="error">The IOException which occured.</param>
        void onError(IOException error);
    }

    class SingleFileWatcher
    {
        private readonly string pathToFile;
        private long currentPos = 0;

        private readonly FileSystemWatcher watcher;
        private readonly object watcherLock = new object();

        private IFileContentObserver contentObserver;

        public SingleFileWatcher(string pathToFile, IFileContentObserver contentObserver, bool isWatcherEnabled)
        {
            this.pathToFile = pathToFile;
            this.contentObserver = contentObserver;

            checkForAdditionalLines();

            if (isWatcherEnabled)
            {
                watcher = new FileSystemWatcher();
                watcher.Path = Path.GetDirectoryName(pathToFile);
                watcher.Filter = Path.GetFileName(pathToFile);
                watcher.NotifyFilter = NotifyFilters.Size;
                watcher.EnableRaisingEvents = isWatcherEnabled;
                watcher.Changed += (sender, args) => checkForAdditionalLines();
            }
            else
            {
                watcher = null;
            }
            
        }

        public void disableWatcher()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
            }
        }

        private void checkForAdditionalLines()
        {
            lock (watcherLock)
            {
                try
                {
                    using (var file = File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        Console.WriteLine("Pos: " + currentPos);
                        Console.WriteLine("Len: " + file.Length);

                        if (currentPos > file.Length)
                        {
                            bool continueWatcher = contentObserver.onFileCleared();
                            if (!continueWatcher)
                            {
                                disableWatcher();
                                return;
                            }
                            currentPos = 0;
                        }
                        else if (currentPos == file.Length)
                        {
                            return;
                        }

                        file.Position = currentPos;

                        using (var streamReader = new StreamReader(file, Encoding.Default, true))
                        {
                            string lines = streamReader.ReadToEnd();
                            currentPos = file.Position;
                            contentObserver.onLinesAdded(lines);
                        }
                    }
                }
                catch (IOException ex)
                {
                    disableWatcher();
                    contentObserver.onError(ex);
                }
            }
        }
    }

}
