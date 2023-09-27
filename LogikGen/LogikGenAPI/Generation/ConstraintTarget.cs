using LogikGenAPI.Generation.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    public class ConstraintTarget
    {
        public ConstraintPattern Pattern { get; private set; }
        public int MaxCount { get; private set; }

        public ConstraintTarget(ConstraintPattern pattern, int maxOccurrences = int.MaxValue)
        {
            this.Pattern = pattern;
            this.MaxCount = maxOccurrences;
        }
    }
}
