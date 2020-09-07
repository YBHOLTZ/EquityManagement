using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityManagement.Model
{
    public class Equity
    {
        [Lib.YboEntityAnnotations.PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }
        public int BrandID { get; set; } //MarcaID
        public string Description { get; set; }
        public int Register { get; set; }
    }
}
