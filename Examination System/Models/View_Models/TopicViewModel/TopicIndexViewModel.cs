namespace Examination_System.Models.View_Models.TopicViewModel
{
    public class TopicIndexViewModel
    {
        public IEnumerable<Topic> Topics { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; } = "Title";
        public string SortDirection { get; set; } = "asc";

        public bool IsSortedAscending => SortDirection.ToLower() == "asc";
    }
}
