using WpfUtils;
using LogViewer.LogViewer.Model;
using System.Windows;

namespace LogViewer.LogViewer.ViewModel
{
    public class LogViewDataVM : NotifyPropertyChanged
    {
        private MyBindingList<LogAtom> _logAtoms;

        public MyBindingList<LogAtom> LogAtoms
        {
            get { return _logAtoms; }
            set
            {
                _logAtoms = value;
                OnPropertyChanged();
            }
        }

        public LogViewDataVM(LogView logView)
        {
            LogAtoms = logView.Statements;
            LogAtoms.ListChanged += LogAtoms_ListChanged;


            try
            {
                DataTemplate template = (DataTemplate) Application.Current.FindResource("MyTemplate1");
            }
            catch (ResourceReferenceKeyNotFoundException ex)
            {
                ///stuff here to hande
            }


        }

        private void LogAtoms_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            OnPropertyChanged("LogAtoms");
        }
    }
}
