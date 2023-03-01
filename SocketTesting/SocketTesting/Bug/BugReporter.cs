using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class BugReporter
{
    public static FileStream reportFile;
    public static List<Bug> bugList = new List<Bug>();

    public static void Report()
    {
        string path = Directory.GetCurrentDirectory();
        string reportDate = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss");
        reportFile = new FileStream(path + "\\Report\\" + reportDate + ".docx", FileMode.Create, FileAccess.Write);
        XWPFDocument doc = new XWPFDocument();

        writeLineToDocument(doc, $"Start: {reportDate}");

        writeLineToDocument(doc, $"Number of ran tests: {StatusSheet.targetedTestCaseList.Count}");
        writeLineToDocument(doc, $"Number of passed test: {TestCaseSheet.passedTest}");
        writeLineToDocument(doc, $"Number of failed test: {StatusSheet.targetedTestCaseList.Count - TestCaseSheet.passedTest}");

        writeLineToDocument(doc, "");

        writeLineToDocument(doc, "Bug list: ");

        foreach(Bug bug in bugList)
        {
            reportBug(doc, bug);
        }

        writeLineToDocument(doc, $"End: {reportDate}");
    }

    public static void reportBug(XWPFDocument doc, Bug bug)
    {
        writeLineToDocument(doc, "");
        writeLineToDocument(doc, "");

        writeLineToDocument(doc, $"Test case ID: {bug.getTestCaseID()}");
        writeLineToDocument(doc, $"Step ID: {bug.getStepID()}");
        writeLineToDocument(doc, $"Device: {bug.getDevice()}");
        writeLineToDocument(doc, $"Description: {bug.getDescription()}");

        addImageToDocument(doc, bug.getEvidence());

        writeLineToDocument(doc, "");
        writeLineToDocument(doc, "");
    }

    public static void writeLineToDocument(XWPFDocument doc, string text)
    {
        XWPFParagraph newLine = doc.CreateParagraph();
        XWPFRun run = newLine.CreateRun();
        run.SetText(text);
        doc.Write(reportFile);
    }

    public static void addImageToDocument(XWPFDocument doc, string ImgPath)
    {
        XWPFParagraph newLine = doc.CreateParagraph();
        XWPFRun run = newLine.CreateRun();
        try
        {
            FileStream imgStream = new FileStream(ImgPath, FileMode.Open, FileAccess.Read);

            int imgType = XSSFWorkbook.PICTURE_TYPE_PNG;
            string imgFileName = imgStream.Name;

            int width = 500;
            int height = 300;

            run.AddPicture(imgStream, imgType, imgFileName, width, height);
            doc.Write(reportFile);
        } catch(Exception ex)
        {
           throw new Exception($"can not get evidence from file path {ImgPath}");
        }
    }
}
