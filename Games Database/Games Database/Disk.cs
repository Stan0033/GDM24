using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games_Database
{
    public  class Disk
    {
        public List<Record>? Single {  get; set; }
        public List<Record>? Series {  get; set; }
        public List<Record>? Sports {  get; set; }
        public List<Record>? Simulation {  get; set; }
        public List<Record>? Authors {  get; set; }
    }
}
