using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    public class ItemsComparer : IEqualityComparer<Rating>
    {
        public bool Equals(Rating x, Rating y)
        {
            return x.StudentId == y.StudentId;
        }

        public int GetHashCode(Rating obj)
        {
            return obj.StudentId.GetHashCode();
        }
    }

    public class RatingComparer : IEqualityComparer<Rating>
    {
        public bool Equals(Rating x, Rating y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Rating obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class SubjectsComparer : IEqualityComparer<Subject>
    {
        public bool Equals(Subject x, Subject y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Subject obj)
        {
            return obj.Id.GetHashCode();
        }
    }

}
