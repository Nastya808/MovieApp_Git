﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MovieApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public MoviesController(MovieContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Director,Genre,Year,Description")] Movie movie, IFormFile poster)
        {
            if (ModelState.IsValid)
            {
                if (poster != null)
                {
                    string path = Path.Combine(_appEnvironment.WebRootPath, "posters", poster.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await poster.CopyToAsync(stream);
                    }
                    movie.Poster = "/posters/" + poster.FileName;
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

    }
}