using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("dateFormatConfig/")]
    [ApiController]
    public class DateFormatConfigController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(DateFormatConfigController));
        private readonly IDateTimeFormatService m_dateTimeFormatService;
        private readonly IUser m_user;

        public DateFormatConfigController(IDateTimeFormatService dateTimeFormatService, IUser user)
        {
            m_dateTimeFormatService = dateTimeFormatService;
            m_user = user;
        }

        [HttpPost, Route("update")]
        public ActionResult SaveFormat([FromBody]DateTimeFormat dateTimeFormat)
        {
            m_dateTimeFormatService.SaveDateTimeFormat(dateTimeFormat);
            
            return Ok();
        }

        [HttpGet, Route("get/")]
        public ActionResult GetAllFormats()
        {

            var dateFormats = m_dateTimeFormatService.GetDateTimeFormatByHospital(m_user.HospitalId);

            return Ok(dateFormats);
        }

        [HttpGet, Route("getPreview")]
        public ActionResult GetPreview([FromQuery]string format, [FromQuery]string dateSample)
        {
            var date = DateTime.Parse(dateSample);
            
            var dayStr = date.ToString(format);

            return Ok(dayStr);
        }

        [HttpGet, Route("getActive")]
        public ActionResult GetActiveFormats()
        {
            var activeDateFormats = m_dateTimeFormatService.GetActiveDateTimeFormatByHospital(m_user.HospitalId);

            return Ok(activeDateFormats);
        }
    }
}
