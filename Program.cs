using System.Text;
using Newtonsoft.Json;
using ConsoleTables;
class Program
{
    static dynamic? jsonObj;
    static string json = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Packages",
        "Microsoft.WindowsTerminal_8wekyb3d8bbwe",
        "LocalState",
        "settings.json"
    );
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
            case "set":
            case "s":
                if(args.Length<2){return;}
                var theme = getTheme(args[1]);
                if(theme==""){return;}
                jsonObj["profiles"]["defaults"]["colorScheme"] = theme;
                Console.WriteLine("Theme set to " + theme);
                break;
            case "acrylic":
            case "a":
                jsonObj["profiles"]["defaults"]["useAcrylic"] = !(bool) jsonObj["profiles"]["defaults"]["useAcrylic"];
                break;
            case "opacity":
            case "o":
                if(args.Length<2){return;}
                    jsonObj["profiles"]["defaults"]["opacity"] = int.Parse(args[1]);
                    break;
            case "backgroundopacity":
            case "b":
                jsonObj["profiles"]["defaults"]["backgroundImageOpacity"] = int.Parse(args[1])/100.0;
                // sets opacity to 0 otherwise image won't be transparent (maybe add to config for the future)
                jsonObj["profiles"]["defaults"]["opacity"] = 0;
                break;
            case "current":
            case "c":
                printCurrent();
                return;
            case "image":
            case "i":
                if(args.Length<2){return;}
                var image = getBackgroundImage(args[1]);
                jsonObj["profiles"]["defaults"]["backgroundImage"] = image;
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


static string getBackgroundImage(string image){
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

    var topBottomBar = new string('-', tableWidth-3);

    return $" {topBottomBar}\n{tableString} {topBottomBar}";
}

    static void printCurrent()
    {
        Console.WriteLine("Current Theme: " + jsonObj!["profiles"]["defaults"]["colorScheme"]);
        Console.WriteLine("Current Acrylic: " + jsonObj!["profiles"]["defaults"]["useAcrylic"]);
        Console.WriteLine("Current Opacity: " + jsonObj!["profiles"]["defaults"]["opacity"]);
        Console.WriteLine("Current Background Opacity: " + (int)(100*((double)jsonObj!["profiles"]["defaults"]["backgroundImageOpacity"])));
    }

    // todo: add to json config file instead of hardcoding
    static Dictionary<string, string> themes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "dark", "BlueBerryPie" },
        { "light", "One Half Light" },
        { "yellow", "Apple Classic" },
        {"lightblue","BlueDolphin"},
        {"purple","C64"},
        {"pink","CyberPunk2077"},
        { "peter", "Peter" }
    };

    static string getTheme(string arg)
    {
        switch (arg.ToLower())
        {
            case "random":
            case "r":
                var keys = themes.Keys.ToList();
                var random = new Random();
                return themes[keys[random.Next(keys.Count)]];
            case "next":
                var current = jsonObj!["profiles"]["defaults"]["colorScheme"];
                keys = themes.Keys.ToList();
                var index = keys.IndexOf(current.ToString());
                if (index == -1)
                {
                    return "";
                }
                return themes[keys[(index + 1) % keys.Count]];
            default:
                return themes.TryGetValue(arg.ToLower(), out var theme) ? theme : arg;
        }
    }
}