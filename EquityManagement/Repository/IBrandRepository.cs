using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement.Model;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Repository
{
    public interface IBrandRepository
    {
        List<Model.Brand> GetAll();

        Brand Find(int id);

        List<Equity> GetEquities(int id_brand);

        ResultRequestModel InsertNew(Brand brand);

        bool Update(Brand brand);

        ResultRequestModel Remove(int id);

    }
}
