
# Generation Strategies 

Not all important information can be recorded on the grid. With particularly challenging puzzles, it can sometimes be helpful to write down our deductions as additional clues to which we can apply our other strategies.

This is the purpose of the *constraint generation* strategies. They take the existing constraints of our puzzle and transform them into different, albeit equivalent constraints that other strategies might be able to make better use of. 

*LogikGen* currently has three such strategies: [LessThan Transitive Constraint Generation](LessThanTransitiveConstraintGenerationStrategy.md), [EitherOr Transitive Constraint Generation](EitherOrTransitiveConstraintGenerationStrategy.md), and [LessThan/NextTo Transitive Constraint Generation](LessThanNextToTransitiveConstraintGenerationStrategy.md).

## LessThan Transitive Constraint Generation Strategy

This strategy is based on the transitivity property of mathematics: if `x`, `y`, and `z` are three numbers such that `x < y` and `y < z`, then we can conclude `x < z`.

Whenever we have two constraints of the form `LessThan(X, Y) & LessThan(Y, Z)`, we can immediately conclude `LessThan(X, Z)`. As an example, consider the clues:

- The Englishman lives to the left of the blue house.  
    `LessThan(Englishman, Blue)`
    
- The blue house lies to the left of the man who keeps snails.  
    `LessThan(Blue, Snails)`

From these, we can immediately conclude:   

- The Englishman lives to the left of the man who keeps snails.  
    `LessThan(Englishman, Snails)`.

Other *LogikGen* strategies will then be able to make use of this new clue.

### Variants

There are two variants of the *LessThan Transitive Constraint Generation Strategy*, differing only by how the two initial `LessThan` constraints are matched up.

- Direct 
    
    The example above illustrates only the easier, albeit less powerful *Direct* variant which recognizes only constraints of the form `LessThan(X, Y) & LessThan(Y, Z)`.
    
- Indirect (Equal Only)  

    This more powerful, but slightly more difficult variant is able to recognize constraints of the form `LessThan(X, Y1) & LessThan(Y2, Z)` where `Y1` and `Y2` are associated. As an example, consider the clues:
    
    - The Englishman lives to the left of the Ukrainian.  
        `LessThan(Englishman, Ukrainian)`
        
    - The blue house lies to the left of the man who keeps snails.  
        `LessThan(Blue, Snails)`
        
    If it is known that the man in the blue house is the Ukrainian, then we can immediately conclude:
    
    - The Englishman lives to the left of the man who keeps snails.  
        `LessThan(Englishman, Snails)`
        
    The *Direct* variant would be unable to take into account whether or not the man in the blue house was the Ukrainian.

