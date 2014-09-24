using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public class ArrayWrapperConverter : IValueConverter
    {
        private static readonly Type ArrayWrappingHelperType = typeof(ArrayWrappingHelper);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var array = (value as ObservableCollection<Rating>).ToArray();
            Type valueType = array.GetType();
            if (!valueType.IsArray)
            {
                return DependencyProperty.UnsetValue;
            }

            IEnumerable wrappingHelper = new ArrayWrappingHelper(array);
            return wrappingHelper;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class ArrayWrappingHelper : IEnumerable
    {
        private readonly Rating[] _array;

        public ArrayWrappingHelper(object array)
        {
            _array = (Rating[])array;
        }

        public IEnumerator GetEnumerator()
        {
            return _array.Select((item, index) => new ArrayItemWrapper(_array, index)).GetEnumerator();
        }
    }

    public class ArrayItemWrapper
    {
        private readonly Rating[] _array;
        private readonly int _index;

        public int Index
        {
            get { return _index + 1; }
        }

        public Rating Value
        {
            get { return _array[_index]; }
            set { _array[_index] = value; }
        }

        public ArrayItemWrapper(Rating[] array, int index)
        {
            _array = array;
            _index = index;
        }
    }
}
