using backend_onboarding.Models.DTOs;
namespace backend_onboarding.Services.Onboarding
{
    public interface IOnboardingService
    {
        // USER
        Task<List<UserOnboardingResponse>> GetAllUsersAsync();
        Task<UserOnboardingResponse?> GetUserOnboardingDataAsync(int userId);
        Task<bool> UpdateOnboardingUserAsync(int userId, UpdateOnboardingUserRequest request);
        Task<bool> DeleteOnboardingUserAsync(int userId);

        // ROUTE
        Task<OnboardingRouteResponse?> GetOnboardingRouteByIdAsync(int routeId);
        Task<int> CreateOnboardingRouteAsync(CreateOnboardingRouteRequest request);
        Task<bool> UpdateOnboardingRouteAsync(int routeId, CreateOnboardingRouteRequest request);
        Task<bool> DeleteOnboardingRouteAsync(int routeId);
        Task<bool> AssignUserToRouteAsync(int userId, int routeId);

        // STAGE
        Task<StageResponse?> GetStageByIdAsync(int stageId);
        Task<bool> AddStagesToRouteAsync(AddStagesToRouteRequest request);
        Task<bool> UpdateStageAsync(int stageId, StageDto request);
        Task<bool> DeleteStageAsync(int stageId);
        Task<bool> AssignUserToStageAsync(int userId, int stageId);

        // COURSE
        Task<CourseFullResponse?> GetCourseByIdAsync(int courseId);
        Task<int> CreateCourseAsync(CreateCourseRequest request);
        Task<bool> UpdateCourseAsync(int courseId, CreateCourseRequest request);
        Task<bool> DeleteCourseAsync(int courseId);

        // MATERIALS
        Task<List<MaterialResponse>> GetMaterialsByCourseIdAsync(int courseId);
        Task<MaterialResponse?> GetMaterialByIdAsync(int id);
        Task<int> CreateMaterialAsync(CreateMaterialRequest request);
        Task<bool> UpdateMaterialAsync(int id, UpdateMaterialRequest request);
        Task<bool> DeleteMaterialAsync(int id);

        // TEST
        Task<List<TestResponse>> GetTestsByCourseIdAsync(int courseId);
        Task<TestResponse?> GetTestByIdAsync(int id, int userId);
        Task<int> CreateTestAsync(CreateTestRequest request);
        Task<bool> UpdateTestAsync(int id, CreateTestRequest request);
        Task<bool> DeleteTestAsync(int id);

        //QUESTION
        Task<List<QuestionResponse>> GetQuestionsByTestIdAsync(int testId);
        Task<int> CreateQuestionAsync(CreateQuestionRequest request);
        Task<bool> UpdateQuestionAsync(int questionId, CreateQuestionRequest request);
        Task<bool> DeleteQuestionAsync(int questionId);

        //ANSWER
        Task<int> SubmitAnswerAsync(UserAnswerRequest request);
        Task<bool> UpdateAnswerAsync(int answerId, UserAnswerRequest request);
        Task<bool> DeleteAnswerAsync(int answerId);
        Task<List<AnswerResponse>> GetUserAnswersByTestAsync(int userId, int testId);

    }
}