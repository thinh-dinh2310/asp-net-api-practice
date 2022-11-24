using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PaginationResult<T>
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int totalInPage { get; set; }
        public int totalItems { get; set; }
        public IEnumerable<T> data { get; set; }

        public PaginationResult()
        {

        }
    }
}
