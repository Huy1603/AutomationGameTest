using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatusSheet{
    public const int COLUMN_INDEX_TEST_CASE = 0;
    public const int COLUMN_INDEX_STATUS = 1;
    public const int STATUS_ROW_START_INDEX = 1;

    public static List<string> targetedTestCaseList = new List<string>();

    public static void readStatusSheet(ISheet sheet)
    {
        foreach(IRow row in sheet)
        {
            if (row.RowNum < STATUS_ROW_START_INDEX) continue;
            
            ICell firstCell = row.GetCell(0);

            if(firstCell == null || firstCell.ToString().Equals("")) continue;
            readStatusLine(row);
        }
    }

    public static void readStatusLine(IRow row)
    {
        ICell firstCell = row.GetCell(0);
        string curTestCase = firstCell.StringCellValue;
        for(int i = 1; i < row.LastCellNum; ++i)
        {
            ICell cell = row.GetCell(i);
            Object cellValue = ReadAndExecuteExcel.getCellvalue(cell);
            if (cellValue == null || cellValue.ToString().Equals("")) continue;
            int colIndex = cell.ColumnIndex;
            
            switch(colIndex)
            {
                case COLUMN_INDEX_TEST_CASE:
                    if (!cellValue.ToString().StartsWith("TC_"))
                    {
                        throw new Exception("Wrong test case status input.");
                    }
                    break;
                case COLUMN_INDEX_STATUS:
                    if (cellValue.ToString().Equals("test"))
                    {
                        Console.WriteLine(cellValue);
                        targetedTestCaseList.Add(cellValue.ToString());
                        continue;
                    }
                    if (cellValue.ToString().Equals("ignore"))
                    {
                        continue;
                    }
                    else
                    {
                        throw new Exception("Wrong test case status input.");
                    }
                    break;
                default: break;
            }
        }
    }
}
