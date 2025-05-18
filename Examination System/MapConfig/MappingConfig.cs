using AutoMapper;
using Examination_System.Model.View_Models.DepartmentViews_Models;
using Examination_System.Models;
using Examination_System.Models.View_Models.ExamView_Models;
using Examination_System.Models.View_Models.Questions_AnswersViewModel;

namespace Examination_System.MapConfig
{
    public class MappingConfig:Profile
    {

        public MappingConfig() 
        {
            CreateMap<CreateDepartmentVModel, Department>().AfterMap((src, dest) =>
            {
                
                dest.name = src.name;
                
            })

            .ForMember(dest => dest.course_depts, opt => opt.Ignore())
            .ReverseMap();  

            CreateMap<Department,DisplayDepartment>().AfterMap((src, dest) =>
            {
               
                dest.dept_id= src.dept_id;
                dest.name= src.name;
              
            })
            .ReverseMap();
            CreateMap<Student_course, StudentCourseViewModel>()
        .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.crs.crsname));



            // Exam → ExamViewModel
            CreateMap<Exam, ExamViewModel>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Exid))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.startat))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.endat))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.duration))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.QIDs)); // ✅ Add this

            // Question_Bank → QuestionViewModel
            CreateMap<Question_Bank, QuestionViewModel>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QID))
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.title))
                .ForMember(dest => dest.Marks, opt => opt.MapFrom(src => src.mark))
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.type))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.answers)); // ✅ Add this

            // answer → AnswerViewModel
            CreateMap<answer, AnswerViewModel>()
                .ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.ansid))
                .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.title));


            // Exam Result Mapping
            CreateMap<Student_Exam, ExamResultViewModel>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam.name))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Exam.crs.crsname))
                .ForMember(dest => dest.TotalScore, opt => opt.MapFrom(src =>
                    int.Parse(src.Grade)))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src =>
                    src.Exam.QIDs.Sum(q => q.mark)))
                .ForMember(dest => dest.SubmissionTime, opt => opt.MapFrom(src => DateTime.Now)).ReverseMap();
        }
    }
}
