using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSheet
{
    internal class Department
    {
        public Department()
        {

        }
        public Department(int id, string name)
        {
            _id = id;
            Name = name;
        }
        public string Name { get; private set; }
        private int _id;
    }
}
