using AutoMapper;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Address;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Category;
using bidify_be.DTOs.PackageBid;
using bidify_be.DTOs.Tags;
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

            // PackageBid Mappings
            CreateMap<AddPackageBidRequest, PackageBid>();
            CreateMap<UpdatePackageBidRequest, PackageBid>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<PackageBid, PackageBidResponse>();

            // Category Mappings
            CreateMap<AddCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<Category, CategoryResponse>();

            // Tag Mappings
            CreateMap<AddTagRequest, Tag>();
            CreateMap<UpdateTagRequest, Tag>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<Tag, TagResponse>();

            // Address Mappings
            CreateMap<AddAddressRequest, Address>();
            CreateMap<UpdateAddressRequest, Address>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<Address, AddressResponse>();
        }
    }
}
