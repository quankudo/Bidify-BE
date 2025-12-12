using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Product;
using bidify_be.Exceptions;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class ProductRepositoryImpl : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }


        //For Update
        public async Task<Product?> GetProductWithDetailsAsync(Guid id)
        {
            return await _context.Products
                .Include(x => x.Images)
                .Include(x => x.Attributes)
                .Include(x => x.ProductTags)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<PagedResult<ProductShortResponse>> FilterProductsAsync(ProductFilterRequest request)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            // ===================== USER FILTER =======================
            if (!request.IsAdmin && !string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(x => x.UserId == request.UserId);
            }

            // ===================== SEARCH ============================
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.Brand!.ToLower().Contains(s) ||
                    x.Id.ToString().ToLower().Contains(s)
                );
            }

            // ===================== FILTER ============================
            if (request.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == request.CategoryId);

            if (request.Status.HasValue)
                query = query.Where(x => x.Status == request.Status);

            if (request.Condition.HasValue)
                query = query.Where(x => x.Condition == request.Condition);

            // ===================== TOTAL COUNT ========================
            var totalItems = await query.CountAsync();

            // ===================== PAGING + PROJECTION ===============
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductShortResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CategoryId = x.CategoryId,
                    Brand = x.Brand,
                    Status = x.Status,
                    Condition = x.Condition,
                    Thumbnail = x.Thumbnail,
                    Note = x.Note
                })
                .ToListAsync();

            return new PagedResult<ProductShortResponse>(
                items,
                totalItems,
                request.PageNumber,
                request.PageSize
            );
        }

        public async Task<ProductResponse?> GetProductDetailAsync(Guid id)
        {
            var productWithDetails = await (from p in _context.Products
                                            join c in _context.Categories
                                                on p.CategoryId equals c.Id
                                            where p.Id == id
                                            select new ProductResponse
                                            {
                                                Id = p.Id,
                                                Name = p.Name,
                                                Description = p.Description,
                                                CategoryId = p.CategoryId,
                                                CategoryName = c.Title,
                                                Brand = p.Brand,
                                                Status = p.Status,
                                                Condition = p.Condition,
                                                Thumbnail = p.Thumbnail,
                                                Note = p.Note,
                                                Images = p.Images.Select(img => new ProductImageResponse
                                                {
                                                    Id = img.Id,
                                                    ImageUrl = img.ImageUrl,
                                                    PublicId = img.PublicId
                                                }).ToList(),
                                                Attributes = p.Attributes.Select(a => new ProductAttributeResponse
                                                {
                                                    Id = a.Id,
                                                    Key = a.Key,
                                                    Value = a.Value
                                                }).ToList(),
                                                ProductTags = p.ProductTags.Select(pt => new ProductTagResponse
                                                {
                                                    TagId = pt.TagId,
                                                    TagName = pt.Tag.Title
                                                }).ToList()
                                            }).FirstOrDefaultAsync();

            return productWithDetails;
        }

        public async Task<bool> existsProductByUser(string userId, Guid id)
        {
            return await _context.Products.AsNoTracking().AnyAsync(x=> x.UserId==userId && x.Id == id);
        }

        public async Task<Product?> GetProductByUser(string userId, Guid id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
        }

        public async Task<Product?> GetProductByAdmin(Guid id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        }

        public void DeleteAsyncByUser(Product product)
        {
            product.Status = ProductStatus.Hidden;
            _context.Products.Update(product);
        }

        public void DeleteAsyncByAdmin(Product product)
        {
            product.Status = ProductStatus.Cancelled;
            _context.Products.Update(product);
        }
    }
}
