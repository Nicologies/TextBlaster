using TextBlaster.HistoryList;
using TextBlaster.Main;
using TextBlaster.ProcessList;
using TextBlaster.ProcessPicker;
using TextBlaster.Sender;
using TextBlaster.TextList;

namespace TextBlaster;

internal class DesignTimeViewModelLocator
{
    public static ProcessPickViewModel ProcessPickViewModel => null!;
    public static ProcessListViewModel ProcessListViewModel => null!;
    public static TextListViewModel TextListViewModel => null!;
    public static SenderViewModel SenderViewModel => null!;
    public static MainWindowViewModel MainWindowViewModel => null!;
    public static HistoryListViewModel HistoryListViewModel => null!;
}
