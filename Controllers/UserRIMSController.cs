using backend_onboarding.Models.DTOs;
using backend_onboarding.Services.Onboarding;
using backend_onboarding.Services.Rims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_onboarding.Controllers
{
    [Route("rims/user")]
    [ApiController]
    public class UserRIMSController : ControllerBase
    {
        private readonly IRimsService _rimsService;

        public UserRIMSController(IRimsService rimsService)
        {
            _rimsService = rimsService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserInRIMSRequest request)
        {
            if (string.IsNullOrEmpty(request.Login))
                return BadRequest("Требуется войти в систему");

            var user = await _rimsService.CreateUserAsync(request);
            return Ok(new
            {
                user.Id,
                user.Uid,
                user.Caption,
                user.Company,
                user.Department,
                user.JobTitle
            });
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateRIMSUserRequest request)
        {
            var success = await _rimsService.UpdateUserAsync(userId, request);

            if (!success)
            {
                return NotFound(new { message = $"Пользователь RIMS с ID {userId} не найден" });
            }

            return Ok(new { message = "Пользователь RIMS успешно обновлен" });
        }


        [HttpDelete("{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var success = await _rimsService.DeleteUserAsync(userId);

            if (!success)
            {
                return NotFound(new { message = $"Пользователь RIMS с ID {userId} не найден" });
            }

            return Ok(new { message = "Пользователь RIMS и связанные данные успешно удалены" });
        }
    }
}
