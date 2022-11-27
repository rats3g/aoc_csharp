using System.CommandLine;

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

return await rootCommand.InvokeAsync(args);