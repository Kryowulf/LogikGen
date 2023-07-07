
# Distinct Equivalent

**LessThan, NextTo, and EitherOr Constraints**

The *LessThan*, *NextTo*, and *EitherOr* constraints are all, in a sense, equivalent to *Distinct* constraints. Several strategies have been created to process them as such:

- LessThan Implies Distinct Strategy
- NextTo Implies Distinct Strategy
- EitherOr Implies Distinct Strategy

For example, consider the following clues:

- The Englishman lives to the left of the man who keeps foxes.  
    `LessThan(Englishman, Fox)`
    
- The Englishman lives next to the man who keeps foxes.  
    `NextTo(Englishman, Fox)`
    
- In the red house lives either the Englishman, or the man who keeps foxes.  
    `EitherOr(Red, Englishman, Fox)`
    
These all necessarily imply that the Englishman is not the one who keeps foxes.

The *EitherOr Implies Distinct Strategy* makes one other deduction when the second and third arguments of an `EitherOr` constraint are of the same category. Consider the following example:

- The Englishman lives in either the red house, or the green house.  
    `EitherOr(Englishman, Red, Green)`
    
In this situation, *LogikGen* will automatically disassociate the Englishman from all colors but red and green. 

