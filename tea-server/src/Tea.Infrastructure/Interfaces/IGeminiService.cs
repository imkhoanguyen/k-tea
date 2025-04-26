using Tea.Application.DTOs.Geminis;

namespace Tea.Infrastructure.Interfaces
{
    public interface IGeminiService
    {
        Task<IEnumerable<DrinkRecommentResponse>> GetDrinkRecommendationAsync(string request);
    }
}