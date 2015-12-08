using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTD_Proj_01
{
   public  class MachineTask 
    {
        public int indexTask { get; set; }
        // public Dictionary<int,int> machine { get; set; }
        public int[] duration { get; set; }
        public int[] start { get; set; }
        public int[]end { get; set; }
        public MachineTask()
        {
            //machine = new Dictionary<int, int>();
            duration = new int[3];
            start = new int[3];
            end = new int[3];

        }
        
    }
}
