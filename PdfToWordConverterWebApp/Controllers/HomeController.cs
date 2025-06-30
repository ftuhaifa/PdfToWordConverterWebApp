using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // ✅ Required for IFormFile
using Spire.Pdf;
using System.IO;

namespace PdfToWordConverterWebApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConvertPdfToWord(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Save uploaded file to a temp path
            var pdfPath = Path.GetTempFileName();
            using (var stream = new FileStream(pdfPath, FileMode.Create))
            {
                pdfFile.CopyTo(stream);
            }

            // Load and convert PDF to Word
            var doc = new PdfDocument();
            doc.LoadFromFile(pdfPath);

            var wordPath = Path.ChangeExtension(pdfPath, ".docx");
            doc.SaveToFile(wordPath, FileFormat.DOCX);
            doc.Close();

            // Return converted Word file
            var fileBytes = System.IO.File.ReadAllBytes(wordPath);
            var fileName = Path.GetFileNameWithoutExtension(pdfFile.FileName) + ".docx";

            return File(fileBytes, 
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                        fileName);
        }
    }
}
