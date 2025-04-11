using AutoMapper;
using DataBalk.Assessment.Services.Models;
using Task = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Bootstrap
{
    public class TaskMapperProfile : Profile
    {
        public TaskMapperProfile() 
        {
            CreateMap<TaskDto, Task>();
            CreateMap<Task, TaskViewModel>()
            .ForMember(dest => dest.Assignee, opt => opt.MapFrom(src => src.Assignee.UserName));
        }
    }
}
