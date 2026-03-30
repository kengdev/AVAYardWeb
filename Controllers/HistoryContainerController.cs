using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Transactions;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class HistoryContainerController : Controller
    {
        private string connStr;
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;

        public HistoryContainerController(DbavayardContext context)
        {
            db = context;
            connStr = db.Database.GetConnectionString();
        }

        public async Task<IActionResult> Drop()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in await serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            return View();
        }

        public async Task<IActionResult> Match()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in await serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            return View();
        }

        public async Task<IActionResult> GetDropContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            List<OrderHistoryModel> _orderData = new List<OrderHistoryModel>();
            using (var connection = new SqlConnection(connStr))
            {
                await connection.OpenAsync();

                var sql = @"
                            SELECT 
                                order_container.order_code,
                                order_container.issue_type, 
                                order_container.container_status, 
                                trans_agent.agent_name, 
                                order_container.container_no, 
                                trans_container_size.container_size_name As container_size, 
                                trans_container_size.container_size_code As container_size_code, 
                                order_container.booking_no, 
                                order_container.truck_license + ' | ' + trans_transportation.transportation_acronym As return_item, 
                                order_payment.truck_license + ' | ' + trans_transportation_1.transportation_acronym AS pickup_item , 
                                FORMAT(issue_date, 'yyyy-MM-dd') As return_date, 
                                FORMAT(order_payment.create_date, 'yyyy-MM-dd') As pickup_date 
                            FROM order_container 
                            INNER JOIN order_payment 
                                ON order_container.order_code = order_payment.order_code 
                            INNER JOIN trans_agent 
                                ON order_container.agent_code = trans_agent.agent_code 
                            INNER JOIN trans_transportation 
                                ON order_container.transportation_code = trans_transportation.transportation_code 
                            INNER JOIN trans_container_size 
                                ON order_container.container_size_code = trans_container_size.container_size_code 
                            INNER JOIN trans_transportation AS trans_transportation_1 
                                ON order_payment.transportation_code = trans_transportation_1.transportation_code 
                            WHERE 
                                order_container.issue_type = 'DROP'
                                AND order_payment.is_paid = 1
                                AND order_payment.create_date >= @start 
                                AND order_payment.create_date <= @end
                        ";
                var filter = new
                {
                    start = iFilter.filterDateRangeStart,
                    end = iFilter.filterDateRangeEnd
                };

                _orderData = (await connection.QueryAsync<OrderHistoryModel>(sql, filter)).ToList();
            }


            var data = _orderData.Where(w =>
                (iFilter.filterContainerNo == null || w.container_no.ToUpper().Contains(iFilter.filterContainerNo.ToUpper())) &&
                (iFilter.filterContainerSize == null || w.container_size_code.ToUpper().Contains(iFilter.filterContainerSize.ToUpper())) &&
                (iFilter.filterLicense == null || w.return_item.ToUpper().Contains(iFilter.filterLicense.ToUpper())) &&
                (iFilter.filterName == null || w.pickup_item.Contains(iFilter.filterName.ToUpper()))
            );


            Func<OrderHistoryModel, string> orderingFunction = (c =>
                param.iSortCol_0 == 0 ? c.order_code :
                param.iSortCol_0 == 1 ? c.container_no :
                param.iSortCol_0 == 2 ? c.container_size :
                param.iSortCol_0 == 3 ? c.return_date :
                param.iSortCol_0 == 4 ? c.return_item :
                param.iSortCol_0 == 5 ? c.pickup_date :
                param.iSortCol_0 == 6 ? c.pickup_item : c.container_no);

            IEnumerable<OrderHistoryModel> listQuery;
            if (param.sSortDir_0 == "asc")
            {
                listQuery = data.OrderBy(orderingFunction)
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);
            }
            else
            {
                listQuery = data.OrderByDescending(orderingFunction)
                                .Skip(param.iDisplayStart)
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

        public async Task<IActionResult> GetMatchContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            var _orderData = await (from a in db.OrderContainers
                                    join c in db.TransAgents on a.AgentCode equals c.AgentCode
                                    join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                                    join f in db.TransContainerSizes on a.ContainerSizeCode equals f.ContainerSizeCode
                                    join g in db.OrderContainerMatchdetails on a.OrderCode equals g.OrderCode
                                    where a.IsReceipt == true &&
                                            a.IsApprove == true &&
                                            a.IssueDate >= iFilter.filterDateRangeStart &&
                                            a.IssueDate <= iFilter.filterDateRangeEnd
                                    select new OrderContainerModel
                                    {
                                        order_code = a.OrderCode,
                                        issue_type = a.IssueType,
                                        container_no = a.ContainerNo,
                                        container_size = f.ContainerSizeName,
                                        container_size_code = f.ContainerSizeCode,
                                        truck_license = a.TruckLicense,
                                        transportation_name = d.TransportationName,
                                        match_type = g.MatchType,
                                        agent_name = c.AgentName,
                                        is_exchange = a.IsExchange,
                                        issue_date_str = a.IssueDate.ToString("yyyy-MM-dd")
                                    }).ToListAsync();

            var data = _orderData.Where(w =>
                (iFilter.filterCode == null || w.order_code.ToUpper().Contains(iFilter.filterCode.ToUpper())) &&
                (iFilter.filterContainerNo == null || w.container_no.ToUpper().Contains(iFilter.filterContainerNo.ToUpper())) &&
                (iFilter.filterContainerSize == null || w.container_size_code.ToUpper().Contains(iFilter.filterContainerSize.ToUpper())) &&
                (iFilter.filterExchangeType == null || w.is_exchange == iFilter.filterExchangeType) &&
                (iFilter.filterAgent == null || w.agent_name.ToUpper().Contains(iFilter.filterAgent.ToUpper())) &&
                (iFilter.filterLicense == null || w.truck_license.ToUpper().Contains(iFilter.filterLicense.ToUpper())) &&
                (iFilter.filterName == null || w.transportation_name.ToUpper().Contains(iFilter.filterName.ToUpper())) &&
                (iFilter.filterMatchType == null || w.match_type.Contains(iFilter.filterCustomer.ToUpper()))
            );

            Func<OrderContainerModel, string> orderingFunction = (c =>
                param.iSortCol_0 == 0 ? c.issue_date_str :
                param.iSortCol_0 == 1 ? c.order_code :
                param.iSortCol_0 == 2 ? c.container_no :
                param.iSortCol_0 == 3 ? c.container_size :
                param.iSortCol_0 == 4 ? c.is_exchange.ToString() :
                param.iSortCol_0 == 5 ? c.agent_name :
                param.iSortCol_0 == 6 ? c.truck_license :
                param.iSortCol_0 == 7 ? c.transportation_name :
                param.iSortCol_0 == 8 ? c.match_type : c.container_no);

            IEnumerable<OrderContainerModel> listQuery;
            if (param.sSortDir_0 == "asc")
            {
                listQuery = data.OrderBy(orderingFunction)
                                .Skip(param.iDisplayStart)
                                .Take(param.iDisplayLength);
            }
            else
            {
                listQuery = data.OrderByDescending(orderingFunction)
                                .Skip(param.iDisplayStart)
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
