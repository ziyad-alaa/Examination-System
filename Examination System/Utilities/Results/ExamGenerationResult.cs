namespace Examination_System.Utilities.Results
{
    public class ExamGenerationResult
    {
        public bool Succeeded { get; set; }
        public int? ExamId { get; set; }
        public int? TotalMarks { get; set; }  // Add this missing property
        public int? QuestionCount { get; set; }  // Add this missing property
        public string ErrorMessage { get; set; }

        public ExamGenerationResult(bool succeeded, int? examId, string errorMessage)
        {
            Succeeded = succeeded;
            ExamId = examId;
            ErrorMessage = errorMessage;
        }

        public ExamGenerationResult()
        {
        }

        public static ExamGenerationResult Success(int examId, int totalMarks, int questionCount)
        {
            return new ExamGenerationResult
            {
                Succeeded = true,
                ExamId = examId,
                TotalMarks = totalMarks,
                QuestionCount = questionCount
            };
        }

        public static ExamGenerationResult Failure(string errorMessage)
        {
            return new ExamGenerationResult(false, null, errorMessage);
        }
    }
}