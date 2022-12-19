using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ElevenNote.Data.Entities
{
    public class UserEntity
    {
        [Key] //<<Primary Key. Don't need [Required] since that's built into making something the primary key
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string? FirstName { get; set; } //question marks forced these to be nullable. Once this was changed, had to push migration. **Note: use SMALL migrations to avoid huge undo's if something goes wrong.
        public string? LastName { get; set; } //user descriptive messages when doing migrations!! #failsafes!

        [Required]
        public DateTime DateCreated { get; set; }

        public List<NoteEntity> Notes { get; set; }
    }
}