using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Domain
{
    public sealed class AppServiceRepository
    {
        private readonly UnitOfWork _unitOfWork;

        public AppServiceRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Domain.AppService GetById(long id)
        {
            return _unitOfWork.Get<Domain.AppService>(id);
        }

        public void Save(Domain.AppService tenant)
        {
            _unitOfWork.SaveOrUpdate(tenant);
        }

        public void Delete(Domain.AppService tenant)
        {
            _unitOfWork.Delete(tenant);
        }
    }
}
