using EquityManagement.Model.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityManagement.Repository
{
    interface ITokenRepository
    {
        bool IsValid(string token);

        ResultRequestModel CheckToken(string token);
    }
}
