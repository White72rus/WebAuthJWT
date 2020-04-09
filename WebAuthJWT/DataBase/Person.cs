using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAuthJWT.DataBase
{
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Pass  { get; set; }
        public int IdUser { get; set; }
        public string Role { get; set; }
        public string Login { get; set; }
    }
}
