using Tea.Application.DTOs.Sizes;
using Tea.Domain.Entities;

namespace Tea.Application.Mappers
{
    public class SizeMapper
    {
        public static SizeResponse EntityToResponse(Size size)
        {
            return new SizeResponse
            {
                Id = size.Id,
                Name = size.Name,
                Description = size.Description ?? "",
                Price = size.Price,
                NewPrice = size.NewPrice ?? 0,
            };
        }
    }
}
