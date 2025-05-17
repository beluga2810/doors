using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOOR.Data;
using DOOR.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;


namespace DOOR.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Employees.Include(e => e.Department);
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(await appDBContext.ToListAsync());
        }
        public async Task<IActionResult> GetServiceByEmployeeName(string eName)
        {
            var services = await _context.Services
                .Where(service => service.Employee.Name == eName)
                .ToListAsync();

            ViewBag.Services = services;

            var employees = await _context.Employees.ToListAsync();

            return View("Index", employees);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Position,Salary,DepartmentId")] Employee employee)
        {
            _context.Add(employee);
            await _context.SaveChangesAsync();
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Position,Salary,DepartmentId")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'AppDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //public async Task<IActionResult> GeneratePdfReport(int? departmentId)
        //{
        //    var employeesQuery = _context.Employees.Include(e => e.Department).AsQueryable();

        //    if (departmentId.HasValue)
        //    {
        //        employeesQuery = employeesQuery.Where(e => e.DepartmentId == departmentId.Value);
        //    }

        //    var employees = await employeesQuery.ToListAsync();

        //    using var stream = new MemoryStream();
        //    var pdfWriter = new PdfWriter(stream);
        //    var pdfDocument = new PdfDocument(pdfWriter);
        //    var document = new Document(pdfDocument);
        //    var font = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arial.ttf", "Identity-H");
        //    document.SetFont(font);
        //    document.Add(new Paragraph("Employee Report")
        //        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));

        //    var table = new iText.Layout.Element.Table(5, true);

        //    table.AddHeaderCell("ID");
        //    table.AddHeaderCell("Name");
        //    table.AddHeaderCell("Position");
        //    table.AddHeaderCell("Salary");
        //    table.AddHeaderCell("Department");

        //    foreach (var employee in employees)
        //    {
        //        table.AddCell(employee.Id.ToString());
        //        table.AddCell(employee.Name.ToString());
        //        table.AddCell(employee.Position.ToString());
        //        table.AddCell(employee.Salary.ToString("C"));
        //        table.AddCell(employee.Department?.Name.ToString() ?? "N/A");
        //    }

        //    document.Add(table);
        //    document.Close();

        //    return File(stream.ToArray(), "application/pdf", "EmployeeReport.pdf");
        //}
    }
    }
