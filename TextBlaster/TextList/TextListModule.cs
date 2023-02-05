using Prism.Ioc;
using Prism.Modularity;

namespace TextBlaster.TextList;

public class TextListModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<TextListView>();
    }
}
