using System.Diagnostics;

namespace TextBlaster;
using Squirrel;
internal static class AutoUpdateFromGitHub
{
    public static async Task<bool> UpdateAsync()
    {
        var manager = new UpdateManager("https://github.com/Nicologies/TextBlaster/releases/latest");

        if (Debugger.IsAttached)
        {
            // app is trying to upgrade itself.
            Debugger.Break();
            return false;
        }
        return await manager.UpdateApp() != null;
    }
}
