﻿using Domain.InventoryDomain;
using DomainServices;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ClientFacingWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        InventoryService _svc;

        public ValuesController()
        {
            _svc = new InventoryService(true);
        }

        // GET api/values
        public string Get()
        {
            return "Get with no ID";
        }

        // GET api/values/5
        public StorageFacility Get(string id)
        {
            return _svc.GetStorageFacility(id);
        }

        // POST api/values
        public void Post(InventoryDomainEventCheckoutItems value)
        {
            _svc.HandlePost(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}