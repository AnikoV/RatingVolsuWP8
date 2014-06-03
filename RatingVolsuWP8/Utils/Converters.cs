﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
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
            else
            {
                return String.Empty;
            }
            
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
                    return "экзамен с защитой";
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
            var total = value as string;
            if (String.IsNullOrEmpty(total))
                return String.Empty;
            if (total.Contains("("))
            {
                return total.Substring(0, total.IndexOf('('));
            }
            return total;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
