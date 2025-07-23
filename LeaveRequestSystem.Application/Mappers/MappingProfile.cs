using AutoMapper;
using LeaveRequestSystem.DTOs;
using LeaveRequestSystem.Models;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<User, UserResponse>();
        // CreateMap<LoginRequest, User>();
    }
}
