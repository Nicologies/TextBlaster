using System.Diagnostics;

namespace TextBlaster;
using Squirrel;
internal static class AutoUpdateFromGitHub
{
    public static async Task<bool> UpdateAsync()
    {
        var manager = new GithubUpdateManager("https://github.com/Nicologies/TextBlaster");

        if (Debugger.IsAttached)
        {
            // app is trying to upgrade itself.
            // Debugger.Break();
            // return false;
        }

        var updateInfo = await manager.UpdateApp();
        return updateInfo != null;
    }
}
