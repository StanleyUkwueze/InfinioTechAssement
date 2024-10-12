using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.ProductRepository;

namespace IfinionBackendAssessment.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            ProductRepo = new ProductRepo(context);
            CategoryRepo = new CategoryRepo(context);
            _context = context;
        }
        public IProductRepo ProductRepo { get; private set; }
        public ICategoryRepo CategoryRepo { get; private set; }
    }
}
