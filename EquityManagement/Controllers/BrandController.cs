using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EquityManagement.Model;
using Microsoft.Extensions.Configuration;
using EquityManagement.Repository;
using EquityManagement.Model.RequestModel;

namespace EquityManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        readonly IConfiguration _config;
        readonly IBrandRepository _brandRepository = null;
        readonly ITokenRepository _tokenRepository = null;

        public BrandController(IConfiguration config)
        {
            _config = config;
            _brandRepository = new BrandRepository(new YboEntityFramework(_config.GetConnectionString("ibope_test")));
            _tokenRepository = new TokenRepository(new YboEntityFramework(_config.GetConnectionString("ibope_test")));
        }

        // POST api/brand
        [HttpPost]
        public ActionResult Post([FromHeader] string token, [FromBody]BrandRequestModel brandRM)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            resultRequest = _brandRepository.InsertNew(new Brand { Name = brandRM.Name});            
            return new ObjectResult(resultRequest);
        }

        // GET api/brand/2
        [HttpGet("{id}")]
        public ActionResult<Brand> Get([FromHeader] string token, int id)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            try
            {
                Brand brand = _brandRepository.Find(id);
                if (brand.Name is null) return new ObjectResult(new ResultRequestModel(200, "Ok. Does not exist Brand with this id."));
                return brand;
            }catch(Exception e)
            {
                return new ObjectResult(new ResultRequestModel(400, "ERROR: Server was unable to get the data."));
            }                       
        }

        // GET api/7/equities        
        [HttpGet("{id}/equities")]
        public ActionResult<IEnumerable<Equity>> GetEquitiesByIdBrand([FromHeader] string token, int id)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            try
            {
                List<Equity> equities = _brandRepository.GetEquities(id);
                if (equities.Count < 1)
                    return new ObjectResult(new ResultRequestModel(200, "Ok. There are no Equities for this brand id."));
                equities.Sort((a, b) => { return a.ID.CompareTo(b.ID); });
                return equities;
            }catch(Exception e)
            {
                return new ObjectResult(new ResultRequestModel(400, "ERROR: Server failed to respond, try later."));
            }           
        }

        [HttpGet]
        public ActionResult<IEnumerable<Brand>> Get([FromHeader]string token)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            try
            {
                List<Brand> brands = _brandRepository.GetAll();
                if (brands.Count < 1)
                    return new ObjectResult(new ResultRequestModel(200, "Ok. There are no Brands."));

                brands.Sort((a, b) => {
                    return a.ID.CompareTo(b.ID);
                });
                return brands;
            }
            catch (Exception e)
            {
                return new ObjectResult(new ResultRequestModel(400, "ERROR: Server failed to respond, try later."));
            }
        }

        // PUT api/brand/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]BrandRequestModel brandRM, [FromHeader] string token)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            _brandRepository.Update(new Brand() { ID = id, Name = brandRM.Name});

            resultRequest = new ResultRequestModel(200, "The Brand has modified.");
            return new OkObjectResult(resultRequest);
        }


        // DELETE api/brand/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id, [FromHeader] string token)
        {
            ResultRequestModel resultRequest = _tokenRepository.CheckToken(token);
            if (resultRequest != null) return new ObjectResult(resultRequest);

            resultRequest = _brandRepository.Remove(id);            
            return new OkObjectResult(resultRequest);
        }

    }
}