using Prism.Ioc;
using Prism.Modularity;

namespace TextBlaster.ProcessList;

public class ProcessListModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ProcessListView>();
    }
}
