using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ElevenNote.Models.Note;
using ElevenNote.Data;
using Microsoft.EntityFrameworkCore;
using ElevenNote.Data.Entities;

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

        public async Task<bool> CreateNoteAsync(NoteCreate request)
        {
            var noteEntity = new NoteEntity
            {
                Title = request.Title,
                Content = request.Content,
                CreatedUtc = DateTimeOffset.Now,
                OwnerId = _userId
            };

            _dbContext.Notes.Add(noteEntity);

            var numberOfChanges = await _dbContext.SaveChangesAsync();
            return numberOfChanges == 1;
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

        public async Task<NoteDetails> GetNoteByIdAsync(int noteId)
        {
            //Find the first note that has the given Id and an OwnerId that matches the requesting userId
            var noteEntity = await _dbContext.Notes
                .FirstOrDefaultAsync(e=>
                    e.Id == noteId && e.OwnerId == _userId);
            
            //If noteEntity is null, the return null; otherwise, initialize and return a new NoteDetails.
            return noteEntity is null ? null : new NoteDetails //<<TERNARY OPERATOR - return null if the entity is not found, and is therefore null; if it is found and therefore not null, then it creates a new NoteDetails with the noteEntity's value assigned to it.
            {
                Id = noteEntity.Id,
                Title = noteEntity.Title,
                Content = noteEntity.Content,
                CreatedUtc = noteEntity.CreatedUtc,
                ModifiedUtc = noteEntity.ModifiedUtc
            };
        }

        public async Task<bool> UpdateNoteAsync(NoteUpdate request)
        {
            var noteEntity = await _dbContext.Notes.FindAsync(request.Id); //Find the note and validate it's owned by the owner
            if (noteEntity?.OwnerId != _userId) //Check it it's null at the same time we check ownerId
                return false;

            //update the entity's properties
            noteEntity.Title = request.Title;
            noteEntity.Content = request.Content;
            noteEntity.ModifiedUtc = DateTimeOffset.Now;

            var numberOfChanges = await _dbContext.SaveChangesAsync(); //Save the changes to the database and capture how many rows were updated

            return numberOfChanges == 1; //numberOfChanges is stated to be equal to 1 because only one row is updated (what you see updated in the db in Azure)
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var noteEntity = await _dbContext.Notes.FindAsync(noteId); //Find the note by the given id
            if(noteEntity?.OwnerId != _userId) //Validates that the note exists and actually belongs to the user
                return false;

            //Remove the note from the DbContext and assert that the one change was saved (deleting does entail altering a row)
            _dbContext.Notes.Remove(noteEntity); 
            return await _dbContext.SaveChangesAsync() == 1;
        }
    }
}