using LogikGenAPI.Model;
using LogikGenAPI.Model.Constraints;
using LogikGenAPI.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace LogikGenAPI.Resolution.Strategies
{
    /*  
     *      Binary Constraint Analysis Strategy
     *      
     *      Take two arbitrary binary constraints sharing an argument,
     *      where the other arguments are known to be distinct. 
     *  
     *      T1(X, A) & T2(X, B)
     * 
     *      For every candidate position apos of A
     *          
     *          calculate the candidate positions bpos_set of B.
     *          
     *          if bpos_set is empty or the singleton {apos}
     *              then disassociate apos from A
     *              
     *              
     *      For every candidate position bpos of B
     *          
     *          calculate the candidate positions apos_set of A.
     *          
     *          if apos_set is empty or the singleton {bpos}
     *              then disassociate bpos from B
     *              
     *              
     *      For every candidate position xpos of X
     *          
     *          calculate the candidate positions 
     *          apos_set of A and bpos_set of B.
     *          
     *          if apos_set == bpos_set and both are singletons,
     *              then disassociate xpos from X.
     *              
     *          
     *      For this last loop, traditional domain strategies should
     *      have handled the case where apos_set or bpos_set are empty.
     *      
     *      I'm not 100% sure whether the first two loops should check 
     *      for whether apos_set or bpos_set are empty. 
     *      Is this / should this be handled by other strategies?
     *      
     */

    public class BinaryConstraintAnalysisStrategy : MultipleConstraintStrategy
    {
        public override StrategyClassification Classification => StrategyClassification.Other;
        public override bool AutoRepeat => true;

        public BinaryConstraintAnalysisStrategy(IndirectionLevel level) 
            : base(level)
        {
        }

        protected override bool ApplyOnce(PuzzleGrid grid, ConstraintSet cset, IPropertyComparer comparer)
        {
            int originalCount = grid.TotalUnresolvedAssociations;

            foreach (Category orderingCategory in cset.OrderedCategories)
            {
                List<OrderedBinaryConstraint> constraints = cset.OrderedBinaryConstraints(orderingCategory).ToList();

                for (int i = 0; i < constraints.Count - 1; i++)
                {
                    for (int j = i + 1 ; j < constraints.Count; j++)
                    {
                        OrderedBinaryConstraint T1 = constraints[i];
                        OrderedBinaryConstraint T2 = constraints[j];

                        Property A, X, B;
                        bool xBeforeA;
                        bool xBeforeB;

                        if (comparer.ProvenEqual(T1.Left, T2.Left))             // T1(X, A) & T2(X, B)
                        {
                            (X, A, B) = (T1.Left, T1.Right, T2.Right);
                            xBeforeA = true;
                            xBeforeB = true;
                        }
                        else if (comparer.ProvenEqual(T1.Left, T2.Right))       // T1(X, A) & T2(B, X)
                        {
                            (X, A, B) = (T1.Left, T1.Right, T2.Left);
                            xBeforeA = true;
                            xBeforeB = false;
                        }
                        else if (comparer.ProvenEqual(T1.Right, T2.Left))       // T1(A, X) & T2(X, B)
                        {
                            (A, X, B) = (T1.Left, T1.Right, T2.Right);
                            xBeforeA = false;
                            xBeforeB = true;
                        }
                        else if (comparer.ProvenEqual(T1.Right, T2.Right))      // T1(A, X) & T2(B, X)
                        {
                            (A, X, B) = (T1.Left, T1.Right, T2.Left);
                            xBeforeA = false;
                            xBeforeB = false;
                        }
                        else
                        {
                            continue;
                        }

                        if (!comparer.ProvenDistinct(A, B))
                            continue;

                        foreach (Property apos in grid[A, orderingCategory])
                        {
                            SubsetKey<Property> xdomain = xBeforeA ? T1.LeftDomainFrom(apos.Singleton)
                                                                    : T1.RightDomainFrom(apos.Singleton);

                            xdomain &= grid[X, orderingCategory];

                            SubsetKey<Property> bdomain = xBeforeB ? T2.RightDomainFrom(xdomain)
                                                                    : T2.LeftDomainFrom(xdomain);

                            bdomain &= grid[B, orderingCategory];

                            if (bdomain.IsEmpty || bdomain == apos.Singleton)
                            {
                                if (grid.Disassociate(A, apos))
                                    Logger.LogInfo($"{T1} & {T2} -> {A} != {apos}");
                            }
                        }

                        foreach (Property xpos in grid[X, orderingCategory])
                        {
                            SubsetKey<Property> adomain = xBeforeA ? T1.RightDomainFrom(xpos.Singleton)
                                                                    : T1.LeftDomainFrom(xpos.Singleton);

                            adomain &= grid[A, orderingCategory];

                            SubsetKey<Property> bdomain = xBeforeB ? T2.RightDomainFrom(xpos.Singleton)
                                                                    : T2.LeftDomainFrom(xpos.Singleton);

                            bdomain &= grid[B, orderingCategory];

                            if (adomain.IsEmpty || bdomain.IsEmpty || adomain == bdomain && adomain.Count == 1)
                            {
                                if (grid.Disassociate(X, xpos))
                                    Logger.LogInfo($"{T1} & {T2} -> {X} != {xpos}");
                            }
                        }

                        foreach (Property bpos in grid[B, orderingCategory])
                        {
                            SubsetKey<Property> xdomain = xBeforeB ? T2.LeftDomainFrom(bpos.Singleton)
                                                                    : T2.RightDomainFrom(bpos.Singleton);

                            xdomain &= grid[X, orderingCategory];

                            SubsetKey<Property> adomain = xBeforeA ? T1.RightDomainFrom(xdomain)
                                                                    : T1.LeftDomainFrom(xdomain);

                            adomain &= grid[A, orderingCategory];

                            if (adomain.IsEmpty || adomain == bpos.Singleton)
                            {
                                if (grid.Disassociate(B, bpos))
                                    Logger.LogInfo($"{T1} & {T2} -> {B} != {bpos}");
                            }
                        }
                    }
                }
            }

            return grid.TotalUnresolvedAssociations < originalCount;
        }
    }
}
