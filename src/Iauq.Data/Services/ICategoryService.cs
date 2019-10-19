using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAllCategories();
        Category GetCategoryById(int id);
        void SaveCategory(Category category);
        void DeleteCategory(Category category);
    }
}