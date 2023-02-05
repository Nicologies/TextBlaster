using PropertyChanged;

namespace TextBlaster.ProcessPicker;

[AddINotifyPropertyChangedInterface]
public class ProcessItem
{
    public ProcessItem() : this("", -1, "")
    {
    }
    public ProcessItem(string name, int id, string title)
    {
        Name = name;
        Id = id;
        Title = title;
    }

    public string Name { get; set; }
    public string Title { get; set; }
    public int Id { get; set; }
}
