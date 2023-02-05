using Prism.Ioc;
using Prism.Modularity;

namespace TextBlaster.ProcessPicker;

public class ProcessPickModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ProcessPickView>();
    }
}
