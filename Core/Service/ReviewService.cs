using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using ServiceAbstracion;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Service
{
    public class ReviewService(IUnitOfWork _unitOfWork, IMapper _mapper) : IReviewService
    {
        public async Task<IEnumerable<ProductReviewDTO>> GetReviewsForProductAsync(int productId)
        {
            var repo = _unitOfWork.GetRepository<ProductReview, int>();
            var all = await repo.GetAllAsync();
            var topLevel = all.Where(r => r.ProductId == productId && r.ParentReviewId == null)
                               .OrderByDescending(r => r.CreatedAt);
            return _mapper.Map<IEnumerable<ProductReview>, IEnumerable<ProductReviewDTO>>(topLevel);
        }

        public async Task<ProductReviewDTO> CreateOrUpdateReviewAsync(
            string userId, string userDisplayName, CreateReviewDTO dto)
        {
            var repo = _unitOfWork.GetRepository<ProductReview, int>();
            var all = await repo.GetAllAsync();

            var existing = all.FirstOrDefault(r =>
                r.ProductId == dto.ProductId && r.UserId == userId && r.ParentReviewId == null);

            if (existing is not null)
            {
                existing.Comment = dto.Comment;
                existing.Rating = dto.Rating;
                existing.CreatedAt = DateTime.UtcNow;
                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<ProductReview, ProductReviewDTO>(existing);
            }

            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            await repo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductReview, ProductReviewDTO>(review);
        }

        public async Task<ProductReviewDTO> CreateReplyAsync(
            string userId, string userDisplayName, CreateReplyDTO dto)
        {
            var repo = _unitOfWork.GetRepository<ProductReview, int>();
            var parent = await repo.GetByIdAsync(dto.ParentReviewId)
                ?? throw new ReviewNotFoundException(dto.ParentReviewId);

            var reply = new ProductReview
            {
                ProductId = parent.ProductId,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Comment = dto.Comment,
                Rating = null,
                ParentReviewId = parent.Id
            };

            await repo.AddAsync(reply);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductReview, ProductReviewDTO>(reply);
        }

        public async Task<ProductReviewDTO> UpdateReviewAsync(int reviewId, string userId, UpdateReviewDTO dto)
        {
            var repo = _unitOfWork.GetRepository<ProductReview, int>();
            var review = await repo.GetByIdAsync(reviewId)
                ?? throw new ReviewNotFoundException(reviewId);

            if (review.UserId != userId)
                throw new UnauthorizedException();

            review.Comment = dto.Comment;
            if (review.ParentReviewId is null)
                review.Rating = dto.Rating;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductReview, ProductReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {
            var repo = _unitOfWork.GetRepository<ProductReview, int>();
            var review = await repo.GetByIdAsync(reviewId)
                ?? throw new ReviewNotFoundException(reviewId);

            if (review.UserId != userId)
                throw new UnauthorizedException();

            if (review.ParentReviewId is null)
            {
                var all = await repo.GetAllAsync();
                foreach (var reply in all.Where(r => r.ParentReviewId == reviewId))
                    repo.Remove(reply);
            }

            repo.Remove(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}