using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUI2.Utils
{
    // Taken from: https://stackoverflow.com/questions/534575/how-do-i-invert-booleantovisibilityconverter
    public sealed class CustomizableBooleanToVisibilityConverter
        : BooleanConverter<Visibility>
    {
        public CustomizableBooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }
}
