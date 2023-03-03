using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

public class TestCaseSheet
{
    public static string nameAndValueSeparator = ":";
    public static string bugStepIDAndDescriptionSeparator = "ERROR:";
    public static int passedTest = 0;
    public const int COLUMN_INDEX_ID = 0;
    public const int COLUMN_INDEX_DRIVER = 1;
    public const int COLUMN_INDEX_ACTION = 2;
    public const int COLUMN_INDEX_PARAMETER = 3;

    public static void readAndExecTestCaseSheet(ISheet sheet)
    {
        int testCaseID;
        string sheetName = sheet.SheetName;
        try
        {
            testCaseID = Int32.Parse(sheetName.Substring(3));
        }
        catch (Exception ex)
        {
            throw new Exception($"can not get test case index from sheet: {sheetName}.");
            return;
        }

        int startRow = ReadAndExecuteExcel.getStartRowOfStep(sheet);
        string testResult = "Passed";
        foreach(IRow row in sheet)
        {
            if (row.RowNum < startRow) continue;

            ICell firstCell = row.Cells[0];
            if(firstCell == null) continue;

            ExcelStep step = readStepLine(row);
            if (step.getStepID() == null) continue;

            Console.WriteLine($"Currenly on {step.ToString()}");

            if (step.getAction() != null && step.getAction().StartsWith("SHEET:"))
            {
                //Write code using a group of command
            }
            else
            {
                try
                {
                    step.execute();
                } catch (Exception ex)
                {
                    testResult = addBug(testCaseID, step.getDriver(), ex.ToString(), step.getEvidence());
                }
            }
        }

        if (testResult.Equals("Passed")) passedTest++;
    }

    public static ExcelStep readStepLine(IRow row)
    {
        ExcelStep step = new ExcelStep();
        for (int i = 0; i < row.LastCellNum; ++i)
        {
            ICell cell = row.GetCell(i);
            if (cell == null || cell.ToString().Equals("")) continue;
            int colIndex = cell.ColumnIndex;
            switch (colIndex)
            {
                case COLUMN_INDEX_ID:
                    step.setStepID(cell.ToString()); break;
                case COLUMN_INDEX_DRIVER:
                    step.setDriver(cell.ToString()); break;
                case COLUMN_INDEX_ACTION:
                    step.setAction(cell.ToString()); break;
                case COLUMN_INDEX_PARAMETER:
                    step.setParam(cell.ToString()); break;
                default: break;
            }
        }
        return step;
    }

    public static string addBug(int testCaseID, string device, string message, string evidence)
    {
        Bug bug = new Bug();
        Console.WriteLine("message " + message);
        try
        {
            string bugStepID = message.Split(bugStepIDAndDescriptionSeparator, 2)[0];
            string bugDescription = message.Split(bugStepIDAndDescriptionSeparator, 2)[1];

            bug = new Bug(testCaseID, device, bugStepID, bugDescription, evidence);
            BugReporter.bugList.Add(bug);

            return "Failed";
        } catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception("Can not get description.");
        }
    }
}
