using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using AutoMapper;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Static;
using AutoMapper.QueryableExtensions;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public BooksController(BookStoreDbContext context, IMapper mapper, ILogger<BooksController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
        {
            //var books = await _context.Books.Include(q => q.Author).ToListAsync();
            //var booksDto = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books);
            //return Ok(booksDto);

            var books = await _context.Books
                .Include(q => q.Author)
                .ProjectTo<BookReadOnlyDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            var booksDto = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books);
            return Ok(booksDto);

        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(q => q.Author)
                .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookDetailsDto>(book);

            return Ok(bookDto);
        }



        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {
            
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            var book = _context.Books.FindAsync(id);
;
            _context.Entry(book).State = EntityState.Modified;

            if (book == null)
            {
                return BadRequest();
            }

            _mapper.Map(bookDto, book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (! await BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error saving book {nameof(PutBook)}");
                    //return StatusCode(500, Messages.Error500Message);
                    throw;  // Rethrow the original exception
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookCreateDto>> PostBook(BookCreateDto bookDto)
        {

            try
            {
                var book = _mapper.Map<Book>(bookDto);

                //await _context.Books.AddAsync(book); //AddAsync() is unnecessary for SQL Server.
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing PUT in {nameof(PostBook)}");
                return StatusCode(500, Messages.Error500Message);


                throw;
            }

        }


        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> BookExists(int id)
        {
            //return _context.Books.Any(e => e.Id == id);
            return await _context.Books.AnyAsync(e => e.Id == id);

        }
    }
}
