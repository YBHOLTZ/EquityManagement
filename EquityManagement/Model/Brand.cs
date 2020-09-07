using EquityManagement.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement;

namespace EquityManagement.Model
{
    public class Brand
    {
        [Lib.YboEntityAnnotations.PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
