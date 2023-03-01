using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ADBDeviceSelector
{
    public static ADBObject GetADBObject(string name)
    {
        if (name.Substring(0, name.Length - 1).Equals("Device")){
            int id;
            try
            {
                id = Int32.Parse(name.Substring(name.Length - 1));
            }
            catch (Exception ex) {
                throw new Exception("Device id isnt a number.");
            }
            return Device.getInstance(id);
        }
        else throw new Exception("Device not exist.");
    }

    public static string getClassname(string name)
    {
        return "Device";
    }
}
