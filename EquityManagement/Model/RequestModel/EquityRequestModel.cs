using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityManagement.Model.RequestModel
{
    public class EquityRequestModel
    {
        public string Name { get; set; }
        public int BrandID { get; set; } //MarcaID
        public string Description { get; set; }        
    }
}
