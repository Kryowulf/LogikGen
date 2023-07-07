using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Resolution.Strategies
{
    public abstract class ConstraintGenerationStrategy : MultipleConstraintStrategy
    {
        protected ConstraintGenerationStrategy(IndirectionLevel level) 
            : base(level)
        {
        }
    }
}
