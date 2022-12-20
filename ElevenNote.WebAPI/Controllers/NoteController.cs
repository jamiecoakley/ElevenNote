using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ElevenNote.Services.Note;
using Microsoft.AspNetCore.Authorization;
using ElevenNote.Models.Note;

namespace ElevenNote.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreate request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(await _noteService.CreateNoteAsync(request))
                return Ok("Note created successfully!");
            
            return BadRequest("Note could not be created.");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }

        [HttpGet("{noteId:int}")]
        public async Task<IActionResult> GetNoteById([FromBody] int noteId)
        {
            var detail = await _noteService.GetNoteByIdAsync(noteId);
            return detail is not null //similar to our service method, we're using a ternary to determine our return type
            ? Ok(detail) //If the returned value (detail) is not null, return it with a 200 OK
            : NotFound(); //Otherwise, return a NotFound() 404 response
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNoteById([FromBody] NoteUpdate request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return await _noteService.UpdateNoteAsync(request)
                ? Ok("Note updated successfully!")
                : BadRequest("Note could not be updated.");
        }
    }
}