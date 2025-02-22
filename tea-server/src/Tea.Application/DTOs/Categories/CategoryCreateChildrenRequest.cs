﻿namespace Tea.Application.DTOs.Categories
{
    public class CategoryCreateChildrenRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int ParentId { get; set; }
    }
}
