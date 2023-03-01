using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public class Device : ADBObject
{
    public int id;
    public int screenWidth { get; set; }
    public int screenHeight { get; set; }
    public int gameWidth {get; set; }
    public int gameHeight { get; set; }
    public string adbName { get; set; }
    Process process = new Process();
    private static Device[] devices = new Device[10];

    public Device(int _id)
    {
        this.id = _id;
        this.adbName = (string) WebSocket.configJSON[$"Device{id}"]["adbName"];
        this.gameWidth = (int) WebSocket.configJSON["gameWidth"];
        this.gameHeight = (int)WebSocket.configJSON["gameHeight"];
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        try
        {
            process.StartInfo.Arguments = $"adb -s {adbName} shell wm size";
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            throw new Exception("Device not plugged in");
        }

        string result = process.StandardOutput.ReadToEnd();
        this.screenWidth = Int32.Parse(result.Substring(result.LastIndexOf(' ') + 1, result.LastIndexOf('x')));
        this.screenHeight = Int32.Parse(result.Substring(result.LastIndexOf('x') + 1));
    }

    public static Device getInstance(int _id)
    {
        if (devices[_id] == null) devices[_id] = new Device(_id);
        return devices[_id];
    }
    public Vector2 ScreenLocation(float x, float y)
    {
        return new Vector2(x * (float)screenWidth / (float)gameWidth, (gameHeight - y) * (float)screenHeight / (float)gameHeight);
        //720 x 1600
        //1080 x 1920
    }

    public void sleep(string time)
    {
        try
        {
            Thread.Sleep(Int32.Parse(time));
        } catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void openGame()
    {
        string packageName = (string)WebSocket.configJSON["packageName"];
        process.StartInfo.Arguments = $"adb -s {adbName} shell monkey -p {packageName} 1";
        process.Start();
        process.WaitForExit();
    }

    public void closeGame()
    {
        string packageName = (string)WebSocket.configJSON["packageName"];
        process.StartInfo.Arguments = $"adb -s {adbName} shell am force-stop {packageName}";
        process.Start();
        process.WaitForExit();
    }

    public void click(String filter)
    {
        packageFindanswer pkt = WebSocket.find(filter);
        if (pkt == null) throw new Exception("find package cannot be found.");
        if (!pkt.exist) throw new Exception($"gameObject name {filter} doesnt exist.");
        Vector2 gameObjectLocation = ScreenLocation(pkt.Posx, pkt.Posy);
        process.StartInfo.Arguments = $"adb -s {adbName} shell input tap {gameObjectLocation.X} {gameObjectLocation.Y}";
        process.Start();
        process.WaitForExit();
    }

    public void type(String text)
    {
        Thread.Sleep(100);
        process.StartInfo.Arguments = $"adb -s {adbName} shell input text {text}";
        process.Start();
        process.WaitForExit();
        Thread.Sleep(100);
    }

    public void removeText()
    {
        string eventCodes = "67"; //KEYCODE_DEL
        process.StartInfo.Arguments = $"adb -s {adbName} shell input keyevent {eventCodes}";
        process.Start();
        process.WaitForExit();
        Thread.Sleep(200);
    }

    public string captureScreen()
    {
        try
        {
            string path = Directory.GetCurrentDirectory();
            string fileName = path + "\\Evidence\\" + DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss");
            process.StartInfo.Arguments = $"adb -s {adbName} shell screencap -p /sdcard/temp.png";
            process.Start();
            process.WaitForExit();
            process.StartInfo.Arguments = $"adb -s {adbName} pull /sdcard/temp.png {fileName}";
            process.Start();
            process.WaitForExit();
            Thread.Sleep(100);
            return fileName;
        }
        catch(Exception ex)
        {
            return null;
        }
    }
}