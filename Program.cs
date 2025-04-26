using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ConsoleTables;
class Program
{
    static dynamic? jsonObj;
    static string json = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Packages",
        "Microsoft.WindowsTerminal_8wekyb3d8bbwe",
        // "Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe",
        "LocalState",
        "settings.json"
    );
    // C:\Users\tonyl\AppData\Local\Packages\Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe\LocalState\settings.json
    static void Main(string[] args)
    {
        string jsonContent = File.ReadAllText(json);
        jsonObj = JsonConvert.DeserializeObject(jsonContent)!;

        if (args == null || args.Length == 0)
        {
            invalidCommand();
            return;
        }

        switch (args[0].ToLower())
        {
            case "convert":
            case "cv":
                var input = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes_raw.json");
                var output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes.json");
                ThemesJson.convertFile(input, output);
                return;
            case "set":
            case "s":
                if (args.Length < 2) { return; }
                setTheme(args[1]);
                // if (theme == "") { return; }
                // jsonObj["profiles"]["defaults"]["colorScheme"] = theme;
                // Console.WriteLine("Theme set to " + theme);
                break;
            case "acrylic":
            case "a":
                if (jsonObj["profiles"]["defaults"]["useAcrylic"] == null)
                {
                    jsonObj["profiles"]["defaults"]["useAcrylic"] = true;
                    break;
                }

                jsonObj["profiles"]["defaults"]["useAcrylic"] = !(bool)jsonObj["profiles"]["defaults"]["useAcrylic"];
                break;
            case "opacity":
            case "o":
                if (args.Length < 2) { return; }
                jsonObj["profiles"]["defaults"]["opacity"] = int.Parse(args[1]);
                break;
            case "backgroundopacity":
            case "b":
                jsonObj["profiles"]["defaults"]["backgroundImageOpacity"] = int.Parse(args[1]) / 100.0;
                // sets opacity to 0 otherwise image won't be transparent (maybe add to config for the future)
                jsonObj["profiles"]["defaults"]["opacity"] = 0;
                break;
            case "current":
            case "c":
                printCurrent();
                return;
            case "image":
            case "i":
                if (args.Length < 2) { return; }
                var image = getBackgroundImage(args[1]);
                jsonObj["profiles"]["defaults"]["backgroundImage"] = image;
                break;
            case "rgb":
                jsonObj["profiles"]["defaults"]["colorScheme"] = "rgb";
                rgb();
                break;
            case "help":
            case "-help":
            case "-h":
            case "h":
                printUsage();
                return;
            default:
                invalidCommand();
                return;
        }
        File.WriteAllText(json, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    static void rgb()
    {
        var theme = getThemeJson("rgb");

            Console.WriteLine("Starting RGB mode...");
            var timer = new System.Timers.Timer(150); // Change hue every 100 milliseconds
            int hue = 0;

            timer.Elapsed += (sender, e) =>
            {
                // Convert hue to RGB
                var color = HueToRgb(hue);
                jsonObj!["profiles"]["defaults"]["foreground"] = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                File.WriteAllText(json, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));

                hue = (hue + 3) % 360; // Increment hue and loop back to 0 after 360
            };

            timer.Start();
            Console.WriteLine("Press any key to stop RGB mode...");
            Console.ReadKey();
            timer.Stop();
    }
    static (int R, int G, int B) HueToRgb(int hue)
    {
        double c = 1.0;
        double x = 1.0 - Math.Abs((hue / 60.0) % 2 - 1);
        double m = 0;

        double r = 0, g = 0, b = 0;

        if (hue < 60) { r = c; g = x; b = 0; }
        else if (hue < 120) { r = x; g = c; b = 0; }
        else if (hue < 180) { r = 0; g = c; b = x; }
        else if (hue < 240) { r = 0; g = x; b = c; }
        else if (hue < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }

        return ((int)((r + m) * 255), (int)((g + m) * 255), (int)((b + m) * 255));
    }
    static string getBackgroundImage(string image)
    {
        var images = new Dictionary<string, string>
    {
        { "peter", "peter.jpg" },
        { "puff", "jigglypuffpfp.png" }
    };

        var currentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imgs");
        return images.TryGetValue(image.ToLower(), out var img) ? Path.Combine(currentPath, img) : Path.Combine(currentPath, image);
    }
    static void invalidCommand()
    {
        Console.WriteLine("Invalid command. Use 'help' to see the list of available commands.");

    }
    static void printUsage()
    {
        var commands = new Dictionary<string, string>
    {
        { "set [<theme>/next/random]", "Set the theme color" },
        { "acrylic", "Toggle acrylic effect" },
        { "opacity [0-100]", "Set the opacity" },
        { "backgroundOpacity [0-100]", "Set background image opacity" },
        { "current", "Print current settings" },
        { "help", "Shows this message box !" }
    };

        Console.WriteLine("Usage:");
        Console.WriteLine("theme <command> [args]");
        Console.WriteLine(GenerateTable(commands));
        Console.WriteLine("You can use the first letter of each command to shorten it.");
        Console.WriteLine("For example, 'theme backgroundOpacity 80' can be shortened to 'theme b 80'.");



        // Print current themes
    }

    static string GenerateTable(Dictionary<string, string> commands)
    {
        var table = new ConsoleTable("Command", "Description");

        foreach (var command in commands)
        {
            table.AddRow(command.Key, command.Value);
        }

        table.Configure(x => x.EnableCount = false);

        var tableString = table.ToMinimalString();
        var tableWidth = tableString.IndexOf('\n');

        var topBottomBar = new string('-', tableWidth - 3);

        return $" {topBottomBar}\n{tableString} {topBottomBar}";
    }

    static void printCurrent()
    {
        Console.WriteLine("Current Theme: " + jsonObj!["profiles"]["defaults"]["colorScheme"]);
        Console.WriteLine("Current Acrylic: " + jsonObj!["profiles"]["defaults"]["useAcrylic"]);
        Console.WriteLine("Current Opacity: " + jsonObj!["profiles"]["defaults"]["opacity"]);
        Console.WriteLine("Current Background Opacity: " + (int)(100 * ((double)jsonObj!["profiles"]["defaults"]["backgroundImageOpacity"])));
    }

    // todo: add to json config file instead of hardcoding
    // static Dictionary<string, string> themes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    // {
    //     { "dark", "BlueBerryPie" },
    //     { "light", "One Half Light" },
    //     { "yellow", "Apple Classic" },
    //     {"lightblue","BlueDolphin"},
    //     {"purple","C64"},
    //     {"pink","CyberPunk2077"},
    //     { "peter", "Peter" }
    // };

    static void setTheme(string arg)
    {
        // Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        var themes = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes.json"));

        ThemesJson.makeJsonDict(themes);

        allThemes = ThemesJson.getThemeNames();
        var len = allThemes.Length;
        string? selected_theme = "";
        int currIndex;
        switch (arg.ToLower())
        {
            case "list":
            case "l":
                Console.WriteLine("Available themes:");
                foreach (var name in allThemes)
                {
                    Console.WriteLine(name);
                }
                return;
            case "random":
            case "r":
                var random = new Random();
                selected_theme = allThemes[random.Next(0, len)];
                break;
            case "next":
            case "n":
                currIndex = Array.IndexOf(allThemes, jsonObj!["profiles"]["defaults"]["colorScheme"].ToString()!);
                selected_theme = allThemes[(currIndex + 1) % len];
                break;
            case "prev":
            case "previous":
            case "p":
                currIndex = Array.IndexOf(allThemes, jsonObj!["profiles"]["defaults"]["colorScheme"].ToString()!);
                selected_theme = allThemes[(currIndex - 1) % len];
                break;
            default:
                selected_theme = allThemes.FirstOrDefault(t => t.Equals(arg, StringComparison.OrdinalIgnoreCase));
                if (selected_theme == null)
                {
                    Console.WriteLine($"Theme '{arg}' does not exist.");
                    return;
                }
                break;
        }
        jsonObj!["profiles"]["defaults"]["colorScheme"] = selected_theme;
        var theme = ThemesJson.getThemeJson(selected_theme);

        // Console.WriteLine(theme);
        jsonObj!["profiles"]["defaults"].Remove("foreground");

        jsonObj["schemes"] = JArray.FromObject(new List<dynamic> { theme });        // jsonObj["schemes"] = schemes;
        Console.WriteLine("Theme set to " + selected_theme);
    }
    static string[]? allThemes;
    static string[] getAllThemes()
    {
        var schemes = jsonObj!["schemes"];
        var themes = new List<string>();
        foreach (var scheme in schemes)
        {
            themes.Add(scheme["name"].ToString()!);
        }
        return themes.ToArray();
    }

    public static string getThemeJson(string theme)
    {
        var schemes = jsonObj!["schemes"];
        var themes = new List<string>();
        foreach (var scheme in schemes)
        {
            if (scheme["name"].ToString()!.ToLower() == theme.ToLower())
            {
                return scheme["name"].ToString()!;
            }
        }
        return "";
    }
}