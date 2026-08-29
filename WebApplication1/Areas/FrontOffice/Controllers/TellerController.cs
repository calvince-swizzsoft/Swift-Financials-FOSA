using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [RoutePrefix("api/frontoffice/tellers")]
    public class TellerController : ApiController
    {


        private readonly ITellerAppService _tellerAppService;

        public TellerController (ITellerAppService tellerAppService)
        {

            _tellerAppService = tellerAppService;
            
        }

        [HttpGet]

        [Route("")]
        public async Task<IHttpActionResult> Index(int tellerType = 0, string text = "", int pageIndex = 0, int pageSize = 20)
        {
            try

            {
                var serviceHeader = Utils.CreateServiceHeader();

                var tellers = _tellerAppService.FindTellers(tellerType, text ?? "", pageIndex, pageSize, serviceHeader);


                if (tellers == null)
                {

                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = tellers });

            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]

        [Route("")]
        public async Task<IHttpActionResult> Create(TellerDTO tellerDTO)
        {
            try
            {

                var serviceHeader = Utils.CreateServiceHeader();

                UpdateTellerAccounts(tellerDTO);
                tellerDTO.ValidateAll();

                if (!tellerDTO.HasErrors)
                {

                    var createdTellerDTO = _tellerAppService.AddNewTeller(tellerDTO, serviceHeader);

                    return Ok(new { success = true, message = "Operation Success", data = createdTellerDTO });
                }

                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", tellerDTO.ErrorMessages), data = (object)null });
                }

            }

            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> UpdateTeller(Guid id, TellerDTO tellerDTO)
        {
            try
            {

                var serviceHeader = Utils.CreateServiceHeader();

                tellerDTO.Id = id;
                UpdateTellerAccounts(tellerDTO);
                tellerDTO.ValidateAll();

                if (!tellerDTO.HasErrors)
                {
                    // UpdateTeller returns bool, not the updated entity — re-fetch
                    // so `data` reflects what was actually saved.
                    var updated = _tellerAppService.UpdateTeller(tellerDTO, serviceHeader);

                    if (!updated)
                    {
                        return NotFound();
                    }

                    var refreshedTellerDTO = _tellerAppService.FindTeller(id, serviceHeader);

                    return Ok(new { success = true, message = "Operation Success", data = refreshedTellerDTO });
                }

                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, message = string.Join("; ", tellerDTO.ErrorMessages), data = (object)null });
                }
            }

            catch (InvalidOperationException exception)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, message = exception.Message, data = (object)null });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetTeller(Guid id)
        {
            try
            {
                var serviceHeader = Utils.CreateServiceHeader();

                var teller = _tellerAppService.FindTeller(id, serviceHeader);

                if (teller == null)
                {
                    return NotFound();
                }

                return Ok(new { success = true, message = "", data = teller });
            }

            catch (Exception)
            {
                throw;
            }
        }

        private void UpdateTellerAccounts(TellerDTO tellerDTO)
        {
            switch ((TellerType)tellerDTO.Type)
            {
                case TellerType.InhousePointOfSale:
                case TellerType.AutomatedTellerMachine:
                    tellerDTO.ShortageChartOfAccountId = tellerDTO.ChartOfAccountId;
                    break;

                case TellerType.AgentPointOfSale:
                    tellerDTO.ChartOfAccountId = tellerDTO.CommissionCustomerAccountCustomerAccountTypeTargetProductId;
                    tellerDTO.ShortageChartOfAccountId = tellerDTO.CommissionCustomerAccountCustomerAccountTypeTargetProductId;
                    break;
            }
        }

        [HttpGet]
        [Route("teller")]
        public async Task<IHttpActionResult> GetTellerByEmployeeId(Guid employeeId)
        {

            bool includeBalance = default(bool);

            try
            {

                var serviceHeader = Utils.CreateServiceHeader();
                includeBalance = true;

                var teller = _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader);
                return Ok(new { success = true, message = "", data = teller });

            }


            catch (Exception)
            {
                throw;
            }
        }




    }
}
