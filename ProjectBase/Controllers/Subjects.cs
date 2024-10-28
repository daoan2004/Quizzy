using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Controllers
{
    public class Subjects : Controller
    {
        private readonly DataContext _context;

        public Subjects(DataContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .ToListAsync();

            // Get the first three subjects
            var featuredSubjects = subjects.Take(3).ToList();

            var categories = await _context.Category.ToListAsync();

            // Pass subjects and categories to the view
            ViewData["Categories"] = categories;
            ViewData["FeaturedSubjects"] = featuredSubjects;
            ViewData["SubjectAll"] = subjects;

            return View(subjects);
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Detail_id = id;

            var subjects_base = await _context.Subjects
              .Include(s => s.Subject_Category)
              .ThenInclude(sc => sc.Category)
              .Include(s => s.Price_package)
              .ToListAsync();

            // Fitered List

            var subjects = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .Where(s => s.ID.Equals(Detail_id) == true)
                .ToListAsync();

            // Get the first three subjects of the un-fitered list
            var featuredSubjects = subjects_base.Take(3).ToList();

            var categories = await _context.Category.ToListAsync();

            // Pass search results and categories to the view
            ViewData["Categories"] = categories;
          
            // Pass the first three subjects to the sidebar
            ViewData["FeaturedSubjects"] = featuredSubjects;

            return View(subjects);
        }



        // POST: Subjects/ShowSearchResults

        [HttpPost]
        public async Task<IActionResult> ShowSearchResults(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                // Handle empty search phrase
                return RedirectToAction(nameof(Index));
            }

            var tagList = searchPhrase.Split(',').ToList();

            // Un-fitered List

            var subjects_base = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .ToListAsync();

            // Fitered List

            var subjects = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .Where(s => s.title.Contains(searchPhrase) || s.Subject_Category.Any(sc => tagList.Contains(sc.Category.title)))
                .ToListAsync();

            // Get the first three subjects of the un-fitered list
            var featuredSubjects = subjects_base.Take(3).ToList();

            var categories = await _context.Category.ToListAsync();

            // Pass search results and categories to the view
            ViewData["Categories"] = categories;
            ViewData["SearchPhrase"] = searchPhrase;

            // Pass the first three subjects to the sidebar
            ViewData["FeaturedSubjects"] = featuredSubjects;

            return View("Index", subjects);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByTags(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                // Handle empty search phrase
                return RedirectToAction(nameof(Index));
            }

            // Un-fitered List
            var tagList = tags.Split(',').ToList();

            var subjects_base = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .ToListAsync();

            // Fitered List

            var subjects = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .Where(s => s.Subject_Category.Any(sc => tagList.Contains(sc.Category.title)))
                .ToListAsync();

            // Get the first three subjects of the un-fitered list
            var featuredSubjects = subjects_base.Take(3).ToList();

            var categories = await _context.Category.ToListAsync();

            // Pass search results and categories to the view
            ViewData["Categories"] = categories;
            //ViewData["SearchPhrase"] = searchPhrase;

            // Pass the first three subjects to the sidebar
            ViewData["FeaturedSubjects"] = featuredSubjects;

            return View("Index", subjects);
        }

        [HttpPost]
        public async Task<IActionResult> GetSubjectData(int subjectId, long userId)
        {
            var subjectRegister = await _context.Subjects
                .Include(s => s.Subject_Category)
                .ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package)
                .Where(s => s.ID.Equals(subjectId))
                .ToListAsync();

            var userRegistration = await _context.Recipe
                .Where(r => r.SubjectID == subjectId && r.UserID == userId)
                .FirstOrDefaultAsync();

            if (subjectRegister == null)
            {
                return NotFound();
            }

            var viewModel = new SubjectViewModel
            {
                Subjects = subjectRegister,
                UserRegistration = userRegistration,
                UserID = userId
            };

            return PartialView("_SubjectPopupPartial", viewModel);
        }



    }
}
