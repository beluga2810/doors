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
    public class OrdersController : BaseController
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders.Include(o => o.Customer).Include(o => o.Door);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            // Фильтруем заказы по диапазону дат
            var orders = await _context.Orders
                .Where(order => order.OrderDate >= startDate && order.OrderDate <= endDate)
                .Include(o => o.Customer)
                .Include(o => o.Door)
                .ToListAsync();

            // Заполняем ViewData для Customer и Door, чтобы выпадающие списки работали
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name");

            // Передаем даты обратно, чтобы они сохранялись в форме
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

            return View("Index", orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Door)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,CustomerContactInfo,AdditionalProperties,CustomerId,DoorId")] Order order)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", order.DoorId);
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", order.DoorId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,CustomerContactInfo,AdditionalProperties,CustomerId,DoorId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", order.DoorId);
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Door)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
