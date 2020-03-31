using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PineappleShop.Models;
using NPOI.HSSF.UserModel;

namespace PineappleShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly pineapple_shopModel.pineapple_shopModel _shopModel;
        private readonly IWebHostEnvironment _env;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _shopModel = new pineapple_shopModel.pineapple_shopModel();
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View("index", _shopModel);
        }
        public List<pineapple_shopModel.User> GetUsers()
        {
            return _shopModel.Users.ToList();
        }
        public List<pineapple_shopModel.PineappleMenu> GetMenu()
        {
            return _shopModel.PineappleMenus.ToList();
        }
        public IActionResult ImportXcl(IFormFile fileExcel)
        {
            var stream = fileExcel.OpenReadStream();
            XSSFWorkbook hssfwb = new XSSFWorkbook(stream);
            List<string> text = new List<string>();
            List<pineapple_shopModel.PineappleMenu> pineapples = new List<pineapple_shopModel.PineappleMenu>();
            ISheet sheet = hssfwb.GetSheet("Demo");
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    pineapples.Add(new pineapple_shopModel.PineappleMenu
                    {
                        Id = (int)sheet.GetRow(row).GetCell(0).NumericCellValue,
                        Name = sheet.GetRow(row).GetCell(1).StringCellValue,
                        Description = sheet.GetRow(row).GetCell(2).StringCellValue,
                        Weight = (int)sheet.GetRow(row).GetCell(3).NumericCellValue,
                        Price = (int)sheet.GetRow(row).GetCell(4).NumericCellValue
                    });
                }
            }

            foreach(var pa in pineapples)
            {
                if (_shopModel.PineappleMenus.FirstOrDefault(p => p.Id == pa.Id) == null)
                    _shopModel.Add(pa);
            }
            _shopModel.SaveChanges();
            return View("index", _shopModel);
        }
        public IActionResult Add(string name, string description, int price, int weight)
        {
            try
            {
                pineapple_shopModel.PineappleMenu pineapple = new pineapple_shopModel.PineappleMenu
                {
                    Id = _shopModel.PineappleMenus.Max(i => i.Id) + 1,
                    Name = name,
                    Description = description,
                    Price = price,
                    Weight = weight
                };
                _shopModel.PineappleMenus.Add(pineapple);
                _shopModel.SaveChanges();
            }
            catch { };
            return View("index", _shopModel);
        }
        private static byte[] GetBytes(DocumentModel document, SaveOptions options)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, options);
                return stream.ToArray();
            }
        }
        public ActionResult Create()
        {

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            var doc = new DocumentModel();

            // Add document content.
            doc.Sections.Add(new Section(doc, new Paragraph(doc, "Menu generated report!")));
            string row = "";
            foreach (var pineapple in _shopModel.PineappleMenus)
            {
                row += "Id: " + pineapple.Id + "\n";
                row += "Name: " + pineapple.Name + "\n";
                row += "Description: " + pineapple.Description + "\n";
                row += "Weight: " + pineapple.Weight + "\n";
                row += "Price: " + pineapple.Price + "\n";
                row += "\n";
            }
            doc.Sections.Add(new Section(doc, new Paragraph(doc, row)));
            // Save the document to DOCX and PDF file.
            SaveOptions options = SaveOptions.DocxDefault;
            return File(GetBytes(doc, options), options.ContentType, "Create.docx");

        }
        public async Task<IActionResult> GetReport()
        {
            string sWebRootFolder = _env.ContentRootPath;
            string sFileName = @"Output.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                int i = 0;
                IRow row = excelSheet.CreateRow(i);
                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Description");
                row.CreateCell(3).SetCellValue("Weight");
                row.CreateCell(4).SetCellValue("Price");
                foreach ( var pineapple in _shopModel.PineappleMenus)
                {
                    i++;
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(pineapple.Id);
                    row.CreateCell(1).SetCellValue(pineapple.Name);
                    row.CreateCell(2).SetCellValue(pineapple.Description);
                    row.CreateCell(3).SetCellValue(pineapple.Weight);
                    row.CreateCell(4).SetCellValue(pineapple.Price);
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        public IActionResult DeleteSelected(int [] ids)
        {
            foreach (var id in ids)
            {
                var toBeRemoved = _shopModel.PineappleMenus.FirstOrDefault(p => p.Id == id);
                _shopModel.PineappleMenus.Remove(toBeRemoved);
            }
            _shopModel.SaveChanges();
            return View("index", _shopModel);
        }
        public IActionResult Privacy(string a, int b)
        {
            string name = _shopModel.PineappleMenus.FirstOrDefault(p => p.Id == 5).Name;
            return View("Privacy", name);
        }
        private void FillMenuWithTestData()
        {
            for (int i = 0; i < 25; i++)
            {
                pineapple_shopModel.PineappleMenu pineapple = new pineapple_shopModel.PineappleMenu
                {
                    Id = i,
                    Price = i * 15 + 25,
                    Name = "Pineapple " + i,
                    Description = "This is " + i + "th Pineapple in menu",
                    Weight = 1000
                };
                _shopModel.Add(pineapple);
            }
            _shopModel.SaveChanges();
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
