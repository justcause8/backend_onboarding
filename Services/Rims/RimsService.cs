using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entities.DbOnboardingRIMS;
using Microsoft.EntityFrameworkCore;
using RimsUser = backend_onboarding.Models.Entities.DbOnboardingRIMS.User;

namespace backend_onboarding.Services.Rims
{
    public partial class RimsService : IRimsService
    {
        private readonly OnboardingRimsContext _rimsContext;

        public RimsService(OnboardingRimsContext rimsContext)
        {
            _rimsContext = rimsContext;
        }
        public async Task<RimsUser> CreateUserAsync(CreateUserInRIMSRequest request)
        {
            // Мы имеем доступ к _rimsContext, объявленному в первой части класса
            var user = new RimsUser
            {
                Uid = Guid.NewGuid(),
                Caption = request.Caption,
                Company = request.Company,
                Department = request.Department,
                JobTitle = request.JobTitle
            };

            _rimsContext.Users.Add(user);
            await _rimsContext.SaveChangesAsync();

            var email = new Email
            {
                FkUserId = user.Id,
                Email1 = request.Email
            };

            _rimsContext.Emails.Add(email);

            var adAccount = new Adaccount
            {
                FkUserId = user.Id,
                AccountName = request.Login,
                Role = request.Role,
                AppName = request.AppName
            };

            _rimsContext.Adaccounts.Add(adAccount);

            await _rimsContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateRIMSUserRequest request)
        {
            // 1. Ищем пользователя
            var user = await _rimsContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            // 2. Обновляем основные поля (Таблица Users)
            // В RIMS поле называется Caption, хотя в запросе может быть Name (или Caption)
            if (request.Caption != null) user.Caption = request.Caption;
            if (request.Company != null) user.Company = request.Company;
            if (request.Department != null) user.Department = request.Department;
            if (request.JobTitle != null) user.JobTitle = request.JobTitle;

            // 3. Обновляем Email (Таблица Emails)
            var emailEntity = await _rimsContext.Emails.FirstOrDefaultAsync(e => e.FkUserId == userId);
            if (emailEntity != null)
            {
                if (request.Email != null) emailEntity.Email1 = request.Email;
            }
            // Если emailEntity == null, можно добавить логику создания, но пока оставляем как есть.

            // 4. Обновляем AdAccount (Таблица Adaccounts)
            var adAccount = await _rimsContext.Adaccounts.FirstOrDefaultAsync(a => a.FkUserId == userId);
            if (adAccount != null)
            {
                // В базе поле AccountName, в запросе Login
                if (request.Login != null) adAccount.AccountName = request.Login;

                if (request.Role != null) adAccount.Role = request.Role;
                if (request.AppName != null) adAccount.AppName = request.AppName;
            }

            // 5. Сохраняем изменения
            // EF Core сам поймет, какие именно поля изменились, и сформирует UPDATE запрос только для них
            await _rimsContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            // 1. Ищем пользователя
            var user = await _rimsContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            // 2. Удаляем связанные записи вручную (если в БД не настроен каскад)
            // Удаляем Email
            var emails = _rimsContext.Emails.Where(e => e.FkUserId == userId);
            _rimsContext.Emails.RemoveRange(emails);

            // Удаляем AdAccounts
            var accounts = _rimsContext.Adaccounts.Where(a => a.FkUserId == userId);
            _rimsContext.Adaccounts.RemoveRange(accounts);

            // 3. Удаляем самого пользователя
            _rimsContext.Users.Remove(user);

            await _rimsContext.SaveChangesAsync();
            return true;
        }
    }
}
