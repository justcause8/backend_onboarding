namespace backend_onboarding.Models.DTOs
{
    public class CourseFullResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int OrderIndex { get; set; }
        public string Status { get; set; } = null!;
        public int? StageId { get; set; }

        // Вложенные материалы
        public List<MaterialResponse> Materials { get; set; } = new List<MaterialResponse>();

        // Вложенные тесты (кратко)
        public List<TestShortResponse> Tests { get; set; } = new List<TestShortResponse>();
    }

    public class MaterialResponse
    {
        public int Id { get; set; }
        public string? UrlDocument { get; set; }
    }

    public class TestShortResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal? PassingScore { get; set; }
    }
}
