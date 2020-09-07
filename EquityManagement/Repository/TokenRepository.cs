using EquityManagement.Model;
using EquityManagement.Model.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityManagement.Repository
{
    public class TokenRepository : ITokenRepository
    {
        readonly YboEntityFramework _yboEntity = null;
        public TokenRepository(YboEntityFramework yboEntityFramework)
        {
            _yboEntity = yboEntityFramework;
        }

        public ResultRequestModel CheckToken(string token)
        {
            ResultRequestModel resultRequest = null;
            if (!IsValid(token))
                resultRequest = new ResultRequestModel(400, "Invalid token");
            return resultRequest;
        }

        public bool IsValid(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            List<int> results = _yboEntity.FreeQuery<int>("SELECT ID FROM SEC_TOKEN WHERE TOKEN = @P0", token);          
            return results.Count > 0? true : false;
        }
    }
}
