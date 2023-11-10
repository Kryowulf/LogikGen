using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFUI2.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetValue<T>(ref T variable, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(variable, value))
            {
                variable = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
