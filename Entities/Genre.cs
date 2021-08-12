using MoviesAPI.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="This field {0} is required")]
        [StringLength(10)]
        [FirstLetterUpperCase]
        public string Name { get; set; }
    }
}
