using System;
using CSharpFunctionalExtensions;
using TenantsWeb.Logic.Base;
using TenantsWeb.Logic.Decorators;
using TenantsWeb.Logic.Domain;
using TenantsWeb.Logic.tenants;
using TenantsWeb.Logic.Utils;

namespace TenantsWeb.Logic.Services
{
    public sealed class RegisterCommand : ICommand
    {
        public string Name { get; }

        public Guid TenantGuid { get; set; }


        public RegisterCommand(string name, Guid tenantGuid)
        {
            Name = name;
            TenantGuid = tenantGuid;
        }

        [AuditLog]
        internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public RegisterCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }

            public Result Handle(RegisterCommand command)
            {
                var unitOfWork = new UnitOfWork(_sessionFactory);
                var tenantRepository = new TenantRepository(unitOfWork);
                var tenant = new Tenant(command.Name, command.TenantGuid);

                tenantRepository.Save(tenant);
                unitOfWork.Commit();

                return Result.Ok();
            }
        }

        
    }
}