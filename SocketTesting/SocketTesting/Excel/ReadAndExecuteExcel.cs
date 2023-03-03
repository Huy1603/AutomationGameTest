using MathNet.Numerics.LinearAlgebra.Solvers;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReadAndExecuteExcel
{

    public static void readExcel(string excelFilePath)
    {
        XSSFWorkbook wb;
        try
        {
            Console.WriteLine(excelFilePath);
            wb = getWorkbook(new FileStream(excelFilePath, FileMode.Open, FileAccess.Read));
        } catch
        {
            throw new Exception("can not find excel file in current path.");
        }

        for(int i = 0; i < wb.NumberOfSheets; ++i)
        {
            ISheet sheet = wb.GetSheetAt(i);
            string sheetName = sheet.SheetName;
            if (sheetName.Equals("Status"))
            {
                Console.WriteLine($"Reading status sheet {sheetName}:");
                StatusSheet.readStatusSheet(sheet);
            }
            else if (sheetName.StartsWith("GROUP_"))
            {

            }
            else if (sheetName.StartsWith("TC_"))
            {
                Console.WriteLine($"Reading and executing {sheetName}:");
                TestCaseSheet.readAndExecTestCaseSheet(sheet);
            }
        }

        wb.Close();
    }

    public static XSSFWorkbook getWorkbook(FileStream inputFile)
    {
        try{
            XSSFWorkbook wb = new XSSFWorkbook(inputFile);
            return wb;
        } catch(Exception e) {
            throw new Exception($"input file {inputFile.ToString()} is not excel file.");
        }
        return null;
    }

    public static Object getCellvalue(ICell cell)
    {
        CellType cellType = cell.CellType;
        Object cellValue = null;
        switch (cellType)
        {
            case CellType.Boolean: cellValue = cell.BooleanCellValue; break;
            case CellType.String: cellValue = cell.StringCellValue.Trim(); break;
            case CellType.Numeric: cellValue = cell.NumericCellValue; break;
            default: break;
        }
        return cellValue;
    }

    public static int getStartRowOfStep(ISheet sheet)
    {
        foreach(IRow row in sheet)
        {
            ICell firstCell = row.GetCell(0);
            if (firstCell.StringCellValue.Trim().Equals("Step"))
            {
                return row.RowNum + 2;
            }
        }
        return -1;
    }
}
