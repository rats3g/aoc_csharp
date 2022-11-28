using AdventOfCode;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Net;
using System.Reflection;

var handler = new HttpClientHandler
{
    CookieContainer = new CookieContainer()
};

var client = new HttpClient(handler);
      
var rootDirectory = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\.."));

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

var rootCommand = new RootCommand("Advent of Code in C#");

var yearOption = new Option<int>(
        aliases: new[] { "-y", "--year" },
        description: "Puzzle set year",
        isDefault: true,
        parseArgument: result =>
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            if (result.Tokens.Count == 0 && currentMonth == 12)
            {
                return currentYear;
            }

            if (result.Tokens.Count == 1 &&
                int.TryParse(result.Tokens.Single().Value, out var year) &&
                year > 2015 &&
                year <= currentYear)
            {
                return year;
            }

            result.ErrorMessage = $"Select a valid puzzle set year (2015 - {(currentMonth == 12 ? currentYear : currentYear - 1)})";
            return currentYear;
        }
    );

rootCommand.AddOption(yearOption);

var dayOption = new Option<int>(
        aliases: new[] { "-d", "--day" },
        description: "Puzzle set day",
        isDefault: true,
        parseArgument: result =>
        {
            var currentDay = DateTime.Now.Day;

            if (result.Tokens.Count == 0 && currentDay < 26)
            {
                return currentDay;
            }

            if (result.Tokens.Count == 1 &&
                int.TryParse(result.Tokens.Single().Value, out var day) &&
                day > 0 &&
                day <= 26)
            {
                return day;
            }

            result.ErrorMessage = "Select a valid puzzle set day (1 - 25)";
            return 1;
        }
    );

rootCommand.AddOption(dayOption);

var sessionOption = new Option<string>(
        aliases: new[] { "-s", "--session" },
        description: "Login session cookie",
        isDefault: true,
        parseArgument: result =>
        {
            if (result.Tokens.Count != 0)
            {
                return result.Tokens[0].Value;
            }

            var sessionFile = Path.Combine(rootDirectory, "session.txt");
            if (File.Exists(sessionFile))
            {
                return File.ReadAllText(sessionFile);
            }

            var sessionVariable = Environment.GetEnvironmentVariable("AOC_SESSION");
            if (sessionVariable != null)
            {
                return sessionVariable;
            }

            result.ErrorMessage = "Specify session cookie using command line argument (--session), file (session.txt), or environment variable (AOC_SESSION)";
            return string.Empty;
        }
    );

rootCommand.AddOption(sessionOption);

rootCommand.SetHandler(async (year, day, session) =>
{
    logger.LogInformation("Running Advent of Code {year} - Day {day}", year, day);

    var problemDirectoryInfo = CreateFolderStructure(year, day);

    CreateSolutionClass(problemDirectoryInfo, year, day);
    CreateTestClass(problemDirectoryInfo, year, day);

    handler.CookieContainer.Add(new Uri("https://adventofcode.com/"), new Cookie("session", session));

    await DownloadInputFile(problemDirectoryInfo, year, day);

    var solution = await ExecuteSolution(problemDirectoryInfo, year, day);
    if (solution != null)
    {
        logger.LogInformation("Solution to part one is {solution1}", solution.Value.solution1);
        logger.LogInformation("Solution to part two is {solution2}", solution.Value.solution2);
    }
},
yearOption, dayOption, sessionOption);

return await rootCommand.InvokeAsync(args);

DirectoryInfo CreateFolderStructure(int year, int day) => Directory.CreateDirectory(Path.Combine(rootDirectory, year.ToString(), day.ToString()));

void CreateSolutionClass(DirectoryInfo directoryInfo, int year, int day)
{
    var solutionFile = Path.Combine(directoryInfo.FullName, $"Day{day}_{year}.cs");
    if (File.Exists(solutionFile)) return;    

    var lCurly = "{";
    var rCurly = "}";
    File.WriteAllText(solutionFile,
        $"""
        namespace AdventOfCode;
        
        internal class Day{day}_{year} : ISolution
        {lCurly}
            public async Task<string> SolvePartOne(string inputFile)
            {lCurly}
                return "unsolved";
            {rCurly}

        public async Task<string> SolvePartTwo(string inputFile)
            {lCurly}
                return "unsolved";
            {rCurly}
        {rCurly}
        """);
}

void CreateTestClass(DirectoryInfo directoryInfo, int year, int day)
{
    var testFile = Path.Combine(directoryInfo.FullName, $"Day{day}_{year}Should.cs");
    if (File.Exists(testFile)) return;

    var lCurly = "{";
    var rCurly = "}";
    File.WriteAllText(testFile,
        $"""
        using Xunit;

        namespace AdventOfCode;
        
        [Trait("Year", "{year}")]
        [Trait("Day", "{day}")]
        public class Day{day}_{year}Should
        {lCurly}
            [Fact]            
            public void HaveATest()
            {lCurly}
                
            {rCurly}
        {rCurly}
        """);
}

async Task DownloadInputFile(DirectoryInfo directoryInfo, int year, int day)
{
    var inputFile = Path.Combine(directoryInfo.FullName, "input.txt");
    if (File.Exists(inputFile)) return;

    var url = $"https://adventofcode.com/{year}/day/{day}/input";

    try
    {
        var response = await client.GetStringAsync(url);
        File.WriteAllText(inputFile, response);
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "Unable to download input file from {url}", url);
    }        
}

async Task<(string solution1, string solution2)?> ExecuteSolution(DirectoryInfo directoryInfo, int year, int day)
{
    var solutionClass = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.Name == $"Day{day}_{year}").SingleOrDefault();
    if (solutionClass == null)
    {
        logger.LogWarning("No solution class found - file generation only");
        return null;
    }

    if (Activator.CreateInstance(solutionClass) is not ISolution solution)
    {
        logger.LogError("Solution class does not implement ISolution");
        return null;
    }

    var inputFile = Path.Combine(directoryInfo.FullName, "input.txt");

    var solution1 = await solution.SolvePartOne(inputFile);
    var solution2 = await solution.SolvePartTwo(inputFile);

    return (solution1, solution2);
}