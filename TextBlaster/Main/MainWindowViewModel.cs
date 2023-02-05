using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.Threading;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using TextBlaster.Mvvm;

namespace TextBlaster.Main;

[AddINotifyPropertyChangedInterface]
public class MainWindowViewModel : RegionViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    public Config Config { get; }
    public string Title { get; set; } = "Text Blaster " + Assembly.GetExecutingAssembly()!.GetName().Version;
    public DelegateCommand<RoutedUICommand> NavigateCmd { get; set; }
    public DelegateCommand LoadedCommand => new(Loaded);
    public SnackbarMessageQueue SnackBarMsgQueue { get; } = new();
    public DelegateCommand ToggleTopMost => new(() => Config.PinAsTopMost = !Config.PinAsTopMost);
    private void Loaded()
    {
        var region = RegionManager.Regions[RegionNames.ContentRegion];
        region.NavigationService.Navigated += NavigationService_Navigated;
        CheckAppUpdateAsync().Forget();
    }

    private async Task CheckAppUpdateAsync()
    {
        var updated = await AutoUpdateFromGitHub.UpdateAsync();
        if (updated)
        {
            _eventAggregator.GetEvent<GenericMessageEvent>().Publish("New version installed, restart application to apply.");
        }
    }

    private bool CanNavigate(RoutedCommand cmd)
    {
        if (!RegionManager.Regions.ContainsRegionWithName(RegionNames.ContentRegion))
        {
            return false;
        }
        var region = RegionManager.Regions[RegionNames.ContentRegion];
        var journal = region.NavigationService.Journal;
        if (cmd == NavigationCommands.BrowseBack)
        {
            return journal.CanGoBack;
        }
        else
        {
            return journal.CanGoForward;
        }
    }

    private void Navigate(RoutedUICommand cmd)
    {
        var region = RegionManager.Regions[RegionNames.ContentRegion];
        if (cmd == NavigationCommands.BrowseBack)
        {
            region.NavigationService.Journal.GoBack();
        }
        else
        {
            region.NavigationService.Journal.GoForward();
        }
    }

    public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, Config config) : base(regionManager)
    {
        _eventAggregator = eventAggregator;
        Config = config;
        eventAggregator.GetEvent<GenericMessageEvent>().Subscribe(x => SnackBarMsgQueue.Enqueue(x));
        NavigateCmd = new DelegateCommand<RoutedUICommand>(Navigate, CanNavigate);
    }

    private void NavigationService_Navigated(object? sender, RegionNavigationEventArgs e)
    {
        // NavigateCmd.RaiseCanExecuteChanged(); doesn't work here
        // I guess because the navigation happens on kind of the child page
        // The buttons on master page/container are not updated
        NavigateCmd = new DelegateCommand<RoutedUICommand>(Navigate, CanNavigate);
    }
}
