namespace backend_onboarding.Models.DTOs
{
    public class AddStagesToRouteRequest
    {
        public int RouteId { get; set; }
        public List<StageDto> Stages { get; set; } = new List<StageDto>();
    }

    public class StageDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Order { get; set; } // Порядковый номер этапа
        public int? CourseId { get; set; } // Если этап привязан к конкретному курсу
        public int? TestId { get; set; }   // Если этап привязан к тесту
    }

    public class UpdateStageRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
        public int? CourseId { get; set; }
        public int? TestId { get; set; }
    }
}
