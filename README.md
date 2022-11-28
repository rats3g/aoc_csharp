# Advent of Code C# Template

Repository template and CLI tool for participating in [Advent of Code](https://adventofcode.com/) using [C#](https://dotnet.microsoft.com/en-us/languages/csharp).

# Getting Started

## Clone the template

Create a new repository using this template. More information on using GitHub repository templates can be found [here](https://docs.github.com/en/repositories/creating-and-managing-repositories/creating-a-repository-from-a-template). 

## Setting up the project

You will need to find your session token for the Advent of Code website in order for the tool to download puzzle inputs. The token is stored in your browser's cookies. Open up the developer tools of your browser and copy the token value:

- Firefox: "Storage" tab, Cookies, and copy the "Value" field of the `session` cookie.
- Google Chrome / Chromium: "Application" tab, Cookies, and copy the "Value" field of the `session` cookie

Provide the token to the tool using one of the provided options:

- Save the token in `session.txt`
- Set the environment variable `AOC_SESSION`
- Use the command line argument `--session`

## Using the tool

Run the tool using `dotnet run` or your preferred IDE.

By default, the tool will try to obtain the input for the current date and run your solution. If Advent of Code is not currently active or to run a prior puzzle use the command line arguments:

- `-y, --year` overrides the puzzle set year
- `-d, --day` overrides the puzzle day

Puzzle inputs as well as solution and test stubs are stored under `aoc_csharp/{year}/{day}`.

---

:gift: Good luck and happy Advent of Code! :christmas_tree:
