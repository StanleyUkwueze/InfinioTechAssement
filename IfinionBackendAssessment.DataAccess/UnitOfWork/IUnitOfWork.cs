using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepo ProductRepo { get; }
        ICategoryRepo CategoryRepo { get; }
    }
}
