using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{

    [RoutePrefix("api/administration/locations")]
    public class LocationController : ApiController
    {

        public readonly ILocationAppService _locationAppService;

        public LocationController(ILocationAppService locationAppService)
        {

            _locationAppService = locationAppService;   

        }


        [HttpGet, Route("")]
        public IHttpActionResult Index()
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();


            try
            {
              var locations =  _locationAppService.FindLocations(serviceHeader);

                return Ok(locations);

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);

            }
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(LocationDTO locationDto)
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();


            try
            {
                var location = _locationAppService.AddNewLocation(locationDto, serviceHeader);
                return Ok(location);

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);

            }
        }


        [HttpPut, Route("")]
        public IHttpActionResult Update(LocationDTO locationDto)
        {

            var serviceHeader = WebApplication1.Helpers.Utils.CreateServiceHeader();


            try
            {
                var location = _locationAppService.UpdateLocation(locationDto, serviceHeader);
                return Ok(location);

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);

            }
        }

       


    }
}