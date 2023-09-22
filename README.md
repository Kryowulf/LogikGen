
# *LogikGen* Logic Puzzle Generator

*LogikGen* is a tool for creating randomized [Zebra-style logic puzzles](https://en.wikipedia.org/wiki/Zebra_Puzzle).

For more information, check out the [guidebook introduction](./Guidebook/Introduction.md).

## Changelog

* v1.0.1 - Bugfix  

    * **Fixed**:  

        * When generating small puzzles using few strategies, the displayed progress would update too frequently and cause the UI to become unresponsive. 

    * **Known Issues**:
    
        * The generator has difficulty satisfying the maximum constraint count requirements for large puzzles. 
        
        * When two different strategies can deduce the same information, neither strategy will be flagged as required. This can cause puzzles to be ranked as much easier than they really are. This can also make it difficult or impossible in some situations to generate puzzles that require particular strategies.
        

* v1.0.0 - Initial Release


