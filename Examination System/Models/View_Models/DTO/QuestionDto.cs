namespace Examination_System.Models.View_Models.DTO
{
    public class QuestionDto
    {
       
            public string Text { get; set; }
            public int Mark { get; set; }
            public string Type { get; set; } // "MCQ", "TrueFalse", "Essay"
            public List<AnswerDto> Answers { get; set; }
        
    }
    public class AnswerDto
    {
        /// <summary>
        /// The text/content of the answer
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Indicates if this is the correct answer
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Optional ordering of answers (useful for MCQ display)
        /// </summary>
        public int? Order { get; set; }
    }
}
