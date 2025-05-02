using AutoMapper;
using Shared.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using DomainLayer.Models;

namespace Services.MappingProfiles
{
    public class PictureUrlResolver : IValueResolver<Product, ProductDTO, string>
    {
        private readonly IConfiguration _configuration;

        public PictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return string.Empty;

            var baseUrl = _configuration.GetSection("Urls")["BaseUrl"];
            return $"{baseUrl}/{source.PictureUrl}";
        }
    }
}