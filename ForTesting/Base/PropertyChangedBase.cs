using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTesting
{
    internal class MagicAttribute : Attribute { }

    [Magic]
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public virtual void RaisePropertyChanged(string propName)
        {
            var e = PropertyChanged;
            if (e != null)
                App.RootFrame.Dispatcher.BeginInvoke(() => e(this, new PropertyChangedEventArgs(propName)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
