namespace Examination_System.Models.View_Models.CourseDeptViewModel
{
    public class CourseDeptIndexViewModel
    {
        public IEnumerable<course_dept> Assignments { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; } = "Course";
        public string SortDirection { get; set; } = "asc";

        public bool IsSortedAscending => SortDirection.ToLower() == "asc";
    }
}
