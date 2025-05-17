using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOOR.Data;
using DOOR.Models;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Kernel.Font;

namespace DOOR.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Services
                .Include(s => s.Door)
                .Include(s => s.Employee);

            ViewBag.Employees = await _context.Employees.ToListAsync();
            ViewBag.Doors = await _context.Doors.ToListAsync();

            return View(await appDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Door)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        public IActionResult Create()
        {
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,EmployeeId,DoorId")] Service service)
        {
            _context.Add(service);
            await _context.SaveChangesAsync();
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", service.DoorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", service.EmployeeId);
            return View(service);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", service.DoorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", service.EmployeeId);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,EmployeeId,DoorId")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["DoorId"] = new SelectList(_context.Doors, "Id", "Name", service.DoorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", service.EmployeeId);
            return View(service);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Door)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'AppDbContext.Services'  is null.");
            }
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        private bool ServiceExists(int id)
        {
            return (_context.Services?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> GeneratePdfReport(int? employeeId, int? doorId)
        {
            var servicesQuery = _context.Services
                .Include(s => s.Employee)
                .Include(s => s.Door)
                .AsQueryable();

            if (employeeId.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.EmployeeId == employeeId.Value);
            }

            if (doorId.HasValue)
            {
                servicesQuery = servicesQuery.Where(s => s.DoorId == doorId.Value);
            }

            var services = await servicesQuery.ToListAsync();

            using var stream = new MemoryStream();
            var pdfWriter = new PdfWriter(stream);
            var pdfDocument = new PdfDocument(pdfWriter);
            var document = new Document(pdfDocument);
            var font = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arial.ttf", "Identity-H");
            document.SetFont(font);

            document.Add(new Paragraph("Services Report")
                .SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));

            var table = new Table(5, true);

            table.AddHeaderCell("ID");
            table.AddHeaderCell("Door");
            table.AddHeaderCell("Service Status");
            table.AddHeaderCell("Price");
            table.AddHeaderCell("Employee");
            

            foreach (var service in services)
            {
                table.AddCell(service.Id.ToString());
                table.AddCell(service.Door?.Name ?? "N/A");
                table.AddCell(service.Description ?? "N/A");
                table.AddCell(service.Price.ToString("C"));
                table.AddCell(service.Employee?.Name ?? "N/A");
                
            }

            document.Add(table);
            document.Close();

            return File(stream.ToArray(), "application/pdf", "ServicesReport.pdf");
        }
    }
}
