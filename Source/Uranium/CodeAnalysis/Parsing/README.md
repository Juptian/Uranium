<h1 align="center"> CodeAnalysis.Parsing namespace </h1>

This namespace only contains two files (including this one) and a single source file. The parser!

<h3 align="center"> Parser </h2>

The parser is the part of the puzzle piece that makes the lexer lex the file, and also makes the tokens that the lexer lexes actually have a meaning. In here we go token by token, lexing it, and until the lexer outputs an end of file token, we just keep going. There's a lot going on in this file, so let's break it down.

Line 50:
```cs
return _tokens[^1];
```

This line is just a fancy way of writing `return _tokens[_tokens.Length - 1];` I do this because overall I find it's cleaner, and once you know it, it's easier to read.

Line 90 - 99:
```cs
//If it doesn't fit any of our current conditions, we take it as an expression
private StatementSyntax ParseStatement()
    => Current.Kind switch
     {
        SyntaxKind.OpenCurlyBrace => ParseBlockStatement(),
        SyntaxKind.LetConstKeyword or 
        SyntaxKind.ConstKeyword or 
        SyntaxKind.VarKeyword => ParseVariableDeclaration(),
        _ => ParseExpressionStatement(),
    };
```
There's a lot going on in this function, what we're doing here, is we're checking what the `Current.Kind` is, if it matches one of our `SyntaxKind.` expressions, we call the according function, `_ => ParseExpressionStatement()` is just a fancy way of writing `if it doesn't fit any of the previous conditions, just call ParseExpressionStatement()`

<h4 align="center"> Block statements </h4>

In the `ParseBlockStatement()`, we loop for as long as we haven't reached the ` EndOfFile ` token, or a closing curly brace (`}`). 

If we meet a line break, or a semi colon, we just move to the next position, and then keep going, this is because we have no reason to read those.

If it's not a semi colon, or a line break, we call `ParseStatement()` which I cover further down. Once we're done that, we add the statement that we got from `ParseStatement()` and add it to our current statements list to be bound and evaluated.

Then, we match the current token to ` CloseCurlyBrace `, and then we continue moving.

Now I'm sure that by this point, you're wondering why I use `MatchToken()` over just assigning it to our current value. This causes two problems.

* We do not advance tokens
* If it's not the expected token, there is no error

We don't want either of these to occur, because if we do not advance tokens, we will get caught in an infinite loop. This is, as I'm sure you guess, <strong>really really bad</strong> because it'll cause your computer to waste memory on a process that will never end. 

<h4 align="center"> Variable Declarations </h4>

This method is pretty short, all we do, is match a keyword, match an identifier (the variable name), and then match the equals token. Then, we read whatever expression is on the other side of the equals sign. After that, we symply return a new `VariableDeclarationSyntax` with all of our variables.

<h4 align="center"> ParseExpressionStatement and ParseExpression </h4>

These two are simply wrapper functions, though, `ParseExpressionStatement()` returns a `StatementSyntax` over a normal expression syntax. These really just call `ParseAssignmentExpression()`

<h4 align="center"> ParseAssignmentExpression </h4>

In this method, I have a visual graph of how I want to do assignment. I have a simple assignment expression, that I make a tree for. Here is the comment:

```
//Assignments will be done like this:
//
//a = b = 5
//
//
//   =
//  / \
// a   =
//    / \
//    b  5
//Checking for an identifier token, and a double equals
//this way we can actually assign two identifiers
```

Here we check if the `Current` token type is an identifier token, and if the `Next` token is an equals token, then, we get the identifier token (the current one), and the operator token (the next token), then the right is a recursive call to `ParseAssignmentExpression()`, and this cycle repeats until the if check returns false.

If it returns false, we call `ParseBinaryExpression`

<h4 align="center"> ParseBinaryExpression </h4>
At the beginning of the method, we declare an `ExpressionSyntax` called left, this is so that we can assign it later based off of what is going on.

Here's the first part of the function
```csharp
//Checking to see if it's a unary operator
var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
//Allowing for unary operator precedence
if(unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
{
    //Eat tokens for breakfast
    var operatorToken = NextToken();
    //Recursively calling ParseBinaryExpression so that we can keep up
    //With out unary operators
    var operand = ParseBinaryExpression(unaryOperatorPrecedence);
    left = new UnaryExpressionSyntax(operatorToken, operand);
}
else
{
    //If UnaryOperatorPrecedence is greater than or equal to ParentPrecedence, we don't get here, if it isn't
    //We then walk the left side of our tree again
    left = ParsePrimaryExpression();
}
```

Here, the comments explain the code very well. We get our `unaryOperatorPrecedence` by getting it from the `SyntaxFacts` class, and then we do some checks, I'll let the comments speak for themselves here.

In general, in this file, the comments describe what's going on quite well.

In the `while()` loop I have, I am just looping over the tree, walking to the right side every time until eventually, we don't have an operator or binary, or the precedence for the operator is less than our current precedence.

<h4 align="center"> ParsePrimaryExpression </h4>
Here, we do another comparison of the type, because here we have a split road, we can have either a number literal (ie: 1, 2, 0249, 142 409, ect), a parethesized expression (ie 1 + (2 + 2)), a boolean expression so `true` or `false`, and if it fits none of the above, we assume it's a name expression. The rest of the methods don't have much to them, so I just won't talk about them, if you want further specifications, feel free to make an issue and I'll gladly respond to questions!