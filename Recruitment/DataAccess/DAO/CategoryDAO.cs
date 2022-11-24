using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;
        private static readonly object instanceLock = new object();
        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            List<Category> list = new List<Category>();
            try
            {
                var context = new eRecruitmentContext();
                list = context.Categories
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllCategories: " + ex.Message);
            }
            return list;
        }
    }
}
