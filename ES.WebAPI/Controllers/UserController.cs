using ES.Business.EventAggregate;
using ES.Entity;
using ES.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ES.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly AggregateRepository _aggregateRepository;
        readonly UserAggregate _userAggregate;
        public UsersController(AggregateRepository aggregateRepository, UserAggregate userAggregate)
        {
            _aggregateRepository = aggregateRepository;
            _userAggregate = userAggregate;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateUserModel model)
        {
            Guid userId = Guid.NewGuid();
            _userAggregate.SetStreamName($"user-{userId}");
            _userAggregate.Created(new()
            {
                Id = userId,
                Email = model.Email,
                Name = model.Name,
                UserName = model.UserName
            });

            await _aggregateRepository.SaveAsync(_userAggregate);
            return StatusCode((int)HttpStatusCode.Created);
        }
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateName(UpdateUserNameModel model)
        {
            _userAggregate.SetStreamName($"user-{model.Id}");
            _userAggregate.NameChanged(model.Name, model.Id);

            await _aggregateRepository.SaveAsync(_userAggregate);
            return StatusCode((int)HttpStatusCode.OK);

        }

        [HttpPut("[action]")]
        public async Task<IActionResult> EmailApprove(UserEmailApproveModel model)
        {
            _userAggregate.SetStreamName($"user-{model.Id}");
            _userAggregate.EmailApproved(model.Id);

            await _aggregateRepository.SaveAsync(_userAggregate);
            return StatusCode((int)HttpStatusCode.OK);

        }
        [HttpGet("[action]/{streamName}")]
        public async Task<IActionResult> GetEvents(string streamName)
        {
            dynamic events = await _aggregateRepository.GetEvents($"user-{streamName}");
            return Ok(events);
        }

        [HttpGet("[action]/{streamName}")]
        public async Task<IActionResult> GetData(string streamName)
        {
            User user = await _aggregateRepository.GetData($"user-{streamName}");
            return Ok(user);
        }
    }
}
