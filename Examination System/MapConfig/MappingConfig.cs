using AutoMapper;
using Examination_System.Model.View_Models.DepartmentViews_Models;
using Examination_System.Models;

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
        }
    }
}
