using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityManagement.Model.RequestModel
{
    public class ResultRequestModel
    {
        public string Message { get; set; }
        public int Status { get; set; }

        public ResultRequestModel(int status, string message)
        {
            this.Status = status;
            Message = message;
        }
    }
}
