using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using PropertyChanged;

namespace TextBlaster.HistoryList;

[AddINotifyPropertyChangedInterface]
public class HistoryListViewModel : BindableBase
{
    private readonly Action<string> _onItemDeleted;
    private readonly Action<string?> _onSelectedToUseItem;

    public HistoryListViewModel(ObservableCollection<string> items, Action<string> onItemDeleted, Action<string?> onSelectedToUseItem)
    {
        _onItemDeleted = onItemDeleted;
        _onSelectedToUseItem = onSelectedToUseItem;
        Items = items;
    }

    public ObservableCollection<string> Items { get; set; }
    public string? SelectedItem { get; set; }

    public DelegateCommand<string?> UseItemCommand => new(OnUseItem);

    public void DeleteItem(string item)
    {
        Items.Remove(item);

        _onItemDeleted(SelectedItem!);
    }

    public void OnUseItem(string? item)
    {
        _onSelectedToUseItem(item ?? SelectedItem);
    }
}
