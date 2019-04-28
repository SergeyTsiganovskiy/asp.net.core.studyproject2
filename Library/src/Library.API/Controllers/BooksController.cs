﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository _libraryRepository)
        {
            this._libraryRepository = _libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksFotAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var booksForAuthor = AutoMapper.Mapper.Map<IEnumerable<BookDto>>(booksFotAuthorFromRepo);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookForAuthor = AutoMapper.Mapper.Map<BookDto>(bookForAuthorFromRepo);

            return Ok(bookForAuthor);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BooksForCreationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = AutoMapper.Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Creating a book fot author {authorId} failed on save");
            }

            var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor", new { authorId, id = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting book {id} for author {authorId} failed on saving.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)  
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
            {
                var bookToAdd = AutoMapper.Mapper.Map<Book>(book);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save");
                }

                var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor", new
                {
                    authorId = authorId, id = bookToReturn.Id,
                }, bookToReturn);
            }

            AutoMapper.Mapper.Map(book, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save");
            }

            return NoContent(); 
        } 
    }
}
