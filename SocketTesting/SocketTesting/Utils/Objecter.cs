using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class Objecter
{
    public float Posx { get; set; }
    public float Posy { get; set; }
    public float width { get; set; }
    public float height { get; set; }

    public Objecter() { }
    public Objecter(JObject jsonObject)
    {
        this.Posx = (float) jsonObject["posx"];
        this.Posy = (float) jsonObject["posy"];
        this.width = (float) jsonObject["width"];
        this.height = (float) jsonObject["height"];
    }
}
