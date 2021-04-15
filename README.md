<h1 align="center"> Uranium <img src="https://img.shields.io/github/workflow/status/Juptian/Compiler/.NET?style=plastic"></img> <img src="https://img.shields.io/badge/Language-C%23-blue"></img> <img src="https://img.shields.io/github/license/Juptian/Compiler?color=brightgreen&style=plastic"></img> </h1>
Uranium is a compiler that I decided to make from scratch, it is targetted at the CLR and will hopefully output IL code later. Currently, Uranium isn't finished

<h2 align="center"> Contributing </h2>

If you wish to contribute please do the following:
* Fork the project to your own public repo,
* Make the changes and commit them to the forked repo,
* Make a pull request to the **TESTING** branch,
* Then, I will personally review the pull request, I will give feedback on it, and then merge or decline it.

<h2 align="center"> Issues </h2>

If you have issues, please do the following:
* Create an issue with a short, yet descriptive title
* In the issue, please explain what the issue is in depth and how to recreate it

Finally, please be patient as I try to fix it. If it's not fixed quickly, you can always contribute to the repo by making a PR for it

<h2 align="center"> Using & Installing </h2>

To use the compiler, please make sure you have .NET 5 installed, once you've done that, do the following steps:
* Go to the ` test.txt ` file, it is located in the ` Main ` folder of the project, under ` Source `.
* Add text to that ` test.txt ` file
* Either run the ` run.bat ` file I have in that same directory, or run it via command line with ` dotnet run test.txt `

### Additional arguments:
The current command line arguments accepted are:
* --#showtree
This will show the syntax tree that is produced.

### How to use additional arguments:
To use additional arguments, simply type ` dotnet run test.txt ` then add each argument, separated by a space, so for example:
* ` dotnet run test.txt --#showtree `
