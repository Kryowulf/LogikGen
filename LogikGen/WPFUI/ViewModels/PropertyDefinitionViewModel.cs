namespace WPFUI.ViewModels
{
    public class PropertyDefinitionViewModel : ViewModel
    {
        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set { SetValue(ref _propertyName, value); }
        }

        private bool _isVisible;
        public bool IsVisible
        { 
            get { return _isVisible; }
            set { SetValue(ref _isVisible, value); }
        }

        public PropertyDefinitionViewModel()
        {
            this.PropertyName = "";
            this.IsVisible = true;
        }
    }
}
