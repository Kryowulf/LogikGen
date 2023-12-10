
# ![Logo](Files/Logo.png) *LogikGen* Logic Puzzle Generator

[Introduction](Introduction.md#logikgen-logic-puzzle-generator) - [Strategies](Strategies.md#logikgen-logic-puzzle-generator) - **Generation Settings**

# Generation Settings

## Table of Contents

- [Overview](#overview)
- [Heading](#heading)
- [Per-Strategy Settings](#per-strategy-settings)
- [Maximum Number of Constraints](#maximum-number-of-constraints)
- [Results](#results)
- [Unsolvable results](#unsolvable-results)

## Overview

The *Generation* tab of *LogikGen* contains many settings allowing you to fine-tune the resulting puzzle. 

It is perfectly fine to leave all settings on their default values, in which case the generator will create puzzles with no restrictions at all on the number of constraints or the types of strategies needed to solve them. Simply click the *Generate* button, wait 10 seconds, then click the *Cancel* button. The most difficult puzzle found within those 10 seconds will then be printed. 

The longer you leave *LogikGen* running, the more challenging the resulting puzzle will be. 

The purpose of each setting is discussed below. 

![Generation Tab](Files/GenerationTab.png)

## Heading

- Presets  
    Predefined settings for generating Easy, Medium, and Hard puzzles respectively. The *Medium* setting is most simliar to the traditional Zebra puzzle in regards to difficulty.

- Unsolvable  
    Applies a brute-force search to find a puzzle that is guaranteed to have a unique solution, but which none of the enabled strategies can solve. Such puzzles may require you to use a "guess and check" approach. Note that this is a great way to discover new strategies!
    
## Per-Strategy Settings

- Enabled  
    When unchecked, the selected strategy will not be used in the analysis of generated puzzles. No puzzle will require disabled strategies in order to solve. 
    
- Min  
    Makes the generator select only those puzzles which require a certain minimum number of applications of the selected strategy to solve. Increase this number to search for more difficult puzzles. 
    
- Max  
    Makes the generator select only those puzzles which require no more than the specified maximum number of applications of the selected strategy to solve. Suggest settings this to 1 for the hardest strategies, to ensure good fun puzzles that aren't insane.    

    **Setting `Max` to 0 is not the same as disabling the strategy!** It is common for a situation to arise where two different strategies could be applied to yield the same conclusions. The first strategy won't be counted as required if the second strategy could be used instead to solve the puzzle. Similarly, the second strategy won't be counted as required if the first could be used instead to solve the puzzle. 
    
    In other words, a puzzle may require at least one of the two strategies to solve, even if neither strategy on its own is required. Such puzzles will be misleadingly rated as easier than they really are.
    
## Maximum Number of Constraints

Since *LogikGen* puzzles are random, it can often happen that the final puzzle it chooses contains nothing but *LessThan* constraints, or nothing but *EitherOr* constraints. Such puzzles aren't very fun. You can influence the proportion and amount of each type of constraint by setting their desired maximum counts.

- Total  
    The generator will prefer puzzles which have no more than this number of constraints in total.
    
- Equal  
    The generator will prefer puzzles which have no more than this number of `Equal` constraints. 
    
- Distinct  
    The generator will prefer puzzles which have no more than this number of `Distinct` constraints. 
    
- Identity  
    The generator will prefer puzzles which have no more than this number of `Identity` constraints. 

- Less Than  
    The generator will prefer puzzles which have no more than this number of `LessThan` constraints
    
- Next To  
    The generator will prefer puzzles which have no more than this number of `NextTo` constraints.
    
- Either Or  
    The generator will prefer puzzles which have no more than this number of `EitherOr` constraints.

Leave these settings blank if you do not care to enforce any particular maximum.

## Results

The final result of the generation process is a *Generation Analysis Report* containing all information about that puzzle - the constraints, an analysis of the strategies required, the selected generation settings, the solution, and a step-by-step guide of how to solve the puzzle. If the process is still ongoing, only a partial report will be printed.

[Sample Report](Files/SampleReport.txt)

#### Satisfied vs. Unsatisfied

Within the top few lines of the report will be the message `All Targets Satisfied` or `Some Targets Unsatisfied`. 

    Report Generated 10:21:35 AM
    All Targets Satisfied
    Search Complete!

This refers to whether or not the puzzle described satisfies all of the desired targets - namely, the minimum/maximum number of applications for each enabled strategy and the maximum constraint counts. Once a puzzle is found that satisfies all targets, *LogikGen* will no longer select puzzles that do not satisfy them. 

#### Constraints

Next we're given the list of the constraints for the newly generated puzzle. In our sample output we have:

    +--------------------------------------------------------------+
    |                         Constraints                          |
    +--------------------------------------------------------------+

    6 total constraints.

    EitherOr(Fox, 4th, Green)
    EitherOr(Snails, Englishman, Yellow)
    Equal(Blue, 3rd)
    Identity(Dog, Spaniard, Blue, 4th)
    LessThan:Location(Red, Ukrainian)
    NextTo:Location(Green, Ukrainian)


These can be written in plain English as:

1. The fox owner lives either in the 4th house, or in the green house.  
    `EitherOr(Fox, 4th, Green)`
    
2. Snails are kept either by the Englishman, or by the owner of the yellow house.  
    `EitherOr(Snails, Englishman, Yellow)`
    
3. The blue house is the 3rd one from the left.  
    `Equal(Blue, 3rd)`
    
4. The four people are: the dog owner, the Spaniard, the owner of the blue house, and the owner of the 4th house.  
    `Identity(Dog, Spaniard, Blue, 4th)`
    
5. The red house is located somewhere to the left of where the Ukrainian lives.  
    `LessThan:Location(Red, Ukrainian)`

6. The green house is located next to where the Ukrainian lives.  
    `NextTo:Location(Green, Ukrainian)`

#### Strategies Required

Next we're given a list of the strategies required in order to solve the puzzle, along with the minimum number of times each strategy must be applied. It's important to note that this list is not exhaustive! As described in [The Paradox of Too Many Strategies](Strategies.md#the-paradox-of-too-many-strategies), if two different strategies can deduce the same information, then neither will be flagged as required even if at least one of them must be used.

    +--------------------------------------------------------------+
    |                     Strategies Required                      |
    +--------------------------------------------------------------+

    EitherOrDomainStrategy (Medium)
    2 application(s) needed.

    EitherOrImpliesDistinctStrategy (Easy)
    1 application(s) needed.

    EqualConstraintStrategy (Easiest)
    1 application(s) needed.

    IdentityConstraintStrategy (Easiest)
    1 application(s) needed.

    LessThanDomainStrategy (Medium)
    1 application(s) needed.

    LessThanImpliesDistinctStrategy (Easy)
    1 application(s) needed.

    NextToDomainStrategy (Medium)
    2 application(s) needed.

    NextToImpliesDistinctStrategy (Easy)
    1 application(s) needed.

    SynchronizeStrategy (Easy)
    3 application(s) needed.

#### Categories & Properties, Constraint Targets, Enabled Strategies, Solution

These sections are mostly self-explanatory and merely list the selected generation settings and the solution of the puzzle.

#### How To Solve

There are many ways to solve a given *Zebra* puzzle. Different people will employ different strategies at different times. This section merley shows one possible approach to arriving at the solution, using only those strategies which were enabled.

## "Unsolvable" Results

If the `Unsolvable` box was checked, a different kind of generation analysis report is printed.

[Sample 'Unsolvable' Report](Files/SampleUnsolvableReport.txt)

Any chosen generation targets - such as the constraint counts or the desired number of applications for each enabled strategy - are ignored. 

The purpose of the Unsolvable setting is purely to explore new strategies. It generates puzzles that are guaranteed to have a unique solution, but for which no enabled strategy can solve. This is how many of *LogikGen's* crazier strategies were discovered. 

In addition to the list of constraints, the solution, and the enabled strategies, a "partial solution" is also printed. This is a grid showing all information that the enabled strategies were able to deduce. From here, you're on your own. 

If you decide to tackle such puzzles, be forewarned that a lengthy brute-force "guess and check" approach will very likely be required.

---

[Introduction](Introduction.md#logikgen-logic-puzzle-generator) - [Strategies](Strategies.md#logikgen-logic-puzzle-generator) - **Generation Settings**
