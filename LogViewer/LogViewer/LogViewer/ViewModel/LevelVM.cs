using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public class LevelVM : NotifyPropertyChanged
    {
        private bool _show;
        public bool Show
        {
            get { return _show; }
            set
            {
                _show = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value; 
                OnPropertyChanged();
            }
        }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand Remove { get; }

        public LevelVM(Action<LevelVM> onRemove)
        {
            Remove = new DelegateCommand(_ =>
            {
                onRemove(this);
            });
        }
    }
}
