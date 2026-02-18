using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class HistoryContainerController : Controller
    {
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;

        public HistoryContainerController(DbavayardContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            var _orderData = await (from a in db.OrderContainers
                              join c in db.TransAgents on a.AgentCode equals c.AgentCode
                              join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                              join f in db.TransContainerSizes on a.ContainerSizeCode equals f.ContainerSizeCode
                              where a.IsReceipt == true && a.IsApprove == true
                              select new OrderContainerModel
                              {
                                  order_code = a.OrderCode,
                                  issue_type = a.IssueType,
                                  container_no = a.ContainerNo,
                                  container_size = f.ContainerSizeName,
                                  truck_license = a.TruckLicense,
                                  transportation_name = d.TransportationName
                        
                              }).ToListAsync();
            var data = _orderData.Where(w => (iFilter.filterName == null || w.container_no.ToUpper().Contains(iFilter.filterName.ToUpper())) &&
                                (iFilter.filterCustomer == null || w.truck_license.Contains(iFilter.filterCustomer.ToUpper())));

            IEnumerable<OrderContainerModel> listQuery;
            if (param.sSortDir_0 == "asc")
            {
                listQuery = data.Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);
            }
            else
            {
                listQuery = data.Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = listQuery
            });
        }
    }
}
