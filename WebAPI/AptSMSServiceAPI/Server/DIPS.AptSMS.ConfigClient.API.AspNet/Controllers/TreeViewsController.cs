using System.Linq;
using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("treeviews/")]
    [ApiController]
    public class TreeViewsController : ControllerBase
    {
        private readonly IOverviewService m_overviewService;
        private readonly IUser m_user;
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(AccountController));

        public TreeViewsController(IOverviewService overviewService, IUser user)
        {
            m_overviewService = overviewService;
            m_user = user;
        }

        [HttpGet, Route("overview")]
        public ActionResult GetOverviewTreeData([FromQuery]bool getInactive)
        {
            m_log.Trace($"Generating overview for the hospital: {m_user.HospitalId}");

            var deparmentnodeList = m_overviewService.GenerateOverviewTree(getInactive, m_user.HospitalId, m_user.SecurityToken);

            var overviewTreeNodes = deparmentnodeList.Select(d => new TreeNode() { 
                Title = d.Name,
                Type = "department",
                ChildNodes = d.Schedules.Select(s => new TreeNode() { 
                    Title = s.Name,
                    Type = "schedule",
                    Tag = new {DaysCount = s.DaysCountBeforeApt, ExcludedIds = s.ExcludedOrgUnitNames, IsGlobal = s.IsGlobal },
                    Id = s.RuleSetId.ToString(),
                    ChildNodes = s.TextTemplates.Select(t => new TreeNode() { 
                        Title = t.Name,
                        Type = "template",
                        Id = t.TempalteId.ToString(),
                        Tag = new { 
                            TargetOrgUnit = t.TargetOrgUnitName, 
                            Location = t.LocationName,
                            t.IsSMSGenerate ,
                            TargetOpd = t.TargetOpdName
                        }
                    }).ToList()
                }).ToList(),
                Id = d.Id.ToString()
            }).ToList();

            m_log.Trace($"Overview tree generated. Nodes: ${overviewTreeNodes.Count()}");
            return Ok(overviewTreeNodes);
        }
    }
}