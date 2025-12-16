using System.Collections.Generic;

namespace backend_onboarding.Services.Authentication
{
    public static class OnboardingRoles
    {
        public const string User = "User";
        public const string Mentor = "Mentor";
        public const string HrAdmin = "HrAdmin";
        public const string SuperAdmin = "SuperAdmin";
    }

    public static class RoleMapper
    {
        private const string HrDepartment = "Отдел по работе с персоналом";
        private const string ItDepartment = "Отдел по управлению персоналом";

        public static string MapRimsRole(string rimsRole, string rimsDepartment)
        {
            if (string.IsNullOrEmpty(rimsRole))
            {
                return OnboardingRoles.User;
            }

            // Нормализация для сравнения (устраняет проблемы с пробелами и регистром)
            string normalizedRole = rimsRole.Trim();
            string normalizedDepartment = rimsDepartment?.Trim() ?? string.Empty;

            // 1. СуперАдмин (IT)
            if ((normalizedRole == "Specialist_2_category" || normalizedRole == "Specialist_3_category") &&
                string.Equals(normalizedDepartment, ItDepartment, StringComparison.OrdinalIgnoreCase))
            {
                return OnboardingRoles.SuperAdmin;
            }

            // 2. HR Админ
            if ((normalizedRole == "Specialist_2_category" || normalizedRole == "Specialist_3_category") &&
                string.Equals(normalizedDepartment, HrDepartment, StringComparison.OrdinalIgnoreCase))
            {
                return OnboardingRoles.HrAdmin;
            }

            // 3. Mentor (Наставник / Руководитель)
            if (normalizedRole == "Specialist_2_category" ||
                normalizedRole == "Specialist_3_category" ||
                normalizedRole == "Head_of_department")
            {
                return OnboardingRoles.Mentor;
            }

            // 4. User (Сотрудник, включая Specialist_1_category)
            return OnboardingRoles.User;
        }
    }
}