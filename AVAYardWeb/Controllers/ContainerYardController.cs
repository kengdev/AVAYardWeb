using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Transactions;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class ContainerYardController : Controller
    {
        private string connStr;
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;
        private const string KSHOP_TEMPLATE = "0002010102110216478772000245472004155303920002454991531343007640052044640122067749700130810016A00000067701011201150107536000315080214KB0000018904640320KPS004KB00000189046431690016A00000067701011301030040214KB0000018904640420KPS004KB00000189046451430014A0000000041010010641697102111234567890152045631530376454041.005802TH5910NOKKY SHOP6004CITY62240508854304870708420677496304E809";


        public ContainerYardController(DbavayardContext context)
        {
            db = context;
            connStr = db.Database.GetConnectionString();
        }

        public IActionResult Drop()
        {
            return View();
        }

        public IActionResult Match()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            return View();
        }

        public async Task<IActionResult> ChangeStage(string order_code)
        {
            var response = new ResponseViewModel();
            try
            {
                var stageData = db.OrderContainerRepairs.FirstOrDefault(w => w.OrderCode == order_code);
                stageData.RepairStatusCode = "D";

                await db.SaveChangesAsync();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public async Task<IActionResult> GetDropContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            List<OrderContainerModel> _orderData = new List<OrderContainerModel>();
            using (var connection = new SqlConnection(connStr))
            {
                await connection.OpenAsync();

                var sql = "SELECT order_container.order_code, order_container.container_no, " +
                "trans_container_size.container_size_name As container_type, order_container.truck_license, " +
                "trans_transportation.transportation_name, " +
                "order_container.issue_date, issue_type, trans_agent.agent_name, is_receipt, " +
                "DATEDIFF(DAY, CAST([issue_date] AS DATE), CAST(GETDATE() AS DATE)) AS days_ago, " +
                "FORMAT(issue_date, 'dd-MM-yyyy') As issue_date_str " +
                "FROM order_container INNER JOIN " +
                "trans_container_size ON order_container.container_size_code = trans_container_size.container_size_code " +
                "INNER JOIN trans_agent ON order_container.agent_code = trans_agent.agent_code " +
                "INNER JOIN trans_transportation ON order_container.transportation_code = trans_transportation.transportation_code " +
                "INNER JOIN order_container_location ON order_container.order_code = order_container_location.order_code " +
                "WHERE(order_container.is_enabled = 'TRUE') AND(order_container.container_status = 'AC') AND " +
                "(order_container.issue_type = 'DROP') AND (order_container.is_receipt = 'FALSE') " +
                "order by order_container.order_code ASC";

                _orderData = (await connection.QueryAsync<OrderContainerModel>(sql)).ToList();
            }

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

        public async Task<IActionResult> GetMatchContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            List<OrderContainerModel> _orderData = new List<OrderContainerModel>();
            using (var connection = new SqlConnection(connStr))
            {
                await connection.OpenAsync();

                var sql = "SELECT order_container.order_code, order_container.container_no, " +
                "trans_container_size.container_size_name As container_type, order_container.truck_license, " +
                "trans_agent.agent_name, order_container.is_exchange, order_container.container_size_code, " +
                "DATEDIFF(DAY, CAST([issue_date] AS DATE), CAST(GETDATE() AS DATE)) AS days_ago, " +
                "DATEDIFF(DAY, CAST(GETDATE() AS DATE), CAST([detention_date] AS DATE)) AS aging, " +
                "FORMAT(issue_date, 'dd-MM-yyyy') As issue_date_str, container_status, " +
                "FORMAT(detention_date, 'dd-MM-yyyy') As detention_date,payment_stage_code As payment_stage, " +
                "order_container_repair.repair_status_code As repair_stage " +
                "FROM order_container INNER JOIN " +
                "trans_container_size ON order_container.container_size_code = trans_container_size.container_size_code " +
                "INNER JOIN trans_agent ON order_container.agent_code = trans_agent.agent_code " +
                "INNER JOIN order_container_location ON order_container.order_code = order_container_location.order_code " +
                "INNER JOIN order_container_matchdetail ON order_container.order_code = order_container_matchdetail.order_code " +
                "INNER JOIN order_container_repair ON order_container.order_code = order_container_repair.order_code " +
                "WHERE (order_container.is_enabled = 'TRUE') " +
                "AND (order_container.container_status = 'AC') " +
                "AND (order_container.issue_type = 'MATCH') " +
                "AND (order_container_location.location_status = 'IN') " +
                "ORDER BY order_container.order_code";

                _orderData = (await connection.QueryAsync<OrderContainerModel>(sql)).ToList();
            }

            var data = _orderData.Where(w => (iFilter.filterContainerNo == null || w.container_no.ToUpper().Contains(iFilter.filterContainerNo.ToUpper())) &&
            (iFilter.filterContainerSize == null || w.container_size_code.ToUpper().Contains(iFilter.filterContainerSize.ToUpper())) &&
            (iFilter.filterMatchType == null || w.is_exchange == iFilter.filterMatchType) &&
            (iFilter.filterDate == null || w.issue_date_str.ToUpper().Contains(iFilter.filterDate.ToUpper())) &&
            (iFilter.filterDetention == null || w.detention_date.ToUpper().Contains(iFilter.filterDetention.ToUpper())) &&
            (iFilter.filterName == null || w.agent_name.ToUpper().Contains(iFilter.filterName.ToUpper())) &&
            (iFilter.filterStatusRepair == null || w.repair_stage.Contains(iFilter.filterStatusRepair.ToUpper())));

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
