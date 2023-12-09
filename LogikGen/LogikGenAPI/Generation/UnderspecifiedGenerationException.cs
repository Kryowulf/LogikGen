using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation
{
    // Most generation exceptions are due to bugs in the LogikGen algorithms. 

    // One type, however, is due to the user's chosen settings making it impossible
    // to generate a puzzle - such as disabling all strategies.
    // This issue should be caught & handled separately from bugs.

    public class UnderspecifiedGenerationException : GenerationException
    {
        public UnderspecifiedGenerationException(string message) 
            : base(message)
        {
        }
    }
}
