using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using TextBlaster.HistoryList;
using TextBlaster.Mvvm;

namespace TextBlaster.TextList;

[AddINotifyPropertyChangedInterface]
public class TextListViewModel : RegionViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    public Config Config { get; set; }
    public TextListViewModel(IRegionManager regionManager, Config config, IEventAggregator eventAggregator)
        : base(regionManager)
    {
        _eventAggregator = eventAggregator;
        Config = config;
        HistoryListViewModel = new HistoryListViewModel(config.TextList, OnItemDeleted, OnItemSelected);
    }

    private void OnItemSelected(string? obj)
    {
        _eventAggregator.GetEvent<TextSelectedEvent>().Publish(obj);
        RegionManager.Regions[RegionNames.ContentRegion].NavigationService.Journal.GoBack();
    }

    private void OnItemDeleted(string obj)
    {
        Config.TextList.Remove(obj);
    }

    public HistoryListViewModel HistoryListViewModel { get; set; }
}
