<h1 align="center"> Uranium.cs </h1>

This is the main file, this gets everything started, once the ` Emit() ` function is called.

As we walk through it, we can see that I check for the arguments parsed in via command line. If we have some, we treat the properly, if we have none, we return immediately, and tell the user they must specify a file or an input string.

Once we hit line 40, we hit the line that truly starts the entire process...
```cs
_syntaxTree = SyntaxTree.Parse(_text); 
```

This line calls for the syntax tree to parse the input text, which makes it so that the file gets lexed, which turns them into tokens, then those tokens get parsed into statements and variables. 

Once the lexer and the parser are done doing their job, we get to the next important line.

```cs
var compilation = _previous?.ContinueWith(_syntaxTree) ?? new Compilation(_syntaxTree);
```
This line is relatively complex, so let's break it down into two parts.

```cs
_previous?.ContinueWith(_syntaxTree)
```
This line states that `_previous` may be `null`, but if it isn't, we call `ContinueWith(_syntaxTree)`

The next part is 

```cs
?? new Compilation(_syntaxTree)
```

This states that if `_previous` is null, we make a new `Compilation(_syntaxTree)`, this way we can run the following line:

```cs
var result = compilation.Evaluate(variables);
```

Here, we call the for our new compilation that we've just created, to evaluate the current variables we have stored.

That's the biggest part, the rest is just dealing with the diagnostics we've accumulated, and then printing out the syntax tree.


<h4> Note: Template classes will <strong>NOT</strong> be covered in the READMEs </h4>
