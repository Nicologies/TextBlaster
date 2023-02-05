using System.Reflection;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using TextBlaster.Main;
using TextBlaster.ProcessList;
using TextBlaster.ProcessPicker;
using TextBlaster.Sender;
using TextBlaster.TextList;

namespace TextBlaster;

public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
        {
            var viewName = viewType.FullName!;
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            const string view = "View";
            if (viewName.EndsWith(view))
            {
                viewName = viewName[..^view.Length];
            }
            var viewModelName = FormattableString.Invariant($"{viewName}ViewModel, {viewAssemblyName}");
            return Type.GetType(viewModelName);
        });
        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance(Config.FromFile());
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<ProcessPickModule>();
        moduleCatalog.AddModule<ProcessListModule>();
        moduleCatalog.AddModule<TextListModule>();
        moduleCatalog.AddModule<SenderModule>();
    }
}
