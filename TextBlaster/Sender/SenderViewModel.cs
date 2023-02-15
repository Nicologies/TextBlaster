using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using FlaUI.Core;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PropertyChanged;
using TextBlaster.Mvvm;
using TextBlaster.ProcessList;
using TextBlaster.ProcessPicker;
using TextBlaster.TextList;
using TextBlaster.Utils;

namespace TextBlaster.Sender;

[AddINotifyPropertyChangedInterface]
public class SenderViewModel : RegionViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    public SenderViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, Config config)
        : base(regionManager)
    {
        _eventAggregator = eventAggregator;
        Config = config;
        ProcessName = config.ProcessList.FirstOrDefault() ?? "";
        TextToSend = config.TextList.FirstOrDefault() ?? "";
        _eventAggregator.GetEvent<ProcessSelectedEvent>().Subscribe(x =>
        {
            ProcessName = x.ProcessName ?? "";
            ProcessId = x.ProcessId;
        });
        _eventAggregator.GetEvent<TextSelectedEvent>().Subscribe(x =>
        {
            TextToSend = x ?? "";
        });
    }

    private void SaveConfiguration()
    {
        if (!string.IsNullOrWhiteSpace(ProcessName))
        {
            Config.ProcessList.Remove(ProcessName);
            Config.ProcessList.Insert(0, ProcessName);
        }

        if (!string.IsNullOrWhiteSpace(TextToSend))
        {
            Config.TextList.Remove(TextToSend);
            Config.TextList.Insert(0, TextToSend);
        }
    }

    [OnChangedMethod(nameof(OnProcessNameChanged))]
    public string ProcessName { get; set; }
    public string TextToSend { get; set; }
    public int? ProcessId { get; set; }

    public Config Config { get; set; }

    public void OnProcessNameChanged()
    {
        ProcessId = null;
    }

    [JsonIgnore]
    public DelegateCommand SubmitCmd => new(Submit, () => ProcessName != string.Empty && TextToSend != string.Empty);

    public DelegateCommand ShowHistoryCommand => new(ShowProcessHistory);
    public DelegateCommand ShowTextHistoryCommand => new(ShowTextHistory);

    public DelegateCommand ToggleAppendCarriageReturn => new(() => Config.AppendCarriageReturn = !Config.AppendCarriageReturn);
    public DelegateCommand ShowProcessPickCommand => new(ShowProcessPick);

    private void ShowProcessPick()
    {
        RegionManager.RequestNavigate(RegionNames.ContentRegion, nameof(ProcessPickView));
    }
    private void ShowProcessHistory()
    {
        RegionManager.RequestNavigate(RegionNames.ContentRegion, nameof(ProcessListView));
    }

    private void ShowTextHistory()
    {
        RegionManager.RequestNavigate(RegionNames.ContentRegion, nameof(TextListView));
    }

    private bool _pendingSend;
    private void Submit()
    {
        if (ProcessName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            ProcessName = Path.GetFileNameWithoutExtension(ProcessName);
        }

        SaveConfiguration();

        if (ProcessId.HasValue)
        {
            try
            {
                Process.GetProcessById(ProcessId.Value);
            }
            catch
            {
                ProcessId = null;
            }
        }

        if (ProcessId == null)
        {
            var processes = ProcessesHaveGui.GetProcessesByName(ProcessName).ToList();
            if (!processes.Any())
            {
                _eventAggregator.GetEvent<GenericMessageEvent>().Publish($"Unable to find process: {ProcessName}");
                return;
            }

            if (processes.Count > 1)
            {
                var param = new NavigationParameters
                {
                    { QueryParams.Filter.ToString(), ProcessName }
                };
                _pendingSend = true;
                RegionManager.Regions[RegionNames.ContentRegion].NavigationService.RequestNavigate(nameof(ProcessPickView), param);
                return;
            }
            else
            {
                using var process = processes.FirstOrDefault();
                ProcessId = process?.Id;
            }
        }

        if (ProcessId == null)
        {
            _eventAggregator.GetEvent<GenericMessageEvent>().Publish($"Unable to find process: {ProcessName}");
            return;
        }

        try
        {
            if (ProcessId.Value == Process.GetCurrentProcess().Id)
            {
                _eventAggregator.GetEvent<GenericMessageEvent>().Publish("Send to this application not allowed");
                return;
            }

            var app = Application.Attach(ProcessId.Value);

            using var automation = new UIA3Automation();
            var wnd = app.GetMainWindow(automation);
            wnd.SetForeground();
            if (wnd.Patterns.Window.Pattern.WindowVisualState.Value ==
                FlaUI.Core.Definitions.WindowVisualState.Minimized)
            {
                wnd.Patterns.Window.Pattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Normal);
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }

            foreach (var modal in wnd.ModalWindows)
            {
                if (modal.IsEnabled)
                {
                    modal.Focus();
                }
            }

            Keyboard.Type(TextToSend);
            if (Config.AppendCarriageReturn)
            {
                Keyboard.Type(VirtualKeyShort.ENTER);
            }

            _eventAggregator.GetEvent<GenericMessageEvent>().Publish("Text Sent!!!");
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            _eventAggregator.GetEvent<GenericMessageEvent>()
                .Publish("Access denied when sending, you may need to run this application as admin");
        }
        catch (InvalidOperationException ex)
        {
            _eventAggregator.GetEvent<GenericMessageEvent>()
                .Publish(ex.ToString());
        }
    }

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);

        if (!_pendingSend)
        {
            return;
        }

        _pendingSend = false;
        if (ProcessId != null)
        {
            Submit();
        }
    }
}
