using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuWP8
{
    public class MainViewModel : PropertyChangedBase
    {
        public string VolsuReview { get; set; }

        public MainViewModel()
        {
            VolsuReview = "ВолГУ – университет, известный в стране и за рубежом качеством образования, высоким научным потенциалом, инновационными проектами, активной социальной позицией.";
        }
    }
}
