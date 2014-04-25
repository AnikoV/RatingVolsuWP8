using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RatingVolsuAPI
{
    public enum RatingType
    {
        RatingOfStudent,
        RatingOfGroup
    }

    public class StudentRat
    {
        public Dictionary<string, string> Predmet;
        public Dictionary<string, List<string>> Table;
        
    }
}
