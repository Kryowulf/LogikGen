
# *LogikGen* Logic Puzzle Generator

[Introduction](Introduction.md#logikgen-logic-puzzle-generator) - **Strategies** - [Generation Settings](GenerationSettings.md#logikgen-logic-puzzle-generator)

# Strategies 

## Table of Contents

- [Overview](#overview)
- [Terminology](#terminology)
- [How LogikGen Works](#how-logikgen-works)
- [Strategies List](#strategies-list)
- [The Paradox of Too Many Strategies](#the-paradox-of-too-many-strategies)

## Overview

[Zebra puzzles](https://en.wikipedia.org/wiki/Zebra_Puzzle) have existed for many decades and numerous strategies have been discovered for solving them. *LogikGen* codifies many of these well-known strategies and also introduces some new ones. Note that other online resources for solving logic puzzles may refer to these strategies by different names. 

## Terminology

Before we get into the strategies, we should first go over a few terms.

- Category  
    The traditional Zebra puzzle has you deduce six types of information for each person - their nationality, their favorite beverage, their favorite brand of cigar, the type of pet they own, the color of their house, and where they live. Such types of information are known as *categories*. 
    
- Property  
    A *property* refers to a particular value within a category. For example, the `Color` category of the traditional Zebra puzzle contains the properties `Blue`, `Green`, `Ivory`, `Red`, and `Yellow`, while the `Beverage` category contains the properties `Coffee`, `Milk`, `Orange Juice`, `Tea` and `Water`.
    
- Category Size  
    The number of properties within each category of a puzzle is known as the *category size* of that puzzle. The traditional Zebra puzzle, for example, has a category size of 5. 
    
- Entity  
    In the traditional Zebra puzzle there are five different people for whom we try to deduce their properties. Once solved, you'll find that one of these people will have the properties `Englishman`, `3rd`, `Red`, `Milk`, `Old Gold`, and `Snails`. Another person will have the properties `Norwegian`, `1st`, `Yellow`, `Water`, `Kools`, and `Fox`. These five people represent the *entities* of the Zebra puzzle. In general, an *entity* refers to an abstract thing which we try to deduce the properties of. 
    
    Every entity will have exactly one property assigned to it from each category. For example, every person in a Zebra puzzle will always have exactly one nationality, exactly one favorite beverage, exactly one kind of pet, etc.. Furthermore, no two entities will be assigned the same property. You can never have two different people owning the same pet, for example, or having the same favorite beverage. 
    
- Constraint  
    A *constraint* is the form in which clues are represented in *LogikGen* puzzles. There are six different types of constraints recognized by *LogikGen*: `Equal`, `Distinct`, `LessThan`, `NextTo`, `EitherOr`, and `Identity`. Examples of each are as follows:

    - The Englishman lives in the red house.  
        `Equal(Englishman, Red)`
        
    - The Spaniard does not own the horse.  
        `Distinct(Spaniard, Horse)`
        
    - The Norwegian lives somewhere to the left of the man who drinks milk.  
        `LessThan(Norwegian, Milk)`
        
    - The man who smokes Chesterfields lives in the house next to the man with the fox.  
        `NextTo(Chesterfields, Fox)`
        
    - The man living in the ivory house is either the one who drinks orange juice, or the one who keeps the horse.  
        `EitherOr(Ivory, Orange Juice, Horse)`
        
    - The five people are: the Englishman, the man who lives in the green house, the man who drinks water, the man who smokes Lucky Strike, and the man who keeps the horse.  
        `Identity(Englishman, Green, Water, Lucky Strike, Horse)`
        
    In some cases, a single clue will require more than one constraint to fully describe. For example:
    
    - The green house is immediately to the right of the ivory house.  
        `LessThan(Ivory, Green) & NextTo(Ivory, Green)`

- Associate  
    When two properties belong to the same entity, they are said to be *associated* and we mark this with an `O` on our grid. The constraint `Equal(Englishman, Red)` associates the properties `Englishman` and `Red`.
    
- Disassociate  
    When two properties belong to different entities, they are said to be *disassociated* and we mark this with an `x` on our grid. The constraint `Distinct(Spaniard, Horse)` disassociates the properties `Spaniard` and `Horse`.

- Relationship  
    The *relationship* between two properties is whether they are associated or disassociated. 

- Ordered Category  
    A category whose properties can be sorted in some way - smallest to largest, youngest to oldest, leftmost to rightmost, etc... - is known as an *ordered category*. These categories are the basis of `LessThan` and `NextTo` constraints. Most puzzles will only have one such category. While *LogikGen* does support making puzzles having multiple ordered categories, this is an experimental feature which hasn't yet been thoroughly tested. In this guidebook, unless otherwise stated we will assume that all puzzles will have at most one ordered category.

- Position  
    A property from a puzzle's ordered category is referred to as a *position*. For example, each property from the Location category of our Zebra puzzle - 1st, 2nd, 3rd, 4th, and 5th - are the *positions* for that puzzle. Another puzzle could conceivably have an Age category whose properties are 25, 30, 35, 40, and 45, which in turn would be the *positions* for that puzzle.

## How *LogikGen* Works

*LogikGen* works by first creating a random set of constraints that fit a preset solution, then attempting to solve the puzzle given just those constraints. If the puzzle isn't solvable yet, more constraints are randomly added until it is. All unnecessary constraints are then removed. What's left is the final set of constraints for our newly generated puzzle.

An analysis process is then performed to identify the minimum number of times each strategy must be applied in order to solve the puzzle. 

Every strategy is assigned to one of seven difficulty ratings - *Easiest*, *Easier*, *Easy*, *Medium*, *Hard*, *Harder*, and *Hardest*. Puzzles which require more applications of the more difficult strategies are ranked higher over puzzles which require fewer.

This whole process of generating and ranking random puzzles is repeated many thousands of times. *LogikGen* will run nonstop until you click the "Cancel" button to halt the search, at which point all details of the highest ranked puzzle found will be printed out. The longer you leave *LogikGen* running, the more difficult the resulting puzzle is likely to be. 

On the [Generation Settings](GeneratingSettings.md) window, you can control the list of enabled strategies and a desired minimum or maximum number of applications for each, giving you more fine-tuned control over the resulting puzzle. 

## Strategies List

The strategies currently recognized by LogikGen are as follows. Despite their admittedly cryptic names, these strategies were designed to mimic the kinds of deductions a person would make when solving Zebra puzzles by hand, in order to more accurately estimate the perceived difficulty of the puzzles. No brute force "guess and check" will ever be needed to solve a *LogikGen* puzzle.

- Grid Only
    - [Synchronize Strategy](Strategies/SynchronizeStrategy.md)
    - [Property Pair Analysis Strategy](Strategies/PropertyPairAnalysisStrategy.md)
    - [Block Crossout Strategy](Strategies/BlockCrossoutStrategy.md)
    - [Pigeonhole Strategy](Strategies/PigeonholeStrategy.md)

- Basic Assertion
    - [Distinct Constraint Strategy](Strategies/BasicAssertions.md)
    - [Equal Constraint Strategy](Strategies/BasicAssertions.md)
    - [Identity Constraint Strategy](Strategies/BasicAssertions.md)
    
- Distinct Equivalent
    - [LessThan Implies Distinct Strategy](Strategies/DistinctEquivalent.md)
    - [NextTo Implies Distinct Strategy](Strategies/DistinctEquivalent.md)
    - [EitherOr Implies Distinct Strategy](Strategies/DistinctEquivalent.md)

- Simple Domain
    - [EitherOr Domain Strategy](Strategies/EitherOrDomainStrategy.md)
    - [LessThan Domain Strategy](Strategies/LessThanDomainStrategy.md)
    - [LessThan-Many Domain Strategy](Strategies/LessThanManyDomainStrategy.md)
    - [NextTo Domain Strategy](Strategies/NextToDomainStrategy.md)
    
- Double NextTo 
    - [Double NextTo Implies Between Strategy](Strategies/DoubleNextToImpliesBetweenStrategy.md)
    - [Double NextTo Implies Equal Strategy](Strategies/DoubleNextToImpliesEqualStrategy.md)
    
- Check Compatibility
    - [Immediate LessThan Compatibility Check Strategy](Strategies/ImmediateLessThanCompatibilityCheckStrategy.md)
    - [LessThan Many Compatibility Check Strategy](Strategies/LessThanManyCompatibilityCheckStrategy.md)
    - [NextTo Compatibility Check Strategy](Strategies/NextToCompatibilityCheckStrategy.md)
    - [NextTo Incompatibility Search Strategy](Strategies/NextToIncompatibilitySearchStrategy.md)
    
- Constraint Generation
    - [LessThan Transitive Constraint Generation Strategy](Strategies/LessThanTransitiveConstraintGenerationStrategy.md)
    - [EitherOr Transitive Constraint Generation Strategy](Strategies/EitherOrTransitiveConstraintGenerationStrategy.md)
    - [LessThan/NextTo Transitive Constraint Generation Strategy](Strategies/LessThanNextToTransitiveConstraintGenerationStrategy.md)
    
- Other
    - [Binary Constraint Analysis Strategy](Strategies/BinaryConstraintAnalysisStrategy.md)
    - [EitherOr Argument Union Strategy](Strategies/EitherOrArgumentUnionStrategy.md)


## The Paradox of Too Many Strategies

*LogikGen* was designed to assess puzzles according to the *minimum* number of times each strategy must be applied in order to solve them. It does this by continually resolving the same puzzle over and over, once for each strategy. The strategy under consideration is removed from the set of available strategies, then applied only when the other strategies cannot deduce any more information. 

The benefit to this approach is that it takes into account how different people, when solving by hand, will employ different strategies in a different order. If a person "gets lucky" by choosing to apply the most useful strategies first, they'll perceive the puzzle as much easier than someone who happened to have chosen the less useful strategies first. 

By counting only the minimum number of applications needed, we're able to establish a baseline - a level of difficulty that even the lucky person must overcome. 

Unfortunately, this creates a problem as more strategies are added to *LogikGen*.

It happens quite often that two different strategies can deduce the same information in certain situations. For example, strategy A won't be counted as required if strategy B can be used instead to solve a particular puzzle. Similarly, strategy B won't be counted as required if strategy A could be used instead. 

When this happens, neither strategy will be counted as required. Their minimum number of applications will be zero, even if the puzzle actually requires one of the two in order to solve. 

This phenomenon throws off the difficulty ranking and could cause quite challenging puzzles to inadvertently be flagged as easy. To mitigate this, it is recommended to disable those strategies you don't want. 

---

[Introduction](Introduction.md) - **Strategies** - [Generation Settings](GenerationSettings.md#logikgen-logic-puzzle-generator)

