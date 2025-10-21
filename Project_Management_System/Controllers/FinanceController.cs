using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using PdfSharpCore.Drawing;
using RepositoriesForPMS.Interfaces;

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

    //[HttpGet("GenerateInvoice/{projectId}")]
    //public async Task<ActionResult<List<Invoice>>> GenerateInvoice(int projectId)
    //{
    //    var invoices = await _financeRepo.GenerateInvoicesForProjectAsync(projectId);
    //    if (invoices == null || !invoices.Any()) return NotFound();
    //    return Ok(invoices);
    //}

    [HttpGet("ExportExcel/{projectId}")]
    public async Task<IActionResult> ExportExcel(int projectId)
    {
        var invoices = await _financeRepo.GetAssignmentSummaryAsync(projectId);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Abstract_Invoices");

        string fileName = $"{invoices.First().ProjectName} - Invoice.xlsx";
        // Sanitize the file name to ensure it's valid for all operating systems
        fileName = string.Join("_", fileName.Split(System.IO.Path.GetInvalidFileNameChars()));

        worksheet.Cell(1, 1).Value = "Employee Name";
        worksheet.Cell(1, 2).Value = "RoleName";
        worksheet.Cell(1, 3).Value = "ProjectName";
        worksheet.Cell(1, 4).Value = "Employee StartDate";
        worksheet.Cell(1, 5).Value = "Employee EndDate";
        worksheet.Cell(1, 6).Value = "Worked Days";
        worksheet.Cell(1, 7).Value = "ClientName";
        worksheet.Cell(1, 8).Value = "Project StartDate";
        worksheet.Cell(1, 9).Value = "Project EndDate";
        worksheet.Cell(1, 10).Value = "Rate Per Day Of Project";
        worksheet.Cell(1, 11).Value = "Final Budget";


        for (int i = 0; i < invoices.Count; i++)
        {
            var row = i + 2;
            var inv = invoices[i];
            worksheet.Cell(row, 1).Value = inv.EmployeeFirstName;
            worksheet.Cell(row, 2).Value = inv.RoleName;
            worksheet.Cell(row, 3).Value = inv.ProjectName;
            worksheet.Cell(row, 4).Value = inv.EmployeeStartDate.ToString();
            worksheet.Cell(row, 5).Value = inv.EmployeeEndDate.ToString();
            worksheet.Cell(row, 6).Value = inv.WorkedDays;
            worksheet.Cell(row, 8).Value = inv.ProjectStartDate.ToString(); ;
            worksheet.Cell(row, 9).Value = inv.ProjectEndDate.ToString(); ;
            worksheet.Cell(row, 10).Value = inv.RatePerDay;
            worksheet.Cell(row, 11).Value = inv.Budget;

        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

    }


    private Stream GetCompanyLogoStream()
    {
        const string logoPath = @"C:\Users\prasa\OneDrive\Desktop\Csharp\Project3\Project_Management\abstractlogo.jpg";

        // Check if the file exists before attempting to open it.
        if (!System.IO.File.Exists(logoPath))
        {
            Console.WriteLine($"ERROR: Logo file not found at: {logoPath}. Check the path and file permissions.");

            // Fallback to a memory stream placeholder (1x1 white pixel) to prevent a crash.
            var ms = new MemoryStream();
            byte[] pngData = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAACklEQVR42mP8/5/hD/gATcEE5b6s8x4AAAAASUVORK5CYII=");
            ms.Write(pngData, 0, pngData.Length);
            ms.Position = 0;
            return ms;
        }

        // This will only run if the file exists. If it still throws an error, 
        // it is a permissions issue (UnauthorizedAccessException).
        var fileStream = System.IO.File.OpenRead(logoPath);
        return fileStream;
    }


    [HttpGet("ExportPdf/{projectId}")]
    public async Task<IActionResult> ExportPdf(int projectId)
    {
        var invoices = await _financeRepo.GetAssignmentSummaryAsync(projectId);

        if (!invoices.Any())
        {
            return NotFound($"No assignment data found for project ID {projectId}.");
        }

        using var stream = new MemoryStream();
        var document = new PdfSharpCore.Pdf.PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        // --- Define Fonts, Brushes, and Layout ---
        var titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
        var headerFont = new XFont("Verdana", 9, XFontStyle.Bold);
        var dataFont = new XFont("Verdana", 9, XFontStyle.Regular);
        var headerBrush = XBrushes.White;
        var dataBrush = XBrushes.Black;
        var headerBackground = XBrushes.DarkSlateGray;

        const int margin = 20;
        int y = 40;
        const int lineHeight = 20;

        try
        {
            //Note: The logoStream returned by GetCompanyLogoStream must be disposed of,
            //     which is handled by the `using` statement in the block below.
            using (var logoStream = GetCompanyLogoStream())
            {
                XImage logo = XImage.FromStream(() => logoStream);

                // Define logo position and size (Top-Left corner)
                int logoWidth = 100;
                int logoHeight = 50;

                // Draw the logo image
                gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

                // Adjust the starting Y position for the Title to be below or aligned with the logo
                y = margin + logoHeight + 10; // Start 10 points below the logo
            }
        }
        catch (Exception ex)
        {
            // Handle error if image fails to load (e.g., file not found, bad stream format)
            // For a production app, you would log this error.
            Console.WriteLine($"Error loading logo: {ex.Message}");
            y = 40; // Fallback Y position if logo fails
        }

        // --- Title and Metadata ---
        gfx.DrawString("Assignment Invoice Report", titleFont, dataBrush, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
        y += 35;

        // Display general project details above the table
        var firstInv = invoices.First();
        gfx.DrawString($"Project: {firstInv.ProjectName} (ID: {projectId})", dataFont, dataBrush, new XPoint(margin, y));
        y += 15;
        gfx.DrawString($"Client: {firstInv.ClientName} | Project Budget: {firstInv.Budget:C0}", dataFont, dataBrush, new XPoint(margin, y));
        y += 30;


        // --- Table Column Definitions (Header, Width, Starting X Position) ---
        var columns = new List<(string Header, int Width, XStringFormat Format)>
    {
        ("Employee", 95, XStringFormats.CenterLeft),
        ("Role", 80, XStringFormats.CenterLeft),
        ("Start Date", 70, XStringFormats.CenterLeft),
        ("End Date", 70, XStringFormats.CenterLeft),
        ("Days", 50, XStringFormats.CenterRight),
        ("Rate/Day", 70, XStringFormats.CenterRight),
        ("Total Cost", 80, XStringFormats.CenterRight),
    };

        int currentX = margin;
        int headerY = y;

        // --- Draw Table Header Row ---
        foreach (var col in columns)
        {
            // Draw Header Background
            var headerRect = new XRect(currentX, headerY, col.Width, lineHeight);
            gfx.DrawRectangle(headerBackground, headerRect);

            // Draw Header Text
            gfx.DrawString(col.Header, headerFont, headerBrush, headerRect, XStringFormats.Center);

            currentX += col.Width;
        }

        y = headerY + lineHeight; // Start drawing data rows right below the header

        // --- Draw Data Rows ---
        foreach (var inv in invoices)
        {
            currentX = margin;

            // Calculate Total Cost for the Assignment
            var totalCost = inv.RatePerDay * (decimal)inv.WorkedDays;

            // Alternate row colors for readability
            if (invoices.IndexOf(inv) % 2 == 1)
            {
                var rowRect = new XRect(margin, y, columns.Sum(c => c.Width), lineHeight);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 245, 245)), rowRect);
            }

            // Draw Data Cells in Order

            // Col 1: EmployeeFirstName
            gfx.DrawString(inv.EmployeeFirstName, dataFont, dataBrush, new XRect(currentX, y, columns[0].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[0].Width;

            // Col 2: RoleName
            gfx.DrawString(inv.RoleName, dataFont, dataBrush, new XRect(currentX, y, columns[1].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[1].Width;

            // Col 3: EmployeeStartDate
            gfx.DrawString(inv.EmployeeStartDate?.ToString("dd/MM/yyyy") ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[2].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[2].Width;

            //// Col 4: EmployeeEndDate
            //// Fixed: C# date format specifiers are case-sensitive. Use "dd-MM-yyyy" (lowercase).
            gfx.DrawString(inv.EmployeeEndDate?.ToString("dd/MM/yyyy"), dataFont, dataBrush, new XRect(currentX, y, columns[3].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[3].Width;

            // Col 5: WorkedDays (Right-aligned)
            gfx.DrawString(inv.WorkedDays.ToString("F1"), dataFont, dataBrush, new XRect(currentX, y, columns[4].Width, lineHeight), XStringFormats.CenterRight);
            currentX += columns[4].Width;

            // Col 6: RatePerDay (Currency, Right-aligned)
            // Use an offset to center the currency symbol slightly better

            gfx.DrawString(inv.RatePerDay?.ToString("C0"), dataFont, dataBrush, new XRect(currentX - 5, y, columns[5].Width, lineHeight), XStringFormats.CenterRight);
            currentX += columns[5].Width;

            // Col 7: Total Cost (Currency, Bold, Right-aligned)
            gfx.DrawString(totalCost?.ToString("C0"), headerFont, dataBrush, new XRect(currentX - 5, y, columns[6].Width, lineHeight), XStringFormats.CenterRight);
            currentX += columns[6].Width;


            y += lineHeight; // Move to the next row

            // Basic safety check for page break
            if (y > page.Height - margin)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = margin;
                // You would typically redraw headers here
            }
        }


        // --- Save and Return ---
        document.Save(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/pdf", $"Invoice_Summary_Project_{projectId}.pdf");
    }


    [HttpGet("projects/{projectId}/assignments")]
    public async Task<ActionResult<IEnumerable<ProjectAssignment>>> GetProjectAssignments(int projectId)
    {
        var assignments = await _financeRepo.GetAssignmentSummaryAsync(projectId);
        return Ok(assignments);
    }
}