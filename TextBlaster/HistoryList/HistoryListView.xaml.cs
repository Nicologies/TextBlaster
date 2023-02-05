using System.Windows.Input;

namespace TextBlaster.HistoryList;

public partial class HistoryListView
{
    public HistoryListView()
    {
        InitializeComponent();
    }

    private void CanDeleteItem(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void DeleteItem(object sender, ExecutedRoutedEventArgs e)
    {
        var vm = (HistoryListViewModel)DataContext;
        var item = e.Parameter?.ToString() ?? vm.SelectedItem;

        if (item == null)
        {
            return;
        }
        vm.DeleteItem(item);
    }
}
