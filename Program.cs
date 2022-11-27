using System.CommandLine;

var rootCommand = new RootCommand("Advent of Code in C#");

return await rootCommand.InvokeAsync(args);