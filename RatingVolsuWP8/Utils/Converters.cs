using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Windows.Devices.Sensors;
using Windows.Foundation.Metadata;
using RatingVolsuAPI;

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
                return institute.Substring(0, institute.IndexOf("(", System.StringComparison.Ordinal) - 1);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
    public class IncrementPlaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collectionViewSource = parameter as CollectionViewSource;
            int counter = 1;
            if (collectionViewSource.Source != null)
            {
                foreach (object item in collectionViewSource.View)
                {
                    if (item == value)
                    {
                        return counter.ToString();
                    }
                    counter++;
                }
                return string.Empty;
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SubjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeNumber = value as string;
            if (String.IsNullOrEmpty(typeNumber))
                return String.Empty;
            switch (typeNumber)
            {
                case "1":
                    return "экзамен";
                case "2":
                    return "зачет";
                case "3":
                    return "экзамен\n с защитой";
                default:
                    return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var total = value as string;
            //if (String.IsNullOrEmpty(total))
            //    return String.Empty;
            //if (total.Contains("("))
            //{
            //    return total.Substring(0, total.IndexOf('('));
            //}
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListCountToVisabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int) value;
            return (count == 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public  class  RatingTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = ((FavoritesItem)value).Type;

            if (type == RatingType.RatingOfStudent)
            {
                var uri = new Uri("/RatingVolsuWP8;component/Assets/Images/student_mini", UriKind.Relative);
                StreamResourceInfo resourceInfo = Application.GetResourceStream(uri);

                var bmp = new BitmapImage();
                bmp.SetSource(resourceInfo.Stream);
                return bmp;
            }
            else
            {
                var uri = new Uri("/RatingVolsuWP8;component/Assets/Images/group_mini", UriKind.Relative);
                StreamResourceInfo resourceInfo = Application.GetResourceStream(uri);

                var bmp = new BitmapImage();
                bmp.SetSource(resourceInfo.Stream);
                return bmp;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RatingTypeToImgSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (RatingType)value;
            if (type == RatingType.RatingOfStudent)
                return "/Assets/Images/student_mini.png";
            return "/Assets/Images/group_mini.png";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 

    public class ExamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rating = value as Rating;
            if (rating != null && rating.Exam == 0)
            {
                return rating.Subject.Type == "2" ? "-" : "0";
            }
            return rating.Exam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
