﻿using BizMate.Application.Common.Dto.UserAggregate;

namespace BizMate.Api.UserCases.Product.CreateProduct
{
    public class CreateProductResponseViewModel
    {
        public ProductCoreDto Product { get; set; }
        public CreateProductResponseViewModel(ProductCoreDto product)
        {
            Product = product;
        }
    }
}
