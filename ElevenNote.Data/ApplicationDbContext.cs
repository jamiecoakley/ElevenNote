using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
    }
}

//base constructor gets called first. Can take in those options.Options of type DbContext options takes in a generic type, in our case Application DbContext.
//Options trickle down to the parent constructor options
//This allows us to make options. Once we make those, they're initialized from the parent object