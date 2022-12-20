using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ElevenNote.Models.Note;
using ElevenNote.Data;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Services.Note
{
    public class NoteService : INoteService
    {
        private readonly int _userId;
        private readonly ApplicationDbContext _dbContext;
        public NoteService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var value = userClaims.FindFirst("Id")?.Value;
            var validId = int.TryParse(value, out _userId);
            if (!validId)
                throw new Exception("Attempted to build NoteService without UserId claim.");
            
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var notes = await _dbContext.Notes
                .Where(entity => entity.OwnerId == _userId)
                .Select(entity => new NoteListItem
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    CreatedUtc = entity.CreatedUtc
                }).ToListAsync();

            return notes;
        }
    }
}