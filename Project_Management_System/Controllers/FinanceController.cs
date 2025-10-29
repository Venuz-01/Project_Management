using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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
            worksheet.Cell(row, 7).Value = inv.ClientName;
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
        //const string logoPath = @"C:\Users\adich\Downloads\Abstractlogo.jpg";
        const string logoPath = @"C:\Users\mdsam\OneDrive\Desktop\proj\bannerlogo (1).jpg";
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
        var FootertitleFont = new XFont("Verdana", 7, XFontStyle.Bold);
        var FooterFont = new XFont("Verdana", 6, XFontStyle.Regular);
        var headerBrush = XBrushes.White;
        var dataBrush = XBrushes.Black;
        var headerBackground = XBrushes.DarkSlateGray;
        var headerLinePen = new XPen(XColors.LightGray, 1); // Pen for the separator line
        var footerLinePen = new XPen(XColors.Black, 0.5); // Pen for the final footer line

        const int margin = 20;
        int y = 40;
        const int lineHeight = 20;

        // --- Financial Calculation Initialization ---
        // Initialize the subtotal accumulator before the loop
        decimal overallSubtotal = 0m;

        // --- LOGO INSERTION (New Section) ---
        try
        {
            using (var logoStream = GetCompanyLogoStream())
            {
                XImage logo = XImage.FromStream(() => logoStream);

                int logoWidth = 100;
                int logoHeight = 30;

                gfx.DrawImage(logo, margin, margin, logoWidth, logoHeight);

                y = margin + logoHeight + 10; // Start 10 points below the logo
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logo drawing: {ex.Message}");
            y = 40; // Fallback Y position if logo fails
        }

        // --- Draw Header Separator Line ---
        int lineY = y - 5;
        gfx.DrawLine(headerLinePen, margin, lineY, page.Width - margin, lineY);


        var title = invoices.First().ProjectName + " Invoice";

        // --- Title and Metadata ---
        gfx.DrawString(title, titleFont, dataBrush, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
        y += 45; // Keeps the spacing from the user's latest snippet

        // Display general project details above the table
        var firstInv = invoices.First();

        // Use currentX to manage horizontal positioning for labels and values
        int currentX;

        // 1. Invoice Date:
        currentX = margin;
        const string InvoiceDate = "Invoice Date: ";

        gfx.DrawString(InvoiceDate, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(InvoiceDate, headerFont).Width;

        string InvoiceDateV = DateTime.Now.ToString("dd MMM yyyy"); ;
        gfx.DrawString(InvoiceDateV, dataFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(InvoiceDateV, dataFont).Width;
        y += 15;

        // 2. Purchase Order:
        currentX = margin;
        const string poLabel = "Purchase Order: ";
        const string poValue = "PO2453";
        gfx.DrawString(poLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(poLabel, headerFont).Width;
        gfx.DrawString(poValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 30; // Spacing after PO


        currentX = margin;
        const string clientEmail = "Company Name: ";

        gfx.DrawString(clientEmail, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientEmail, headerFont).Width;

        string clientEmailv = firstInv.ProjectName;
        gfx.DrawString(clientEmailv, dataFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientEmailv, dataFont).Width;
        y += 15;

        // 3. Contact Name:
        currentX = margin;
        const string clientLabel = "Contact Name: ";

        gfx.DrawString(clientLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientLabel, headerFont).Width;

        string clientValue = firstInv.ClientName;
        gfx.DrawString(clientValue, dataFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(clientValue, dataFont).Width;
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
        const string addressLabel = "Billing Address: ";
        const string addressValue = "123 Business Avenue Suite 400, Metropolis, 10001";
        gfx.DrawString(addressLabel, headerFont, dataBrush, new XPoint(currentX, y));
        currentX += (int)gfx.MeasureString(addressLabel, headerFont).Width;
        gfx.DrawString(addressValue, dataFont, dataBrush, new XPoint(currentX, y));
        y += 15;


        // --- Table Column Definitions (Header, Width, Starting X Position) ---
        var columns = new List<(string Header, int Width, XStringFormat Format)>
        {
            ("Role", 100, XStringFormats.CenterLeft),  // Col 0: Left-Aligned
            ("Start Date", 90, XStringFormats.Center),    // Col 1: Center-Aligned
            ("End Date", 85, XStringFormats.Center),      // Col 2: Center-Aligned
            ("Days", 40, XStringFormats.Center),          // Col 3: Center-Aligned
            ("Allocation", 100, XStringFormats.Center), // Col 4: Center-Aligned
            ("Rate/Day", 70, XStringFormats.Center),      // Col 5: Center-Aligned
            ("Cost", 80, XStringFormats.CenterRight),     // Col 6: Right-Aligned
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

            decimal rate = inv.RatePerDay ?? 0m;
            var WorkedDays = (inv.WorkedDays - inv.HolidayCount - inv.LeaveCount) * (inv.AllocationPercent / 100);
            var totalCost = (rate * (decimal)WorkedDays);

            // ACCUMULATION: Add the calculated cost to the overall subtotal
            overallSubtotal += totalCost;

            // Alternate row colors for readability
            if (invoices.IndexOf(inv) % 2 == 1)
            {
                var rowRect = new XRect(margin, y, columns.Sum(c => c.Width), lineHeight);
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 245, 245)), rowRect);
            }

            // Draw Data Cells in Order

            // Col 0: RoleName (Left-aligned)
            gfx.DrawString(inv.RoleName, dataFont, dataBrush, new XRect(currentX, y, columns[0].Width, lineHeight), columns[0].Format);
            currentX += columns[0].Width;

            // Col 1: EmployeeStartDate (Center-aligned)
            gfx.DrawString(inv.EmployeeStartDate?.ToString("dd/MM/yyyy") ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[1].Width, lineHeight), columns[1].Format);
            currentX += columns[1].Width;

            // Col 2: EmployeeEndDate (Center-aligned)
            gfx.DrawString(inv.EmployeeEndDate?.ToString("dd/MM/yyyy") ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[2].Width, lineHeight), columns[2].Format);
            currentX += columns[2].Width;

            // Col 3: WorkedDays (Center-aligned)
            gfx.DrawString(WorkedDays.ToString("F0"), dataFont, dataBrush, new XRect(currentX, y, columns[3].Width, lineHeight), columns[3].Format);
            currentX += columns[3].Width;

            // Col 4: AllocationPercent (Center-aligned)
            gfx.DrawString(inv.AllocationPercent.ToString("F1")+"%", dataFont, dataBrush, new XRect(currentX, y, columns[4].Width, lineHeight), columns[4].Format);
            currentX += columns[4].Width;

            // Col 5: RatePerDay (Center-aligned)
            gfx.DrawString("£ " + inv.RatePerDay?.ToString() ?? "N/A", dataFont, dataBrush, new XRect(currentX, y, columns[5].Width, lineHeight), columns[5].Format);
            currentX += columns[5].Width;

            // Col 6: Total Cost (Currency, Bold, Right-aligned)
            gfx.DrawString("£ " + totalCost.ToString(), headerFont, dataBrush, new XRect(currentX - 5, y, columns[6].Width, lineHeight), columns[6].Format);
            currentX += columns[6].Width;


            y += lineHeight; // Move to the next row

            // Basic safety check for page break
            if (y > page.Height - 120)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = margin;
                // Redraw headers here if needed for new page
            }
        }

        // Calculate Totals after iterating through all items
        decimal vatRate = 0.20m;
        decimal vatAmount = overallSubtotal * vatRate;
        decimal grandTotalAmount = overallSubtotal + vatAmount;

        // --- Draw Summary Table (2x2) Below Main Data ---
        y += 30; // Add 30 points of space below the main table

        // Define column widths for the summary table (reusing last two columns of the main table)
        var costCol = columns[6]; // Width: 80 (Value)
        var rateCol = columns[5]; // Width: 70 (Label)
        var summaryTableWidth = costCol.Width + rateCol.Width;
        var summaryX = (int)page.Width - margin - summaryTableWidth;
        int summaryY = y;

        // Row 1: Subtotal (Now calculated)
        // Cell 1 (Label) - Bold
        var labelRect1 = new XRect(summaryX, summaryY, rateCol.Width, lineHeight);
        gfx.DrawString("Subtotal:", headerFont, dataBrush, labelRect1, XStringFormats.CenterLeft);

        // Cell 2 (Value) - Regular
        var valueRect1 = new XRect(summaryX + rateCol.Width, summaryY, costCol.Width, lineHeight);
        gfx.DrawString("£ "+overallSubtotal.ToString(), dataFont, dataBrush, valueRect1, XStringFormats.CenterRight);

        summaryY += lineHeight;

        // Row 2: Tax / VAT (Now calculated at 20%)
        // Cell 3 (Label) - Bold
        var labelRect2 = new XRect(summaryX, summaryY, rateCol.Width, lineHeight);
        // Update label to reflect the calculated rate
        gfx.DrawString($"Tax ({vatRate:P0}):", headerFont, dataBrush, labelRect2, XStringFormats.CenterLeft);

        // Cell 4 (Value) - Regular
        var valueRect2 = new XRect(summaryX + rateCol.Width, summaryY, costCol.Width, lineHeight);
        gfx.DrawString("£ " + vatAmount.ToString(), dataFont, dataBrush, valueRect2, XStringFormats.CenterRight);

        summaryY += lineHeight + 5; // Add extra space before Grand Total (or next section)

        // Row 3: Grand Total (Now calculated)
        // Cell 5 (Label) - Title Font
        var labelRect3 = new XRect(summaryX, summaryY, rateCol.Width, lineHeight);
        gfx.DrawString("Grand Total:", headerFont, dataBrush, labelRect3, XStringFormats.CenterLeft);

        // Cell 6 (Value) - Title Font
        var valueRect3 = new XRect(summaryX + rateCol.Width, summaryY, costCol.Width, lineHeight);
        gfx.DrawString("£ " + grandTotalAmount.ToString(), dataFont, dataBrush, valueRect3, XStringFormats.CenterRight);

        y = summaryY + lineHeight + 30; // Update Y for the rest of the document (footer)

        // 1. --- Print About Sender's Company (Updated for mixed font styling) ---

        // Mock Sender Company Details (Placeholder Data)
        //const string issuedByLabel = "Invoice Issued By: ";
        const string senderName = "Mohammed Sameer Ali";
        const string Name = "Abstract Group Ltd";
        const string senderAddressLine1 = "1st Floor, The Coachworks, Harcourt House,";
        const string senderAddressLine2 = "Leeds LS2 7EH, United Kingdom";
        const string senderContact = "Contact: +91 9398158088";
        const string senderEmail = "sameerali.mohammed@abstract-group.com";
        // If the table ran close to the footer, we might need a page break here.
        if (y > page.Height - 120)
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            y = margin;
        }   

        // Determine starting Y position for the sender block
        int footerY = y+35;
        int footerX = margin;

        // Draw the label in bold (headerFont)
        //gfx.DrawString(issuedByLabel, headerFont, dataBrush, new XPoint(footerX, footerY));

        //// Measure the width of the bold label to position the name
        //double issuedByLabelWidth = gfx.MeasureString(issuedByLabel, headerFont).Width;

        // Draw the name in regular font (dataFont) immediately after the label
        gfx.DrawString(senderName, dataFont, dataBrush, new XPoint(footerX , footerY));

        footerY += 30; // Move to the next line

        // Draw remaining address details
        gfx.DrawString(Name, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderAddressLine1, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderAddressLine2, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderContact, dataFont, dataBrush, new XPoint(footerX, footerY));
        footerY += 15;
        gfx.DrawString(senderEmail, dataFont, dataBrush, new XPoint(footerX, footerY));

        const string BankName = "Wessex Bank plc";
        const string Account = "98765432";
        const string sortcode = "12-34-56";
        const string BIC = "WESTGBAV";
        const string iBAN = "GB82 WEST 1234 5698 7654 32";

        // If the table ran close to the footer, we might need a page break here.
        if (y > page.Height - 120)
        {
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            y = margin;
        }

        // Determine starting Y position for the bank details block, fixed near the bottom.
        int foooterY = (int)page.Height - 110; // Initial fixed position
        int foooterX = margin;

        // Draw Bank Name
        gfx.DrawString(BankName, FooterFont, dataBrush, new XPoint(foooterX, foooterY));

        foooterY += 10; // Move to the next line

        // Draw Account
        gfx.DrawString(Account, FooterFont, dataBrush, new XPoint(foooterX, foooterY));
        foooterY += 10;

        // Draw Sort Code
        gfx.DrawString(sortcode, FooterFont, dataBrush, new XPoint(foooterX, foooterY));
        foooterY += 10;

        // Draw BIC
        gfx.DrawString(BIC, FooterFont, dataBrush, new XPoint(foooterX, foooterY));
        foooterY += 10;

        // Draw IBAN
        gfx.DrawString(iBAN, FooterFont, dataBrush, new XPoint(foooterX, foooterY));
        // --- End Bank Details Section ---


        // 2. --- Draw Footer's Line Immediately Below ---
        int finalLineY = (int)page.Height - 40;
        gfx.DrawLine(footerLinePen, margin, finalLineY, page.Width - margin, finalLineY);

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