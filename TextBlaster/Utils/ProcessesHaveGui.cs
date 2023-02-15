using System.Diagnostics;

namespace TextBlaster.Utils;
public static class ProcessesHaveGui
{
    public static List<Process> GetProcesses()
    {
        var processes = Process.GetProcesses().GuiOnly().ExcludeThisProcess().ToList();
        return processes;
    }
    public static IEnumerable<Process> GuiOnly(this IEnumerable<Process> self)
    {
        return self.Where(x => x.MainWindowHandle != IntPtr.Zero);
    }
    public static IEnumerable<Process> ExcludeThisProcess(this IEnumerable<Process> self)
    {
        return self.Where(x => x.Id != Process.GetCurrentProcess().Id);
    }

    public static List<Process> GetProcessesByName(string name)
    {
        var processes = Process.GetProcessesByName(name).GuiOnly().ExcludeThisProcess().ToList();
        return processes;
    }
}
