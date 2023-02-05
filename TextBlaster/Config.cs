using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;
using PropertyChanged;

namespace TextBlaster;

[AddINotifyPropertyChangedInterface]
public class Config
{
    private static readonly string _configFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EmbedCard");

    private static readonly string _configFile = Path.Combine(_configFolder, "TextSender.json");

    private readonly Subject<int> _throttleSave = new();

    // ReSharper disable UnusedMember.Global
    public List<string> ProcessListJson
    {
        get
        {
            return ProcessList.ToList();
        }
        set
        {
            ProcessList.AddRange(value);
        }
    }
    public List<string> TextListJson
    {
        get
        {
            return TextList.ToList();
        }
        set
        {
            TextList.AddRange(value);
        }
    }

    // ReSharper restore UnusedMember.Global

    [OnChangedMethod(nameof(FireSaveConfigRequest))]
    public bool AppendCarriageReturn { get; set; } = true;

    [OnChangedMethod(nameof(FireSaveConfigRequest))]
    public bool PinAsTopMost { get; set; } = true;

    [JsonIgnore]
    public ObservableCollection<string> ProcessList = new();

    [JsonIgnore]
    public ObservableCollection<string> TextList = new();

    static Config()
    {
        if (!Directory.Exists(_configFolder))
        {
            Directory.CreateDirectory(_configFolder);
        }
    }

    public Config()
    {
        _throttleSave.Throttle(TimeSpan.FromSeconds(1))
            .Subscribe(_ => SaveConfiguration());
        ProcessList.CollectionChanged += (_, _) => FireSaveConfigRequest();
        TextList.CollectionChanged += (_, _) => FireSaveConfigRequest();
    }
    public static Config FromFile()
    {
        if (!File.Exists(_configFile))
        {
            return new Config();
        }

        using var stream = File.Open(_configFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return JsonSerializer.Deserialize<Config>(stream) ?? new Config();
    }
    private void FireSaveConfigRequest()
    {
        _throttleSave.OnNext(0);
    }
    private void SaveConfiguration()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(_configFile, json);
    }
}
