using AutoMapper;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;

namespace bidify_be.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
            CreateMap<UserRegisterRequest, ApplicationUser>();
            CreateMap<ApplicationUser, UserRegisterResponse>();

        }
    }
}
