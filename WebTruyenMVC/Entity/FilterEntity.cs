namespace WebTruyenMVC.Entity
{
    public class FilterEntity
    {
        public string? q { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public bool OrderByDescending { get; set; }
        public Filter filter { get; set; }
    }

    public class Filter
    {

    }
    public class Register
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ReadChapterRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string StoryId { get; set; } = string.Empty;
        public int ChapterNumber { get; set; }
    }

}
