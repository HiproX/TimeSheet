using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    internal class Employee
    {
        public enum PositionType : int
        {
            Intern,
            Cook,
            ITDirector,
            Programmer,
        }
        public string Name { get; private set; }
        public PositionType Position { get; private set; }
        public int TabelNumber { get; private set; }

        public Employee()
        {

        }
    }
}
