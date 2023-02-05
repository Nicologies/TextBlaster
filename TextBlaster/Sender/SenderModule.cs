using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TextBlaster.Sender;

public class SenderModule : IModule
{
    private readonly IRegionManager _regionManager;

    public SenderModule(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(SenderView));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<SenderView>();
    }
}
