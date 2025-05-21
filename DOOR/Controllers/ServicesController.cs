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
using Spire.Doc.Documents;
using Spire.Doc;

namespace DOOR.Controllers
{
    public class ServicesController : BaseController
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            var services = _context.Services
                .Include(s => s.Door)
                .Include(s => s.Employee)
                .AsQueryable();


            switch (sortOrder)
            {
                case "price_asc":
                    services = services.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    services = services.OrderByDescending(s => s.Price);
                    break;
                default:

                    break;
            }

            ViewBag.Employees = await _context.Employees.ToListAsync();
            ViewBag.Doors = await _context.Doors.ToListAsync();

            return View(await services.ToListAsync());
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


        public async Task<IActionResult> GenerateReport(string format, int? employeeId, int? doorId, string sortOrder = "")
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

           
            switch (sortOrder)
            {
                case "price_asc":
                    servicesQuery = servicesQuery.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    servicesQuery = servicesQuery.OrderByDescending(s => s.Price);
                    break;
                default:
                    
                    break;
            }

            var services = await servicesQuery.ToListAsync();

            byte[] fileBytes;

            if (format == "pdf")
            {
                fileBytes = GeneratePdfBytes(services);
                return File(fileBytes, "application/pdf", "ServicesReport.pdf");
            }
            else if (format == "word" || format == "docx")
            {
                fileBytes = GenerateWordBytes(services);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ServicesReport.docx");
            }

            return BadRequest("Неверный формат отчёта");
        }


        private byte[] GeneratePdfBytes(List<Service> services)
        {
            using var stream = new MemoryStream();
            var writer = new iText.Kernel.Pdf.PdfWriter(stream);
            var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);
            var font = iText.Kernel.Font.PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arial.ttf", "Identity-H");
            document.SetFont(font);

            document.Add(new iText.Layout.Element.Paragraph("Отчет об отгрузках")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20));

            var table = new iText.Layout.Element.Table(5, true);
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Модель");
            table.AddHeaderCell("Статус отгрузки");
            table.AddHeaderCell("Цена");
            table.AddHeaderCell("Сотрудник");

            foreach (var service in services)
            {
                table.AddCell(service.Id.ToString());
                table.AddCell(service.Door?.Name ?? "N/A");
                table.AddCell(service.Description ?? "N/A");
                table.AddCell(service.Price.ToString("C"));
                table.AddCell(service.Employee?.Name ?? "N/A");
            }

            document.Add(table);

          
            var totalCount = services.Count;
            var totalPrice = services.Sum(s => s.Price);

         
            document.Add(new iText.Layout.Element.Paragraph($"Общее количество отгрузок: {totalCount}")
                .SetFontSize(14)
                .SetBold()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                .SetMarginTop(15));

            document.Add(new iText.Layout.Element.Paragraph($"Общая стоимость: {totalPrice:N2} ₽")
                .SetFontSize(14)
                .SetBold()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

            document.Add(new iText.Layout.Element.Paragraph($"Отчет сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}")
                .SetFontSize(12)
                .SetItalic()
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

            document.Close();

            return stream.ToArray();
        }

        
        private byte[] GenerateWordBytes(List<Service> services)
        {
            try
            {
                var document = new Spire.Doc.Document();
                var section = document.AddSection();

                
                var headerParagraph = section.AddParagraph();
                headerParagraph.AppendText("Отчет об отгрузках").CharacterFormat.Bold = true;
                headerParagraph.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;

                
                var table = section.AddTable(true);
                table.ResetCells(1, 5); 

                var headerRow = table.Rows[0];
                headerRow.Cells[0].AddParagraph().AppendText("ID");
                headerRow.Cells[1].AddParagraph().AppendText("Модель");
                headerRow.Cells[2].AddParagraph().AppendText("Статус отгрузки");
                headerRow.Cells[3].AddParagraph().AppendText("Цена");
                headerRow.Cells[4].AddParagraph().AppendText("Сотрудник");

                foreach (var service in services)
                {
                    var dataRow = table.AddRow();
                    dataRow.Cells[0].AddParagraph().AppendText(service.Id.ToString());
                    dataRow.Cells[1].AddParagraph().AppendText(service.Door?.Name ?? "N/A");
                    dataRow.Cells[2].AddParagraph().AppendText(service.Description ?? "N/A");
                    dataRow.Cells[3].AddParagraph().AppendText(service.Price.ToString("C"));
                    dataRow.Cells[4].AddParagraph().AppendText(service.Employee?.Name ?? "N/A");
                }

              
                var totalCount = services.Count;
                var totalPrice = services.Sum(s => s.Price);

               
                var totalParagraph = section.AddParagraph();
                totalParagraph.AppendText($"Общее количество отгрузок: {totalCount}")
                    .CharacterFormat.Bold = true;

                var totalPriceParagraph = section.AddParagraph();
                totalPriceParagraph.AppendText($"Общая стоимость: {totalPrice:N2} ₽")
                    .CharacterFormat.Bold = true;

                var dateParagraph = section.AddParagraph();
                dateParagraph.AppendText($"Отчет сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .CharacterFormat.Italic = true;

                
                using var stream = new MemoryStream();
                document.SaveToFile(stream, Spire.Doc.FileFormat.Docx2013);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации Word-отчета: {ex.Message}");
                throw;
            }

        }

    }

}
