namespace Examination_System.Models.View_Models.InstructorViewModel
{
    public class InstructorListViewModel
    {
        public IEnumerable<InstructorIndexViewModel> Instructors { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; } = "Name";
        public string SortDirection { get; set; } = "asc";

        public bool IsSortedAscending => SortDirection.ToLower() == "asc";
    }
}
