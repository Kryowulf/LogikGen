using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.Viewmodels
{
    public class PropertyDefinitionViewModel : ViewModel
    {
        private string _name = "";
        public string Name { 
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
    }
}
