using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using TextBlaster.Mvvm;
using TextBlaster.ProcessList;
using TextBlaster.Utils;

namespace TextBlaster.ProcessPicker;

[AddINotifyPropertyChangedInterface]
public class ProcessPickViewModel : RegionViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    private readonly Subject<string> _throttleFilter = new();
    public ProcessPickViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        : base(regionManager)
    {
        _eventAggregator = eventAggregator;
        _ = _throttleFilter.Throttle(TimeSpan.FromMilliseconds(500));
        _throttleFilter.Subscribe(_ => LoadProcesses());
    }

    public ObservableCollection<ProcessItem> Items { get; } = new();

    public ProcessItem? SelectedItem { get; set; }
    public DelegateCommand<ProcessItem?> UseItemCommand => new(OnUseItem);
    public DelegateCommand LoadedCommand => new(LoadProcesses);

    [OnChangedMethod(nameof(FireFilterRequest))]
    public string Filter { get; set; } = "";

    private void LoadProcesses()
    {
        var processes = ProcessesHaveGui.GetProcesses()
            .Select(x => new ProcessItem(x.ProcessName, x.Id, x.MainWindowTitle));

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            processes = processes.Where(x =>
                x.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                || x.Id.ToString(CultureInfo.InvariantCulture).Contains(Filter, StringComparison.OrdinalIgnoreCase)
                || x.Title.Contains(Filter, StringComparison.OrdinalIgnoreCase));
        }

        Items.Clear();
        Items.AddRange(processes.OrderBy(x => x.Name));
    }

    private void FireFilterRequest()
    {
        _throttleFilter.OnNext(Filter);
    }

    public void OnUseItem(ProcessItem? item)
    {
        item ??= SelectedItem;

        if (item == null)
        {
            return;
        }

        _eventAggregator.GetEvent<ProcessSelectedEvent>().Publish(new ProcessSelectedEventArgs
        {
            ProcessId = item.Id,
            ProcessName = item.Name,
        });

        var navigationService = RegionManager.Regions[RegionNames.ContentRegion].NavigationService;
        navigationService.Journal.GoBack();
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);

        var query = navigationContext.Parameters;
        query.TryGetValue<string>(QueryParams.Filter.ToString(), out var filter);
        if (filter != null)
        {
            Filter = filter;
        }
    }
}
