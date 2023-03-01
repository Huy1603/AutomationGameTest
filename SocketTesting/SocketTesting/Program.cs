using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json.Linq;
using System.Security.AccessControl;

public class WebSocket
{
    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 5000;
    IPAddress address = IPAddress.Parse("127.0.0.1");
    static ASCIIEncoding encoding = new ASCIIEncoding();
    static TcpClient client = new TcpClient();
    static Stream stream;
    public static JObject configJSON = JObject.Parse(File.ReadAllText(@"config\config.json"));

    static void Main()
    {
        try
        {
            // Connect to the server
            //IPAddress address = IPAddress.Parse("127.0.0.1");

            //Set up socket server
            IPAddress address = IPAddress.Parse((string) configJSON["Device"]["IPAddress"]);
            client.Connect(address, PORT_NUMBER);
            stream = client.GetStream();

            string excelFilepath = Directory.GetCurrentDirectory() + "\\ExcelFile\\TestCase.xlsx";
            try
            {
                ReadAndExecuteExcel.readExcel(excelFilepath);
            }
            catch
            {
                throw new Exception("can not find input excel file.");
            }

            try
            {
                BugReporter.Report();
            }
            catch 
            { 
                throw new Exception("can not find output docx file.");
            }
            /*
            string hello = "Hello from client";
            byte[] data = encoding.GetBytes(hello);
            SendPackage(PackageDefine.PKT_HELLO, "Hello");
            string response = GetResponse();
            Console.WriteLine(response);
            SendPackage(PackageDefine.PKT_ABC, "ABC");
            ExcelStep excelStep = new ExcelStep("click", "button", "ClickButton");
            excelStep.executeAction();*/
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public static void SendPackage(PackageDefine PKT_DEF, string PKT_JSON)
    {
        byte[] data = encoding.GetBytes(PKT_JSON);
        stream.WriteAsync(data, 0, data.Length);
    }

    public static string GetResponse()
    {
        byte[] data = new byte[BUFFER_SIZE];
        stream.Read(data, 0, data.Length);
        return encoding.GetString(data);
    }
    public static packageFindanswer find(String filter)
    {
        networkPackage pkt = new networkPackage
        {
            name = filter,
            action = "find"
        };
        string jsonstring = JsonConvert.SerializeObject(pkt);
        WebSocket.SendPackage(PackageDefine.PKT_FIND, jsonstring);
        string response = WebSocket.GetResponse();
        Console.WriteLine(response);
        return JsonConvert.DeserializeObject<packageFindanswer>(response);
    }
}


public enum PackageDefine
{
    PKT_HELLO,
    PKT_ABC,
    PKT_CHECK,
    PKT_FIND
}

[Serializable]
public class packageFind
{
    public string name;
    public string type;
}

[Serializable]
public class packageFindanswer
{
    public bool exist;
    public float Posx;
    public float Posy;
    public float width;
    public float height;
}

[Serializable]
public class networkPackage
{
    public string name;
    public string action;
}