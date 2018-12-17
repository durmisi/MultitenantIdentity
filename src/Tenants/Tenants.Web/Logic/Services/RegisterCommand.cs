using System;
using CSharpFunctionalExtensions;
using Tenants.Web.Logic.Base;
using Tenants.Web.Logic.Decorators;
using Tenants.Web.Logic.Domain;
using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Services
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

                var tenant0 = tenantRepository.GetByName(command.Name);
                if (tenant0 != null)
                {
                    return  Result.Fail($"Tenant with name {command.Name} already exist.");
                }
                
                var tenant = new Tenant(command.Name, command.TenantGuid);

                tenantRepository.Save(tenant);
                unitOfWork.Commit();

                return Result.Ok();
            }
        }

        
    }
}