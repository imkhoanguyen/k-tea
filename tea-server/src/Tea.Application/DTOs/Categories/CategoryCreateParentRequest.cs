﻿namespace Tea.Application.DTOs.Categories
{
    public class CategoryCreateParentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
