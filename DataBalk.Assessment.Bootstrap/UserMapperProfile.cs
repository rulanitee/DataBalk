using AutoMapper;
using DataBalk.Assessment.Data.Models;
using DataBalk.Assessment.Services.Models;

namespace DataBalk.Assessment.Bootstrap
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile() 
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserViewModel>();
        }
    }
}
