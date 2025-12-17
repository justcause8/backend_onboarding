using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<List<MaterialResponse>> GetMaterialsByCourseIdAsync(int courseId)
        {
            return await _onboardingContext.Materials
                .Where(m => m.FkCourseId == courseId)
                .Select(m => new MaterialResponse
                {
                    Id = m.Id,
                    UrlDocument = m.UrlDocument
                }).ToListAsync();
        }

        public async Task<MaterialResponse?> GetMaterialByIdAsync(int id)
        {
            return await _onboardingContext.Materials
                .Where(m => m.Id == id)
                .Select(m => new MaterialResponse
                {
                    Id = m.Id,
                    UrlDocument = m.UrlDocument
                }).FirstOrDefaultAsync();
        }

        public async Task<int> CreateMaterialAsync(CreateMaterialRequest request)
        {
            var courseExists = await _onboardingContext.Courses.AnyAsync(c => c.Id == request.CourseId);
            if (!courseExists) throw new Exception("Курс не найден");

            var material = new Material
            {
                FkCourseId = request.CourseId,
                UrlDocument = request.UrlDocument
            };

            _onboardingContext.Materials.Add(material);
            await _onboardingContext.SaveChangesAsync();
            return material.Id;
        }

        public async Task<bool> UpdateMaterialAsync(int id, UpdateMaterialRequest request)
        {
            var material = await _onboardingContext.Materials.FindAsync(id);
            if (material == null) return false;

            material.UrlDocument = request.UrlDocument;

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _onboardingContext.Materials.FindAsync(id);
            if (material == null) return false;

            _onboardingContext.Materials.Remove(material);
            await _onboardingContext.SaveChangesAsync();
            return true;
        }
    }
}
