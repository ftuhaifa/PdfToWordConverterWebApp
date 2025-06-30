using Microsoft.AspNetCore.Mvc;
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

            var pdfPath = Path.GetTempFileName();
            using (var stream = new FileStream(pdfPath, FileMode.Create))
            {
                pdfFile.CopyTo(stream);
            }

            var doc = new PdfDocument();
            doc.LoadFromFile(pdfPath);

            var wordPath = Path.ChangeExtension(pdfPath, ".docx");
            doc.SaveToFile(wordPath, FileFormat.DOCX);
            doc.Close();

            var fileBytes = System.IO.File.ReadAllBytes(wordPath);
            var fileName = Path.GetFileNameWithoutExtension(pdfFile.FileName) + ".docx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }
    }
}