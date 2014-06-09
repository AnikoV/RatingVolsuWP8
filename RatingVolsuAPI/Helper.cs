using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RatingVolsuAPI;
using RatingVolsuAPI.Base;

namespace RatingVolsuAPI
{
    public enum RatingType
    {
        RatingOfStudent,
        RatingOfGroup
    }

    public enum InputDataMode
    {
        UseTemplate,
        Standart,
        AddTemplate,
        EditTemplate
    }

    public static class RequestInfo
    {
        public static int CurrentFavorites;
        public static RatingType CurrentRatingType;
        public static string FacultId;
        public static string GroupId;
        public static string Semestr;
        public static string StudentId;
    }

    public static class Info
    {
        public static string DbConnectionString = @"isostore:/RatingDataBase.sdf";
        public static bool ToFavoritesPivot;
    }

    public class StudentRat
    {
        public Dictionary<string, BasePredmet> Predmet;
        public Dictionary<string, List<string>> Table;
    }

    public class GroupRat
    {
        public Dictionary<string, BasePredmet> Predmet;
        public Dictionary<string, BaseStudent> Table;
    }

    public class BaseStudent
    {
        public string Name;
        public Dictionary<string, string> Predmet;
    }

    public class BasePredmet
    {
        public string Name;
        public string Type;
    }

    public class Semester : PropertyChangedBase
    {
        public string Number { get; set; }
        public string NumberText 
        {
            get { return Number + " семестр"; }
            private set { } 
        }
        public string YearsPeriod { get; set; }
    }
}
