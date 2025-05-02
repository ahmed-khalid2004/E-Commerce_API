using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using ServicesAbstraction;

namespace Presentation.Controllers
{
    public class ProductsController(IProductService productService)
    {
        private readonly IProductService _productService = productService;
    }
}