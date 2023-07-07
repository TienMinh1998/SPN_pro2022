﻿using System;

namespace Hola.Api.Requests.Product
{
    public class ProductRequest
    {
        public string Name { get; set; }          // tên sản phẩm
        public string Description { get; set; }   // mô tả sản phẩm
        public double Price { get; set; }         // giá sản phẩm
        public string ImageUrl { get; set; }      // ảnh sản phẩm
        public int Type { get; set; }              // Loại sản phẩm
    }
}