using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Tea.Infrastructure.Configurations;
using AutoGen.Gemini;
using AutoGen.Core;
using Tea.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tea.Application.DTOs.Geminis;



namespace Tea.Infrastructure.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly TeaContext _context;
        private readonly GeminiAIConfig _config;

        public GeminiService(IOptions<GeminiAIConfig> config, TeaContext context)
        {
            _context = context;
            _config = config.Value;
        }

        public async Task<IEnumerable<DrinkRecommentResponse>> GetDrinkRecommendationAsync(string request)
        {
            try
            {
                var apiKey = _config.ApiKey;

                if (apiKey is null)
                {
                    throw new BadRequestException("Chưa thiết lập API Key");
                }

                var geminiAgent = new GeminiChatAgent(
                        name: "gemini",
                        model: "gemini-1.5-flash-001",
                        apiKey: apiKey,
                        systemMessage: "Bạn là một chuyên gia đồ uống. Hãy phân tích yêu cầu và gợi ý đồ uống phù hợp từ danh sách được cung cấp. Trả về dưới dạng JSON array.")
                    .RegisterMessageConnector()
                    .RegisterPrintMessage();

                var items = await _context.Items
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Description,
                        x.ImgUrl
                    })
                    .ToListAsync();

                // create prompt
                var prompt = $$"""
                    DANH SÁCH ĐỒ UỐNG (định dạng JSON):
                    {{JsonSerializer.Serialize(items)}}

                    YÊU CẦU: "{{request}}"

                    HÃY:
                    1. Phân tích yêu cầu và chọn ra tối đa 3 đồ uống phù hợp nhất
                    2. Trả về kết quả dưới dạng JSON array với cấu trúc:
                    [
                        {
                            "Id": "id sản phẩm",
                            "Name": "tên sản phẩm",
                            "Description": "mô tả ngắn",
                            "ImgUrl": "url hình ảnh",
                            "Reason": "lý do chọn món này"
                        }
                    ]
                    3. Chỉ trả về JSON, không thêm bất kỳ text nào khác
                    """;

                var reply = await geminiAgent.SendAsync(prompt);
                var content = reply.GetContent();

                // Xử lý kết quả JSON
                try
                {
                    var recommendations = JsonSerializer.Deserialize<List<DrinkRecommentResponse>>(content)
                        ?? new List<DrinkRecommentResponse>();
                    return recommendations;
                }
                catch (JsonException)
                {
                    // Fallback nếu không parse được JSON
                    return new List<DrinkRecommentResponse>
                    {
                        new DrinkRecommentResponse
                        {
                            Name = "Không thể phân tích kết quả",
                            Description = "Xin lỗi, có lỗi khi xử lý gợi ý",
                            ImgUrl = null,
                            Reason = content
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Lỗi khi lấy gợi ý đồ uống: {ex.Message}");
            }
        }
    }
}