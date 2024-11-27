# Windows Terminal Theme Utility

This utility allows you to manage and customize the theme settings of your Windows Terminal without having to manually edit the settings.json file.

## Prerequisites

- .NET Core 8.0 SDK or later
- Windows Terminal


## Building the project

this command will build the executable at: .\out\net8.0\win-x64\publish\theme.exe

```shell
    dotnet publish
```

## Usage

1. Put the theme.exe file wherever you prefer.

2. Add the folder containing the theme.exe file to the PATH environment variable.

3. Run the following command to see the available options:
```shell
    theme help
```


## Testing the project
```shell
    dotnet run "arguments"
```




## Used packages:
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [ConsoleTables](https://github.com/khalidabuhakmeh/ConsoleTables)