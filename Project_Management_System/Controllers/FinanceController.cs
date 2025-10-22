using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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
        const string logoPath = @"C:\Users\adich\Downloads\Abstractlogo.jpg";

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
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        // --- Define Fonts, Brushes, and Layout ---
        var titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
        var headerFont = new XFont("Verdana", 9, XFontStyle.Bold);
        var dataFont = new XFont("Verdana", 9, XFontStyle.Regular);
        var headerBrush = XBrushes.White;
        var dataBrush = XBrushes.Black;
        var headerBackground = XBrushes.DarkSlateGray;
        var headerLinePen = new XPen(XColors.LightGray, 1); // Pen for the separator line
        var footerLinePen = new XPen(XColors.Black, 0.5); // Pen for the final footer line

        const int margin = 20;
        int y = 40;
        const int lineHeight = 20;

        // --- LOGO INSERTION (New Section) ---
        try
        {
            // Note: The logoStream returned by GetCompanyLogoStream must be disposed of,
            // which is handled by the `using` statement in the block below.
            using (var logoStream = GetCompanyLogoStream())
            {
                XImage logo = XImage.FromStream(() => logoStream);

                // Define logo position and size (Top-Left corner) based on user's latest values
                int logoWidth = 100;
                int logoHeight = 30;

                // Draw the logo image
                gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

                // Adjust the starting Y position for the Title to be below or aligned with the logo
                y = margin + logoHeight + 10; // Start 10 points below the logo
            }
        }
        catch (Exception ex)
        {
            // Handle error if image fails to load (e.g., file not found, bad stream format)
            // The GetCompanyLogoStream now tries to catch File Not Found and use a placeholder,
            // but we keep this outer catch for other stream/format errors.
            Console.WriteLine($"Error during logo drawing: {ex.Message}");
            y = 40; // Fallback Y position if logo fails
        }

        // --- Draw Header Separator Line ---
        // Draw the line 5 points above the calculated 'y' (which is the title starting point)
        int lineY = y - 5;
        gfx.DrawLine(headerLinePen, margin, lineY, page.Width - margin, lineY);


        var title = invoices.First().ProjectName + " Invoice Report";

        // --- Title and Metadata ---
        // Draw the title based on the updated 'y' position (Title is centered)
        gfx.DrawString(title, titleFont, dataBrush, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
        y += 35; // Move past the title

        // Display general project details above the table
        var firstInv = invoices.First();

        // Use currentX to manage horizontal positioning for labels and values
        int currentX;

        // 1. Purchase Order:
        currentX = margin;
        const string poLabel = "Purchase Order: ";
        const string poValue = "2453";
        gfx.DrawString(poLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(poLabel, headerFont).Width;
        gfx.DrawString(poValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 15;


        // 3. Client: and Project Budget: (Compound Line)
        currentX = margin;
        const string clientLabel = "Client: ";
        const string budgetLabel = " | Project Budget: "; // Separator included in the bold label

        // Client Label (Bold)
        gfx.DrawString(clientLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientLabel, headerFont).Width;

        // Client Value (Regular)
        string clientValue = firstInv.ClientName;
        gfx.DrawString(clientValue, dataFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientValue, dataFont).Width;

        // Budget Label (Includes separator) (Bold)
        gfx.DrawString(budgetLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(budgetLabel, headerFont).Width;

        // Budget Value (Regular)
        string budgetValue = firstInv.Budget.ToString("C0");
        gfx.DrawString(budgetValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 15;

        // 4. Contact Email:
        currentX = margin;
        const string emailLabel = "Contact Email: ";
        string emailValue = firstInv.ClientEmail;
        gfx.DrawString(emailLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(emailLabel, headerFont).Width;
        gfx.DrawString(emailValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 15;

        // 5. Client-Billing Address:
        currentX = margin;
        const string addressLabel = "Client-Billing Address: ";
        const string addressValue = "123 Business Avenue Suite 400, Metropolis, 10001";
        gfx.DrawString(addressLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(addressLabel, headerFont).Width;
        gfx.DrawString(addressValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 15;


        // --- Table Column Definitions (Header, Width, Starting X Position) ---
        var columns = new List<(string Header, int Width, XStringFormat Format)>
        {
            ("Employee", 95, XStringFormats.CenterLeft),
            ("Role", 95, XStringFormats.CenterLeft),
            ("Start Date", 85, XStringFormats.CenterLeft), // User's requested width
            ("End Date", 85, XStringFormats.CenterLeft), // User's requested width
            ("Days", 50, XStringFormats.CenterRight),
            ("Rate/Day", 70, XStringFormats.CenterRight),
            ("Total Cost", 80, XStringFormats.CenterRight),
        };

        currentX = margin;
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

            // FIX: Safely handle nullable RatePerDay (decimal?) for calculation
            decimal rate = inv.RatePerDay ?? 0m;
            var totalCost = rate * (decimal)inv.WorkedDays;

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

            // Col 3: EmployeeStartDate (FIX: Use null-conditional operator for DateTime?)
            gfx.DrawString(inv.EmployeeStartDate?.ToString("dd/MM/yyyy") ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[2].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[2].Width;

            // Col 4: EmployeeEndDate (FIX: Use null-conditional operator for DateTime?)
            gfx.DrawString(inv.EmployeeEndDate?.ToString("dd/MM/yyyy") ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[3].Width, lineHeight), XStringFormats.CenterLeft);
            currentX += columns[3].Width;

            // Col 5: WorkedDays (Right-aligned)
            gfx.DrawString(inv.WorkedDays.ToString("F1"), dataFont, dataBrush, new XRect(currentX, y, columns[4].Width, lineHeight), XStringFormats.CenterRight);
            currentX += columns[4].Width;

            // Col 6: RatePerDay (FIX: Use null-conditional operator for decimal?)
            gfx.DrawString(inv.RatePerDay?.ToString("C0") ?? "N/A", dataFont, dataBrush, new XRect(currentX - 5, y, columns[5].Width, lineHeight), XStringFormats.CenterRight);
            currentX += columns[5].Width;

            // Col 7: Total Cost (Currency, Bold, Right-aligned)
            gfx.DrawString(totalCost.ToString("C0"), headerFont, dataBrush, new XRect(currentX - 5, y, columns[6].Width, lineHeight), XStringFormats.CenterRight);
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


        // 1. --- Print About Sender's Company (Updated for mixed font styling) ---

        // Mock Sender Company Details (Placeholder Data)
        const string issuedByLabel = "Invoice Issued By: ";
        const string senderName = "Mohammed Sameer Ali";
        const string Name = "Abstract Group Ltd";
        const string senderAddressLine1 = "1st Floor, The Coachworks, Harcourt House,";
        const string senderAddressLine2 = "Leeds LS2 7EH, United Kingdom";
        const string senderContact = "Contact: +91 9398158088 | sameerali.mohammed@abstract-group.com";

        // Determine starting Y position for the sender block (e.g., 80 points from the bottom)
        int footerY = (int)page.Height - 110; // Start higher to fit all lines
        int footerX = margin;

        // Draw the label in bold (headerFont)
        gfx.DrawString(issuedByLabel, headerFont, dataBrush, new XPoint(footerX, footerY));

        // Measure the width of the bold label to position the name
        double issuedByLabelWidth = gfx.MeasureString(issuedByLabel, headerFont).Width;

        // Draw the name in regular font (dataFont) immediately after the label
        gfx.DrawString(senderName, dataFont, dataBrush, new XPoint(footerX + issuedByLabelWidth, footerY));

        footerY += 15; // Move to the next line

        // Draw remaining address details
        gfx.DrawString(Name, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderAddressLine1, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderAddressLine2, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderContact, dataFont, dataBrush, new XPoint(footerX, footerY));

        // 2. --- Draw Footer's Line Immediately Below ---
        // Place the line below the contact information, near the bottom margin.
        int finalLineY = (int)page.Height - 20;
        gfx.DrawLine(footerLinePen, margin, finalLineY, page.Width - margin, finalLineY);

        string generatedDateText = $"Report created on {DateTime.Now:dd MMM yyyy}";
        // Position the text 5 points below the line (finalLineY + 5), aligned to the left margin.
        gfx.DrawString(generatedDateText, dataFont, dataBrush, new XPoint(margin, finalLineY + 5), XStringFormats.TopLeft);

        // --- Save and Return ---
        document.Save(stream);
        stream.Position = 0;

        var Naming = invoices.First().ProjectName + " _Invoice_Report";

        return File(stream.ToArray(), "application/pdf", Naming);
    }


    [HttpGet("projects/{projectId}/assignments")]
    public async Task<ActionResult<IEnumerable<ProjectAssignment>>> GetProjectAssignments(int projectId)
    {
        var assignments = await _financeRepo.GetAssignmentSummaryAsync(projectId);
        return Ok(assignments);
    }
}