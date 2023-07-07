using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Model
{
    public interface IPropertyComparer
    {
        public bool ProvenEqual(Property x, Property y);
        public bool ProvenDistinct(Property x, Property y);
    }
}
