using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement.Model;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Repository
{
    public class EquityRepository : IEquityRepository
    {
        readonly YboEntityFramework _yboEntity = null;

        public EquityRepository(YboEntityFramework yboEntityFramework)
        {
            _yboEntity = yboEntityFramework;
        }

        public Equity Find(int id)
        {
            return _yboEntity.Find<Equity>(id);            
        }

        public List<Equity> GetAll()
        {
            return _yboEntity.All<Equity>();            
        }

        public ResultRequestModel InsertNew(Equity equity)
        {https://github.com/YBHOLTZ/EquityManagement/pulls
            ResultRequestModel result = new ResultRequestModel(200, "Ok. Company Equity has Inserted");
            try
            {
                equity.Register = generateNewRegister();
                if(_yboEntity.Add<Equity>(equity) < 1) result = new ResultRequestModel(400, "ERROR: server fails to insert new Company Equity. Try later.");
            }
            catch(Exception e)
            {
                result.Status = 400;
                result.Message = "ERROR: Internal exception. Try later";
                if (e.Message.StartsWith("The INSERT statement conflicted with the FOREIGN KEY constraint"))
                {
                    result.Message = "ERROR: Invalid Brand ID.";
                }                
            }
            return result;
        }

        public int Remove(int id)
        {
            Equity equity = _yboEntity.Find<Equity>(id);
            if (equity is null) return 0;
            return _yboEntity.Remove<Equity>(equity);
        }

        public ResultRequestModel Update(Equity equity)
        {
            ResultRequestModel result = new ResultRequestModel(200, "Ok. Updated.");
            try
            {
                if (_yboEntity.Update<Equity>(equity) <= 0) result = new ResultRequestModel(400, "ERROR: Server fails to update data. Try again later.");
            }
            catch(Exception e)
            {
                result.Status = 400;
                result.Message = "ERROR: Internal exception in update data. Try Later";
                if (e.Message.StartsWith("The UPDATE statement conflicted with the FOREIGN KEY constraint")) result.Message = "ERROR: Invalid BrandID. Insert a valid BrandID.";
            }            
            return result;
        }

        int generateNewRegister()
        {
            DateTime dt = DateTime.Now;
            Random rand = new Random();
            int randNumber = rand.Next(0, 500);
            int f = dt.Day + dt.Month + dt.Minute + dt.Second + dt.AddMilliseconds(randNumber).Millisecond + dt.Year;
            return f;
        }
    }
}
