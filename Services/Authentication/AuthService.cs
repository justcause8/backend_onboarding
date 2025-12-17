using backend_onboarding.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using RimsUser = backend_onboarding.Models.Entitie.DbOnboardingRIMS.User;
using OnboardingUser = backend_onboarding.Models.Entitie.DbOnboarding.User;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;

namespace backend_onboarding.Services.Authentication
{
    public partial class AuthService : IAuthService
    {
        private readonly OnboardingRimsContext _rimsContext;
        private readonly OnboardingContext _onboardingContext;

        public AuthService(OnboardingRimsContext rimsContext, OnboardingContext onboardingContext)
        {
            _rimsContext = rimsContext;
            _onboardingContext = onboardingContext;
        }

        public async Task<UserAuthRequest?> AuthenticateAsync()
        {
            // 1. Получение логина через Windows Auth
            string login = GetLogin();

            // Вызываем логику аутентификации через новый метод
            return await AuthenticateByLoginAsync(login);
        }

        public async Task<UserAuthRequest?> AuthenticateByLoginAsync(string login)
        {
            // Шаг 2: Проверка в RIMS
            var adAccount = await _rimsContext.Adaccounts
                // Подгружаем RimsUser, чтобы получить его данные
                .Include(a => a.FkUser)
                .FirstOrDefaultAsync(a => a.AccountName == login);

            if (adAccount == null) return null;

            // Получаем RimsUser
            var rimsUser = adAccount.FkUser;

            // Сопоставление роли
            var onboardingRole = RoleMapper.MapRimsRole(adAccount.Role, rimsUser.Department);

            // 3. Проверка в Onboarding (локальная копия)
            var onboardingUser = await _onboardingContext.Users
                .FirstOrDefaultAsync(u => u.Uid == rimsUser.Uid);

            if (onboardingUser == null)
            {
                // Получаем email из RIMS
                var email = await _rimsContext.Emails
                    .Where(e => e.FkUserId == rimsUser.Id)
                    .Select(e => e.Email1)
                    .FirstOrDefaultAsync();

                // Создаем новую запись в локальной БД
                onboardingUser = new OnboardingUser
                {
                    Uid = rimsUser.Uid,
                    Name = rimsUser.Caption,
                    Login = adAccount.AccountName,
                    Role = onboardingRole,
                    Department = rimsUser.Department,
                    JobTitle = rimsUser.JobTitle,
                    Email = email
                };

                _onboardingContext.Users.Add(onboardingUser);
                await _onboardingContext.SaveChangesAsync();
            }
            // Обновление данных, если они изменились в RIMS
            else
            {
                bool changed = false;

                if (onboardingUser.Role != onboardingRole)
                {
                    onboardingUser.Role = onboardingRole;
                    changed = true;
                }

                if (onboardingUser.Department != rimsUser.Department)
                {
                    onboardingUser.Department = rimsUser.Department;
                    changed = true;
                }

                if (onboardingUser.JobTitle != rimsUser.JobTitle)
                {
                    onboardingUser.JobTitle = rimsUser.JobTitle;
                    changed = true;
                }

                if (changed)
                {
                    _onboardingContext.Users.Update(onboardingUser);
                    await _onboardingContext.SaveChangesAsync();
                }
            }

            // 4. Возвращаем DTO
            return new UserAuthRequest
            {
                Id = onboardingUser.Id,
                Name = onboardingUser.Name,
                Login = onboardingUser.Login,
                Role = onboardingUser.Role
            };
        }

        private string GetLogin()
        {
            // Восстанавливаем оригинальную логику Windows Auth
            string login = Environment.UserName;
            // Если есть DOMAIN\login, берём короткий логин
            if (login.Contains("\\")) login = login.Split('\\')[1];
            return login;
        }
    }
}