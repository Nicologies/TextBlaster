using Prism.Mvvm;
using Prism.Navigation;

namespace TextBlaster.Mvvm;

public abstract class ViewModelBase : BindableBase, IDestructible
{
    public virtual void Destroy()
    {
    }
}
