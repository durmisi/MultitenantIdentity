using CSharpFunctionalExtensions;

namespace Tenants.Web.Logic.Base
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Result Handle(TCommand command);
    }


}