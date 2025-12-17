using AutoMapper;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Address;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Category;
using bidify_be.DTOs.Gift;
using bidify_be.DTOs.GiftType;
using bidify_be.DTOs.PackageBid;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Tags;
using bidify_be.DTOs.Users;
using bidify_be.DTOs.Voucher;

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

            // Gift Type Mappings
            CreateMap<AddGiftTypeRequest, GiftType>();
            CreateMap<UpdateGiftTypeRequest, GiftType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<GiftType, GiftTypeResponse>();

            // Gift Mapping
            CreateMap<AddGiftRequest, Gift>();
            CreateMap<UpdateGiftRequest, Gift>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<Gift, GiftResponse>();

            // Voucher Mapping
            CreateMap<AddVoucherRequest, Voucher>();
            CreateMap<UpdateVoucherRequest, Voucher>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => !Equals(srcMember, destMember)));
            CreateMap<Voucher, VoucherResponse>();


            CreateMap<Product, ProductResponse>();

            // ===== Images =====
            CreateMap<ProductImage, ProductImageResponse>();

            // ===== Attributes =====
            CreateMap<ProductAttribute, ProductAttributeResponse>();

            // ===== Tags =====
            CreateMap<ProductTag, ProductTagResponse>()
                .ForMember(
                    dest => dest.TagName,
                    opt => opt.MapFrom(src => src.Tag.Title)
                );

            // Product Mapping
            CreateMap<AddProductRequest, Product>()
            .ForMember(dest => dest.ProductTags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

            CreateMap<UpdateProductRequest, Product>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Attributes, opt => opt.Ignore())
            .ForMember(dest => dest.ProductTags, opt => opt.Ignore());

            // Map ProductImageRequest → ProductImage
            CreateMap<ProductImageRequest, ProductImage>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore());

            // Map ProductAttributeRequest → ProductAttribute
            CreateMap<ProductAttributeRequest, ProductAttribute>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore());

            // Map ProductTagRequest → ProductTag
            CreateMap<ProductTagRequest, ProductTag>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Tag, opt => opt.Ignore());
        }
    }
}
