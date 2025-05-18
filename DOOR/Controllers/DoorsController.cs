using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOOR.Data;
using DOOR.Models;

namespace DOOR.Controllers
{
    public class DoorsController : BaseController
    {
        private readonly AppDbContext _context;

        public DoorsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Doors.Include(d => d.Category).Include(d => d.Supplier);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Doors == null)
            {
                return NotFound();
            }

            var door = await _context.Doors
                .Include(d => d.Category)
                .Include(d => d.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (door == null)
            {
                return NotFound();
            }

            return View(door);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Material,Price,Description,CategoryId,SupplierId")] Door door)
        {
            _context.Add(door);
            await _context.SaveChangesAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId);
            return View(door);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Doors == null)
            {
                return NotFound();
            }

            var door = await _context.Doors.FindAsync(id);
            if (door == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId);
            return View(door);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Material,Price,Description,CategoryId,SupplierId")] Door door)
        {
            if (id != door.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(door);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoorExists(door.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId);
            return View(door);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Doors == null)
            {
                return NotFound();
            }

            var door = await _context.Doors
                .Include(d => d.Category) // Загрузка связанной категории
                .Include(d => d.Supplier) // Загрузка связанного поставщика
                .FirstOrDefaultAsync(m => m.Id == id);

            if (door == null)
            {
                return NotFound();
            }

            // Передаем данные в представление
            ViewData["CategoryName"] = door.Category?.Name; // Название категории
            ViewData["SupplierName"] = door.Supplier?.Name; // Название поставщика

            return View(door);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Doors == null)
            {
                return Problem("Entity set 'AppDbContext.Doors' is null.");
            }

            var door = await _context.Doors.FindAsync(id);
            if (door != null)
            {
                _context.Doors.Remove(door);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoorExists(int id)
        {
            return (_context.Doors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using DOOR.Data;
//using DOOR.Models;

//namespace DOOR.Controllers
//{
//    public class DoorsController : Controller
//    {
//        private readonly AppDbContext _context;

//        public DoorsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Doors
//        public async Task<IActionResult> Index()
//        {
//            var appDbContext = _context.Doors
//                .Include(d => d.Category) // Включаем связанные категории
//                .Include(d => d.Supplier); // Включаем связанных поставщиков

//            return View(await appDbContext.ToListAsync());
//        }

//        // GET: Doors/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null || _context.Doors == null)
//            {
//                return NotFound();
//            }

//            var door = await _context.Doors
//                .Include(d => d.Category) // Включаем категорию
//                .Include(d => d.Supplier) // Включаем поставщика
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (door == null)
//            {
//                return NotFound();
//            }

//            return View(door);
//        }

//        // GET: Doors/Create
//        public IActionResult Create()
//        {
//            // Подготовка списка категорий и поставщиков
//            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name"); // Отображаем Name для категорий
//            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name"); // Отображаем Name для поставщиков

//            return View();
//        }

//        // POST: Doors/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Name,Material,Price,Description,CategoryId,SupplierId")] Door door)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(door);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            // Перезаполнение списков при ошибке валидации
//            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId);
//            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId);

//            return View(door);
//        }

//        // GET: Doors/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null || _context.Doors == null)
//            {
//                return NotFound();
//            }

//            var door = await _context.Doors.FindAsync(id);
//            if (door == null)
//            {
//                return NotFound();
//            }

//            // Подготовка списка категорий и поставщиков
//            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId); // Отображаем Name для категорий
//            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId); // Отображаем Name для поставщиков

//            return View(door);
//        }

//        // POST: Doors/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Material,Price,Description,CategoryId,SupplierId")] Door door)
//        {
//            if (id != door.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(door);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!DoorExists(door.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }

//            // Перезаполнение списков при ошибке валидации
//            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", door.CategoryId);
//            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", door.SupplierId);

//            return View(door);
//        }

//        // GET: Doors/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null || _context.Doors == null)
//            {
//                return NotFound();
//            }

//            var door = await _context.Doors
//                .Include(d => d.Category) // Включаем категорию
//                .Include(d => d.Supplier) // Включаем поставщика
//                .FirstOrDefaultAsync(m => m.Id == id);

//            if (door == null)
//            {
//                return NotFound();
//            }

//            return View(door);
//        }

//        // POST: Doors/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            if (_context.Doors == null)
//            {
//                return Problem("Entity set 'AppDbContext.Doors' is null.");
//            }

//            var door = await _context.Doors.FindAsync(id);
//            if (door != null)
//            {
//                _context.Doors.Remove(door);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool DoorExists(int id)
//        {
//            return (_context.Doors?.Any(e => e.Id == id)).GetValueOrDefault();
//        }
//    }
//}