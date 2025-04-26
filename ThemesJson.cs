using Newtonsoft.Json;

public class ThemesJson
{
    // json dict
    static Dictionary<string, dynamic> jsonDict = new();
    public static void convertFile(string input, string output)
    {
#if DEBUG
        input = "C:\\Users\\antho\\Documents\\GitHub\\Windows-Terminal-theme-utility\\themes_raw.json";
        output = "C:\\Users\\antho\\Documents\\GitHub\\Windows-Terminal-theme-utility\\themes.json";
#endif
        Console.WriteLine("Converting " + input + " to " + output);
        string json = File.ReadAllText(input);

        dynamic jsonObj = JsonConvert.DeserializeObject(json)!;

        var themes = jsonObj["schemes"];

        var new_json = new Dictionary<string, dynamic>();

        foreach (var theme in themes)
        {
            var themeDict = new Dictionary<string, string>();

            foreach (var property in theme)
            {
                themeDict[property.Name] = property.Value.ToString();
            }

            new_json[theme["name"].ToString()] = themeDict;
        }

        string jsonString = JsonConvert.SerializeObject(new_json, Formatting.Indented);
        File.WriteAllText(output, jsonString);
        Console.WriteLine("Converted " + input + " to " + output);
    }


    public static void makeJsonDict(string path)
    {
#if DEBUG
        path = File.ReadAllText("C:\\Users\\antho\\Documents\\GitHub\\Windows-Terminal-theme-utility\\themes.json");
#endif
        jsonDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(path)!;
    }


    public static string[] getThemeNames()
    {
        string[] names = new string[jsonDict.Count];
        int i = 0;
        foreach (var theme in jsonDict)
        {
            names[i] = theme.Key;
            i++;
        }
        return names;
    }
    public static string getTheme(string theme)
    {
        if (jsonDict.ContainsKey(theme))
        {
            return jsonDict[theme];
        }
        else
        {
            Console.WriteLine("Theme " + theme + " not found");
            return "";
        }
    }
    
    public static dynamic getThemeJson(string themeName)
    {
        var themes = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "themes.json")));
    
        if (themes!.TryGetValue(themeName, out var theme))
        {
            return theme;
        }
    
        throw new Exception($"Theme '{themeName}' not found.");
    }
}