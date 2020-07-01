using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [ApiController]
    public class DIPSContactTypeController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(DIPSContactTypeController));

        private IDIPSContactTypeService m_DIPSContactTypeService;

        public DIPSContactTypeController(IDIPSContactTypeService DIPSContactTypeService, IUser user)
        {
            m_DIPSContactTypeService = DIPSContactTypeService;
        }

        [HttpGet, Route("officialLevelOfCare/get")]
        public ActionResult GetOfficialLevelOfCareDetails()
        {
            m_log.Info("executing get official level of care infomation");
            try
            {
                var officialLevelOfCareResultsSet = m_DIPSContactTypeService.GetOfficialLevelOfCareInfo();
                return Ok(officialLevelOfCareResultsSet);
            }
            catch (Exception ex)
            {
                m_log.ErrorException("Error in Getting Official Level of Care", ex);
                throw;
            }

            
        }

        [HttpGet, Route("contactType/get")]
        public ActionResult GetContactTypeDetails()
        {
            m_log.Info("executing get contact type infomation");
            try
            {
                var contactTypeResultsSet = m_DIPSContactTypeService.GetContactTypeInfo();

                return Ok(contactTypeResultsSet);
            }
            catch (Exception ex)
            {
                m_log.ErrorException("Error in Getting Contact Type Details", ex);
                throw;
            }
        }
    }
}
