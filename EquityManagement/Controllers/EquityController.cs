using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquityManagement.Model;
using EquityManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquityController : ControllerBase
    {
        readonly IConfiguration _config;
        readonly IEquityRepository _equityRepository;
        readonly ITokenRepository _tokenRepository;

        public EquityController(IConfiguration config)
        {
            _config = config;
            _equityRepository = new EquityRepository(new YboEntityFramework(_config.GetConnectionString("ibope_test")));
            _tokenRepository = new TokenRepository(new YboEntityFramework(_config.GetConnectionString("ibope_test")));

        }

        // POST api/equity
        [HttpPost]
        public ActionResult Post([FromHeader]string token, [FromBody]EquityRequestModel equityRM)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            try
            {
                Equity equity = new Equity()
                {
                    Name = equityRM.Name,
                    BrandID = equityRM.BrandID,
                    Description = equityRM.Description
                };
                resultRequest = _equityRepository.InsertNew(equity);
            }catch(Exception e)
            {
                return new ObjectResult(new ResultRequestModel(400,"ERROR: Server erro. Try later."));
            }           
            return new OkObjectResult(resultRequest);
        }

        //GET api/equity
        [HttpGet]
        public ActionResult<IEnumerable<Equity>> Get([FromHeader]string token)
        {
            
            try
            {
                ResultRequestModel result = _tokenRepository.CheckToken(token);
                if (result != null) return new ObjectResult(result);
                List<Equity> eqs = _equityRepository.GetAll();
                return eqs;
            }
            catch
            {
                return new ObjectResult(new ResultRequestModel(400, "ERROR: Server was unable to get the data. Try later."));
            }
        }

        // GET api/equity/5
        [HttpGet("{id}")]
        public ActionResult<Equity> Get(int id,[FromHeader] string token)
        {
            ResultRequestModel result = _tokenRepository.CheckToken(token);
            if (result != null) return new ObjectResult(result);

            try
            {
                return _equityRepository.Find(id);
            }catch(Exception e)
            {
                result.Status = 400;
                result.Message = "ERROR: Server was unable to get the data. Try later.";
                if (e.Message.Equals("Primary key property not found")) result.Message = "ERROR: Primary key of entity it's not found";
                return new ObjectResult(result);
            }
        }

        // PUT api/equity/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]EquityRequestModel equityRM, [FromHeader] string token)
        {
            if (id <= 0) return new BadRequestResult();

            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            Equity equity = _equityRepository.Find(id);
            equity.Name = equityRM.Name;
            equity.Description = equityRM.Description;
            equity.BrandID = equityRM.BrandID;

            resultRequest = _equityRepository.Update(equity);            

            return new ObjectResult(resultRequest);
        }


        // DELETE api/equity/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id, [FromHeader] string token)
        {
            if (id <= 0) return new BadRequestResult();

            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            resultRequest = new ResultRequestModel(200, "Ok. Removed");
            if(_equityRepository.Remove(id) <= 0) resultRequest = new ResultRequestModel(200, "ERROR. The server can't find the ID.");
            return new ObjectResult(resultRequest);
        }
    }
}