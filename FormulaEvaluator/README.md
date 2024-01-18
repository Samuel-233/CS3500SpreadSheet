```
Author:     Shu Chen
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Samuel-233
Repo:       https://github.com/uofu-cs3500-spring24/spreadsheet-Samuel-233
Date:       14/1/2024
Project:    Formula Evaluator
Copyright:  CS 3500 and Shu Chen - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

I think this code is not too efficient, it has many part can be optimized: like I think it shouldn't search for a usable number/variables, but just remove the null element. But I'm still thinking about how to deal with it.
Another problem is that I can't figure out how to use the Google docs algorithm, I used my own approach: use three stacks: first stack tracks front and back braces, another two track the operator with order(*/ before than +-). First use the first stack to locate the most inner equation, then use the operator to locate left and right value then get the result.

# Assignment Specific Topics


# Consulted Peers:

I didn't speak to anyone yet, I will do it if I have time. Most of my problems are solved by myself.

# References:

    1. C# Access String - https://www.w3schools.com/cs/cs_strings_access.php
    2. C# String to int - https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-convert-a-string-to-a-number
    3. How to know if a String is a number - https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/how-to-determine-whether-a-string-represents-a-numeric-value
    4. Trim String - https://www.geeksforgeeks.org/c-sharp-trim-method/