﻿using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class GenreCreationDTO
    {
        [Required(ErrorMessage = "This field {0} is required")]
        [StringLength(10)]
        [FirstLetterUpperCase]
        public string Name { get; set; }
    }
}
