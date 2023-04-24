using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2Txt.Models
{
    public class AIResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Text{ get; set; }
        
        public AIResponse()
        {
            this.Code = "N/A";
            this.Name = "N/A";
            this.Text = "N/A";
        }
        public AIResponse(string code, string text)
        {
            this.Code = code;
            this.Name = "No Name";
            this.Text = text;
        }
        public AIResponse(string code, string name, string text)
        {
            this.Code = code;
            this.Name = name;
            this.Text = text;
        }
    }
}
