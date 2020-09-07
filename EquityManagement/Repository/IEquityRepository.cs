using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement.Model;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Repository
{
    interface IEquityRepository
    {
        List<Equity> GetAll();

        Equity Find(int id);

        ResultRequestModel InsertNew(Equity equity);

        ResultRequestModel Update(Equity equity);

        int Remove(int id);
    }
}
