using ClosedXML.Excel;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using PdfSharpCore.Drawing;
using RepositoriesForPMS.Interfaces;
using ModelForPMS.ModelDtos;
using iTextSharp.text.pdf;
using PdfSharpCore.Pdf;


[ApiController]
[Route("api/[controller]")]
public class FinanceController : ControllerBase
{
    private readonly IFinanceRepository _financeRepo;

    public FinanceController(IFinanceRepository financeRepo)
    {
        _financeRepo = financeRepo;
    }

    [HttpGet("Projects")]
    public async Task<ActionResult<List<Project>>> GetAllProjects()
    {
        var projects = await _financeRepo.GetAllProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("ProjectDetails/{projectId}")]
    public async Task<ActionResult<Project>> GetProjectDetails(int projectId)
    {
        var project = await _financeRepo.GetProjectWithDetailsAsync(projectId);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpGet("GenerateInvoice/{projectId}")]
    public async Task<ActionResult<List<Invoice>>> GenerateInvoice(int projectId)
    {
        var invoices = await _financeRepo.GenerateInvoicesForProjectAsync(projectId);
        if (invoices == null || !invoices.Any()) return NotFound();
        return Ok(invoices);
    }

    [HttpGet("ExportExcel/{projectId}")]
    public async Task<IActionResult> ExportExcel(int projectId)
    {
        var invoices = await _financeRepo.GenerateInvoicesForProjectAsync(projectId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoices");

        worksheet.Cell(1, 1).Value = "Invoice ID";
        worksheet.Cell(1, 2).Value = "Date";
        worksheet.Cell(1, 3).Value = "Employee Name";
        worksheet.Cell(1, 4).Value = "Project Name";
        worksheet.Cell(1, 5).Value = "Client Name";
        worksheet.Cell(1, 6).Value = "Worked Days";
        worksheet.Cell(1, 7).Value = "Rate/Day";
        worksheet.Cell(1, 8).Value = "Budget";

        for (int i = 0; i < invoices.Count; i++)
        {
            var row = i + 2;
            var inv = invoices[i];
            worksheet.Cell(row, 1).Value = inv.InvoiceId;
            worksheet.Cell(row, 2).Value = inv.Date.ToString("dd-MM-yyyy HH:mm");
            worksheet.Cell(row, 3).Value = inv.EmployeeName;
            worksheet.Cell(row, 4).Value = inv.ProjectName;
            worksheet.Cell(row, 5).Value = inv.ClientName;
            worksheet.Cell(row, 6).Value = inv.WorkedDays;
            worksheet.Cell(row, 7).Value = inv.RatePerDay;
            worksheet.Cell(row, 8).Value = inv.Budget;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Invoice_{projectId}.xlsx");
    }

    [HttpGet("ExportPdf/{projectId}")]
    public async Task<IActionResult> ExportPdf(int projectId)
    {
        var invoices = await _financeRepo.GenerateInvoicesForProjectAsync(projectId);

        using var stream = new MemoryStream();
        var document = new PdfSharpCore.Pdf.PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 10, XFontStyle.Regular);

        int y = 40;
        gfx.DrawString("Invoice Report", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
        y += 30;

        foreach (var inv in invoices)
        {
            gfx.DrawString($"Employee: {inv.EmployeeName}", font, XBrushes.Black, new XPoint(40, y)); y += 15;
            gfx.DrawString($"Project: {inv.ProjectName}", font, XBrushes.Black, new XPoint(40, y)); y += 15;
            gfx.DrawString($"Client: {inv.ClientName}", font, XBrushes.Black, new XPoint(40, y)); y += 15;
            gfx.DrawString($"Worked Days: {inv.WorkedDays}, Rate/Day: ₹{inv.RatePerDay}, Budget: ₹{inv.Budget}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
        }

        document.Save(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/pdf", $"Invoice_{projectId}.pdf");
    }
}