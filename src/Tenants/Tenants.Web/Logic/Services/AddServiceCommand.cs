using System;
using CSharpFunctionalExtensions;
using Tenants.Web.Logic.Base;
using Tenants.Web.Logic.Decorators;
using Tenants.Web.Logic.Domain;
using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Services
{
    public class AddServiceCommand : ICommand
    {
        public long TenantId { get; set; }

        public long AppServiceId { get; }

        public string HostName { get; set; }

        public AddServiceCommand(long tenantId, long appServiceId, string hostName)
        {
            TenantId = tenantId;
            AppServiceId = appServiceId;
            HostName = hostName;
        }

        [AuditLog]
        internal sealed class AddServiceCommandHandler : ICommandHandler<AddServiceCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public AddServiceCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Result Handle(AddServiceCommand command)
            {
                var unitOfWork = new UnitOfWork(_sessionFactory);
                var tenantRepository = new TenantRepository(unitOfWork);
                var appServiceRepository = new AppServiceRepository(unitOfWork);

                var tenant = tenantRepository.GetById(command.TenantId);
                if (tenant == null)
                {
                    return Result.Fail($"Tenant with Id={command.TenantId} was not found.");
                }
                var service = appServiceRepository.GetById(command.AppServiceId);
                if (service == null)
                {
                    return Result.Fail($"Service with Id={command.AppServiceId} was not found.");
                }

                try
                {
                    tenant.AddService(service, command.HostName);
                }
                catch (InvalidOperationException iox)
                {
                    if (iox.Message == "App Service is already registered for this Tenant.")
                        return Result.Fail(iox.Message);
                    throw;
                }
                
                tenantRepository.Save(tenant);
                unitOfWork.Commit();

                return Result.Ok();
            }
        }

    }
}