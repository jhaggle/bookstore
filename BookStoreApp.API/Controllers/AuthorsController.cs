using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Net.WebRequestMethods;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using System.Reflection.Metadata;
using BookStoreApp.API.Models.Author;
using AutoMapper;
using NuGet.Packaging.Signing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Humanizer;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using BookStoreApp.API.Static;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(BookStoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger) 
        {
            _context = context;  
            this.mapper = mapper; //bör ändra till _mapper
            this.logger = logger;
        }

        // GET: api/Authors
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<AuthorReadOnlyDto>>> GetAuthors() 
        {

            try
            {
                ////Denna line säger "database get me this table and pass everything to a regular list and that is what I'm returning."
                //return await _context.Authors.ToListAsync();

                var authors = await _context.Authors.ToListAsync(); //the data from the database
                var authorDtos = mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors); //collection of author read only dtos
                return Ok(authorDtos);
            }
            catch (Exception)
            {
                logger.LogError($"Error");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")] 

        public async Task<ActionResult<AuthorReadOnlyDto>> GetAuthor(int id)
        {

            try
            {
                var author = await _context.Authors.FindAsync(id); //the data from the database

                if (author == null)
                {
                    logger.LogWarning($"Record not found: {nameof(GetAuthor)} with id {id}.");
                    return NotFound();
                }

                var authorDto = mapper.Map<AuthorReadOnlyDto>(author); //collection of author read only dtos
                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error performing GET in {nameof(GetAuthors)}" );
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //The tutor mentioned that IDs are automatically assigned when creating a record(during a POST request), 
        //but in an update(PUT) request, the API needs to know which existing author should be modified.

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
        {

            if (id != authorDto.Id)
            {
                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id); // hämtar från databasen

            if(author == null)
            {  
                return NotFound(); 
            }

            // This updates the author object in memory with the new values from authorDto.
            // It copies matching properties from authorDto (the DTO from the client) into author (the entity from the database)
            mapper.Map(authorDto, author);
            //Entity Framework doesn’t automatically track changes when you modify an object manually. So we have to tell it
            _context.Entry(author).State = EntityState.Modified;

            try
            {
                // This saves all changes made in _context to the database. This executes the actual SQL UPDATE query.
                //If we don’t call SaveChangesAsync(), the changes to author will NOT be saved in the database.
                //All modifications stay only in memory until SaveChangesAsync() commits them to the database.
                // Jfr save in word.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorCreateDto>> PostAuthor(AuthorCreateDto authorDto)
        {
            var author = mapper.Map<Author>(authorDto);

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author); 
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {

            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    logger.LogWarning($"{nameof(Author)} record not found in {nameof(DeleteAuthor)} - ID: {id})");
                    return NotFound();
                }
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                //logger.LogError(ex, Messages.Error500Message);
                //return BadRequest();

                logger.LogError(ex, $"Error performing DELETE in {nameof(DeleteAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}
