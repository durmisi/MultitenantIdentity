using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Domain
{
    public sealed class TenantRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public TenantRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Domain.Tenant GetById(long id)
        {
            return _unitOfWork.Get<Domain.Tenant>(id);
        }

        public void Save(Domain.Tenant tenant)
        {
            _unitOfWork.SaveOrUpdate(tenant);
        }

        public void Delete(Domain.Tenant tenant)
        {
            _unitOfWork.Delete(tenant);
        }
    }
}
