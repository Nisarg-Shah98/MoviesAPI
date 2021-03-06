using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
        }

        public DbSet<Genre> Genres{ get; set; }
        public DbSet<Actor> Actors{ get; set; }
        public DbSet<MovieTheater> MovieTheaters{ get; set; }

    }
}
