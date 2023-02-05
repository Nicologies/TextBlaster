using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using TextBlaster.HistoryList;
using TextBlaster.Mvvm;

namespace TextBlaster.ProcessList;

[AddINotifyPropertyChangedInterface]
public class ProcessListViewModel : RegionViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    public Config Config { get; set; }
    public ProcessListViewModel(IRegionManager regionManager, Config config, IEventAggregator eventAggregator)
        : base(regionManager)
    {
        _eventAggregator = eventAggregator;
        Config = config;
        HistoryListViewModel = new HistoryListViewModel(config.ProcessList, OnItemDeleted, OnItemSelected);
    }

    private void OnItemSelected(string? obj)
    {
        _eventAggregator.GetEvent<ProcessSelectedEvent>().Publish(new ProcessSelectedEventArgs { ProcessName = obj });
        RegionManager.Regions[RegionNames.ContentRegion].NavigationService.Journal.GoBack();
    }

    private void OnItemDeleted(string obj)
    {
        Config.ProcessList.Remove(obj);
    }

    public HistoryListViewModel HistoryListViewModel { get; set; }
}
