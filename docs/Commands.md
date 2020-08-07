# Command Line Reference

Devs Entity Framework Core has the following commands

## Initialize Command

The `initialize` or `init` command configure the project solution

```Shell
Usage: dotnet DevsEntityFrameworkCore.ConsoleUi.dll init [options]

Options:
  --help                   Show help information
  -d <directory>           Full Path to .csproj file target
  --c                      Used to create only Context Class
  --u                      Used to create only UnitOfWork Class and Interface
  --r                      Used to create only RepositoryBase Class and Interface
  --i                      Used to create only Initialize Class
  --all                    Used to create all options below included mappings and repositories
  --replace                Used to force replace file if exist

Example:

dotnet DevsEntityFrameworkCore.ConsoleUi.dll init -d "/ProjectName.Infrastructure/ProjectName.csproj" --context --replace
```

## Mapping Command

The `mapping` or `map` command creates entity configuration to database

```Shell
Usage: dotnet DevsEntityFrameworkCore.ConsoleUi.dll map [options]

Options:
  --help                   Show help information
  -d <directory>           Full Path to .csproj file target
  --replace                Used to force replace file if exist

Example:

dotnet DevsEntityFrameworkCore.ConsoleUi.dll map -d "/ProjectName.Infrastructure/ProjectName.csproj" --replace
```

## Repository Command

The `repository` or `repo` command creates the Class and Interface Repository to access database for entity

```Shell
Usage: dotnet DevsEntityFrameworkCore.ConsoleUi.dll repo [options]

Options:
  --help                   Show help information
  -d <directory>           Full Path to .csproj file target
  --replace                Used to force replace file if exist

Example:

dotnet DevsEntityFrameworkCore.ConsoleUi.dll repo -d "/ProjectName.Infrastructure/ProjectName.csproj" --replace
```