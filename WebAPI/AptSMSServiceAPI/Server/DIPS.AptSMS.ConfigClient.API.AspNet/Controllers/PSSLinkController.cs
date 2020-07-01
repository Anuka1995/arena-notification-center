using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("PSSLinkPage/")]
    [ApiController]
    public class PSSLinkController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(PSSLinkController));
        private readonly IPSSLinkService m_PSSLinkService;
        private readonly IUser m_user;

        public PSSLinkController(IPSSLinkService configPSSLinkService, IUser user)
        {
            m_PSSLinkService = configPSSLinkService;
            m_user = user;
        }

        [HttpPost, Route("save")]
        public ActionResult SaveChangedPSS([FromBody]PSSLinkModel pssLink)
        {
            pssLink.HospitalId = m_user.HospitalId;
            var guid = m_PSSLinkService.CreateUpdatePSSLink(pssLink);
            return Ok(guid);
        }

        [HttpGet, Route("get")]
        public ActionResult GetPSSLink()
        {
            var hospitalURL = m_PSSLinkService.GetPSSLinkByHospital(m_user.HospitalId);

            return Ok(hospitalURL.First());
        }
    }
}
