using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogikGenAPI.Generation.Patterns
{
    public class DefaultConstraintPatternList : IReadOnlyList<ConstraintPattern>
    {
        private List<ConstraintPattern> _patterns;

        public ConstraintPattern this[int index] => _patterns[index];
        public int Count => _patterns.Count;
        public IEnumerator<ConstraintPattern> GetEnumerator() => _patterns.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _patterns.GetEnumerator();

        public DefaultConstraintPatternList()
        {
            _patterns = new List<ConstraintPattern>()
            {
                new DistinctConstraintPattern(),
                new EitherOrConstraintPattern(),
                new EqualConstraintPattern(),
                new IdentityConstraintPattern(),
                new LessThanConstraintPattern(),
                new NextToConstraintPattern()
            };
        }
    }
}
