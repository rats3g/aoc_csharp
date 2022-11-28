namespace AdventOfCode;

internal interface ISolution
{
    Task<string> SolvePartOne(string inputFile);

    Task<string> SolvePartTwo(string inputFile);
}
