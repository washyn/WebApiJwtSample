// See https://aka.ms/new-console-template for more informatio
using ClosedXML.Excel;
using Ganss.Excel;
using OfficeOpenXml;
using Org.BouncyCastle.Math.EC;

Console.WriteLine("Hello, World!");


var data = new List<Data>()
{
    new Data()
    {
        Type = 3
    }
};

new ExcelMapper().Save("result.xlsx", data, "Hoja");
//

// If you are a commercial business and have
// purchased commercial licenses use the static property
// LicenseContext of the ExcelPackage class :
ExcelPackage.LicenseContext = LicenseContext.Commercial;

// If you use EPPlus in a noncommercial context
// according to the Polyform Noncommercial license:
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

using(var package = new ExcelPackage(@"myWorkbookEp.xlsx"))
{
    var sheet = package.Workbook.Worksheets.Add("My Sheet");
    sheet.Cells["A1"].Value = "Hello World!";

    // Save to file
    package.Save();
}

// using (var workbook = new XLWorkbook())
// {
//     var worksheet = workbook.Worksheets.Add("Sample Sheet");
//     worksheet.Cell("A1").Value = "Hello World!";
//     worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";
//     workbook.SaveAs("HelloWorld.xlsx");
// }


public class Data
{
    public int Type { get; set; }
}
