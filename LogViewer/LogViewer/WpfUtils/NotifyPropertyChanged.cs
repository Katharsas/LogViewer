using System.ComponentModel;
using System.Runtime.CompilerServices;

// ReSharper disable CheckNamespace
namespace WpfUtils
{
    /// <summary>
    /// 
    /// This implementation of <see cref="INotifyPropertyChanged"/> is not needed if attribute
    /// [ImplementPropertyChanged] is used on a class, but can be combined with it to allow 
    /// attaching event listeners without reflection.
    /// 
    /// If only [ImplementPropertyChanged] is used, it will weave this implementation in by itself.
    /// 
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
