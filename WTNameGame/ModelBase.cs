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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
