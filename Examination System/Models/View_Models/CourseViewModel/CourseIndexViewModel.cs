namespace Examination_System.Models.View_Models.CourseViewModel
{
    public class CourseIndexViewModel
    {
        public IEnumerable<Models.course> Courses { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; } = "ID";
        public string SortDirection { get; set; } = "asc";

        public bool IsSortedAscending => SortDirection.ToLower() == "asc";
    }
}
