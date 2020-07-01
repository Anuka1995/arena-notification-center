using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IWcfConfigurationProvider
    {
        string FullOrganizationServiceUrl { get; }
        string FullHospitalServiceUrl { get; }
        string FullDepartmentServiceUrl { get; }
        string FullWardServiceUrl { get; }
        string FullSectionServiceUrl { get; }
        string FullLocationServiceUrl { get; }
    }
}
