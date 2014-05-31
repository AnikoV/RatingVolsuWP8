    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RatinVolsuAPI
{
    public static class NavigateExtensions
    {
        private static object _data;

        public static void Navigate(this NavigationService navigationService,
            Uri source, object data)
        {
            _data = data;
            navigationService.Navigate(source);
        }

        public static object GetNavigationData(this NavigationService service)
        {
            return _data;
        }
    }
}
