﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Hola.Api.Models
{
    public class AddTopicModel
    {
        public int FK_Course_Id { get; set; }
        public string Image { get; set; }
        public string EnglishContent { get; set; }
        public string VietNamContent { get; set; }
    }
}
