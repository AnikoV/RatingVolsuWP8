using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using Windows.Foundation.Metadata;

namespace RatingVolsuWP8
{
    public class InstuteNameToFilialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new object();

            string filial = "филиал не определен";
            var institute = value as string;

            if (!String.IsNullOrEmpty(institute))
            {
                Regex pattern = new Regex(@"\((?<val>.*?)\)", RegexOptions.Compiled | RegexOptions.Singleline);
                if (pattern.IsMatch(institute))
                {
                    var match = pattern.Match(institute);
                    if (match.Success)
                    {
                        filial = match.Groups[0].Value.Replace("(","").Replace(")","");
                    }
                }
            }
            return filial;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InstituteNameCutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return new object();
            var institute = value as string;
            if (!String.IsNullOrEmpty(institute))
            {
                return institute;
                    //= institute.Substring(0, institute.IndexOf("(", System.StringComparison.Ordinal) - 1);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
