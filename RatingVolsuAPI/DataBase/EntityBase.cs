using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatingVolsuAPI
{
    public interface IRepository
    {
        string Id{ get; set; }

        void Update(IRepository item);
    }
}
