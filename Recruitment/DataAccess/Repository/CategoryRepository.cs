using BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<IEnumerable<Category>> GetAllCategories() => CategoryDAO.Instance.GetAllCategories();
    }
}
