﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("{id}")]
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
    }
}
