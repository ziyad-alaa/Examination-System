namespace Examination_System.Models.View_Models.InstructorViewModel
{
    public class InstructorIndexViewModel
    {
        public int InstructorId { get; set; }
        public string Name { get; set; }
        public string StCity { get; set; }
        public string DeptName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BranchName { get; set; }
        public bool UserIsActive { get; set; }
        public string JobTitle { get; set; }
        public decimal Salary { get; set; }
        public bool InstructorIsActive { get; set; }
    }
}
