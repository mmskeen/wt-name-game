using System.ComponentModel;

namespace WTNameGame
{
    /// <summary>
    /// Base class for (inherited) objects to update their XAML bindings
    /// </summary>
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
