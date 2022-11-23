﻿namespace Hola.Api.Models
{
    public class AnnouncementCategoryFilterModel
    {
        public string SearchText { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; } //ASC or DESC
    }
}
