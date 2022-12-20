using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Models.Note;

namespace ElevenNote.Services.Note
{
    public interface INoteService
    {
        Task<bool> CreateNoteAsync(NoteCreate request); //bool value to represent whether or not the note was created.
        Task<IEnumerable<NoteListItem>> GetAllNotesAsync();
    }
}