using Microsoft.AspNetCore.Mvc;
using Tenants.Web.Logic.Dtos;
using Tenants.Web.Logic.Services;
using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Api
{
    [Route("api/tenants")]
    public sealed class TenantController : BaseController
    {
        private readonly Messages _messages;

        public TenantController(Messages messages)
        {
            _messages = messages;
        }

        [HttpGet("")]
        public IActionResult GetList()
        {
            var list = _messages.Dispatch(new GetListQuery());
            return Ok(list);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterTenantDto dto)
        {
            var command = new RegisterCommand(dto.Name, dto.TenantGuid);
            var result = _messages.Dispatch(command);
            return FromResult(result);
        }

    }
}
