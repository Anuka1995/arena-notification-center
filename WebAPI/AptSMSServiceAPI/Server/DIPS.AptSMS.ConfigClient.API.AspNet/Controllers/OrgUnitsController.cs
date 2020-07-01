using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using DIPS.AptSMS.ConfigClient.API.Interface;
using Microsoft.AspNetCore.Authorization;
using DIPS.AptSMS.ConfigClient.Common.Models;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("orgunit/")]
    [ApiController]
    public class OrgUnitsController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(OrgUnitsController));
        private readonly IUser m_user;
        private readonly IOrgUnitsService m_orgUnitsService;
        public OrgUnitsController(IUser user, IOrgUnitsService orgUnitsService)
        {
            m_user = user;
            m_orgUnitsService = orgUnitsService;
        }

        [HttpGet, Route("departmentList")]
        public ActionResult GetDepartmentsForHospital()
        {
            try
            {
                m_log.Info($"orgunit/departmentList endpoint called with hospital Id - {m_user.HospitalId}");
                var deparmentResponse = m_orgUnitsService.GetDepartmentListByHospitalId(m_user.HospitalId, m_user.SecurityToken);
                return Ok(deparmentResponse);
            }
            catch (Exception e)
            {
                var ErrorMessage = " Exception thrown at GetDepartmentsForHospital for the hospital Id =" + m_user.HospitalId;
                m_log.ErrorException(ErrorMessage, e);
                throw;
            }
        }

        [HttpGet, Route("department/opd")]
        public ActionResult GetOPDsByDepartmentId(long departmentId)
        {
            try
            {
                m_log.Info($"/department/opd endpoint called with department id - {departmentId}");
                var opdResponse = m_orgUnitsService.GetOPDListByDepartmentId(departmentId, m_user.SecurityToken);
                return Ok(opdResponse);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at GetOpdsByDepartmentId ", e);
                throw;
            }
        }

        [HttpGet, Route("opdList")]
        public ActionResult GetOPDsByHospitalId()
        {
            try
            {
                m_log.Info($"orgunit/opdList endpoint called with hospital id - {m_user.HospitalId}");
                var opdResponse = m_orgUnitsService.GetOPDListByHospitalId(m_user.HospitalId, m_user.SecurityToken);
                return Ok(opdResponse);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at GetOPDsByHospitalId ", e);
                throw;
            }
        }

        [HttpGet, Route("department/section")]
        public ActionResult GetSectionsByDepartmentId(long departmentId)
        {
            try
            {
                m_log.Info($"/department/section endpoint called with department id - {departmentId}");
                var sectionResponse = m_orgUnitsService.GetSectionListByDepartmentId(departmentId, m_user.SecurityToken);
                return Ok(sectionResponse);
            }
            catch (Exception e)
            {
                var errorMessage = "Exception thrown at GetSectionsByDepartmentId  for the hospital Id =" + m_user.HospitalId;
                m_log.ErrorException(errorMessage, e);
                throw;
            }
        }

        [HttpGet, Route("section/ward")]
        public ActionResult GetWardsBySection(long sectionId)
        {
            try
            {
                m_log.Info($"/section/ward endpoint called with section id - {sectionId}");
                var wardnResponse = m_orgUnitsService.GetWardListBySectionId(sectionId, m_user.SecurityToken);
                return Ok(wardnResponse);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at GetWardsBySection ", e);
                throw;
            }
        }

        [HttpGet, Route("department/ward")]
        public ActionResult GetWardsByDepartmentId(long departmentId)
        {
            try
            {
                m_log.Info($"/department/ward endpoint called with input department id - {departmentId}");
                var wardnResponse = m_orgUnitsService.GetWardListByDepartmentId(departmentId, m_user.SecurityToken);
                return Ok(wardnResponse);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at GetWardsByDepartmentId ", e);
                throw;
            }
        }

        [HttpGet, Route("department/location")]
        public ActionResult GetLocationListByDepartmentId(long departmentId)
        {
            try
            {
                m_log.Info($"/department/location endpoint called with input department id - {departmentId}");
                var wardnResponse = m_orgUnitsService.GetLocationListByDepartmentId(departmentId, m_user.SecurityToken);
                return Ok(wardnResponse);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at GetLocationListByDepartmentId ", e);
                throw;
            }
        }

        [HttpGet, Route("department/fullTree")]
        public IActionResult GetOrganizationUnitsByDepartmentId(long departmentId)
        {
            try
            {
                m_log.Info($"orgunit/department/fullTree endpoint called with input department id - {departmentId}");
                var department = m_orgUnitsService.GetOrgUnitStructureForDepartment(m_user.HospitalId,departmentId, m_user.SecurityToken);
                return Ok(department);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception throw at GetOrganizationUnitsByDepartmentId ", e);
                throw;
            }
        }

        [HttpGet, Route("hospital/fullTree")]
        public IActionResult GetFullOrgUnitsByHospitalId()
        {
            try
            {
                m_log.Info($"orgunit/hospital/fullTree endpoint called ");
                var department = m_orgUnitsService.GetFullOrgUnitStructureTreeNodeList(m_user.HospitalId, m_user.SecurityToken);
                return Ok(department);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception throw at GetFullOrgUnitsByHospitalId ", e);
                throw;
            }
        }
    }
}
