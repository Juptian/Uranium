<h1 align="center"> CodeAnalysis.Syntax namespace </h1>

This namespace is for all things syntax related, other than lexing/parsing. The statement subfolder is merely a folder for our template classes.

<h3 align="center"> SyntaxFacts </h3>

The `SyntaxFacts` class is a class that contains all of our current syntax facts, as the name would suggest.

In this class, you'll find a lot of helper methods, for stuff like unary operator precedence, binary operator precedence, the `SyntaxKind` for keywords, and the keywords for `SyntaxKind`s


<h3 align="center"> SyntaxKind </h3>

The `SyntaxKind` enum is an enum containing all of our syntax values.

<h3 align="center"> SyntaxNode </h3>

The `SyntaxNode` class is our base class for stuff like our `ExpressionSyntax` class, and our `StatementSyntax` class, this holds very useful methods such as `GetChildren()`, and the `PrettyPrint()` method which is there to allow us to visualize our syntax tree with ease.

<h3 align="center"> SyntaxTree </h3>

The `SyntaxTree` class is the class in charge of calling on the parser to parse the tokens, and the parser then lexes them.