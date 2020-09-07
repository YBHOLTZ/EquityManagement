using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement.Model;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Repository
{
    public class BrandRepository : IBrandRepository
    {
        readonly YboEntityFramework _yboEntity = null;
        public BrandRepository(YboEntityFramework yboEntityFramework)
        {
            _yboEntity = yboEntityFramework;
        }

        public Brand Find(int id)
        {
            return _yboEntity.Find<Brand>(id);
        }

        public List<Brand> GetAll()
        {
            return _yboEntity.All<Brand>();            
        }

        public List<Equity> GetEquities(int id_brand)
        {
            string query = $@"SELECT E.* FROM Equity AS E INNER JOIN Brand AS B on B.ID = E.BrandID where B.ID = @P0";
            return _yboEntity.FreeQuery<Equity>(query, id_brand);            
        }

        public ResultRequestModel InsertNew(Brand brand)
        {
            ResultRequestModel result = new ResultRequestModel(200, "Ok. Inserted.");

            try
            {
                if (_yboEntity.Add<Brand>(brand) < 1) result = new ResultRequestModel(400, "ERROR: Sever failed to insert new data.");
            }catch(Exception e)
            {
                result.Status = 400;
                result.Message = "ERROR: Faild to insert";
                if (e.Message.Contains("Cannot insert duplicate key in object")) result.Message = "ERROR: The Brand name already exists.";
            }
            return result;
        }

        public ResultRequestModel Remove(int id)
        {
            ResultRequestModel result = new ResultRequestModel(200,"Ok. Removed.");
            Brand brand = _yboEntity.Find<Brand>(id);
            try
            {
                result = _yboEntity.Remove<Brand>(brand) >= 1 ? new ResultRequestModel(200, "Ok. Removed.") : new ResultRequestModel(400,"ERROR: Internal error, try later");
            }catch(Exception e)
            {
                result = new ResultRequestModel(400, "ERROR: Internat exception. Try later");
                if (e.Message.StartsWith(@"The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    result.Message = "ERROR: The brand is already included an one or more Equity's. It's no possible to remove the Brand.";
                }
            }
            return result;
        }

        public bool Update(Brand brand)
        {
            return _yboEntity.Update<Brand>(brand) > 1 ? true : false;
        }
    }
}
