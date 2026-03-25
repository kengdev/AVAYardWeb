using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Security.Claims;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class YardReceiptController : Controller
    {
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;

        public YardReceiptController(DbavayardContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var dropdown = new DropListRepository(db);
            string month = DateTime.Now.ToString("MM");
            string year = DateTime.Now.ToString("yyyy");
            ViewData["month"] = from a in dropdown.GetMonth() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == month };
            ViewData["year"] = from a in dropdown.GetYear() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == year };

            return View();
        }

        public async Task<IActionResult> Edit(string code)
        {
            var serviceReceipt = new YardReceiptRepository(db);
            var receiptData = await serviceReceipt.GetOrderReceiptByCode(code);

            return View(receiptData);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditItem(string code, string item, decimal decval, string strval)
        {
            var response = new ResponseViewModel();
            try
            {
                var receiptData = await db.OrderReceipts.FirstOrDefaultAsync(w => w.ReceiptCode == code);
                if (item == "total")
                {
                    receiptData.Total = decval;
                    receiptData.Vat = (receiptData.Total * 7) / 100;
                    receiptData.NetTotal = receiptData.Total + receiptData.Vat;
                }
                else if (item == "container")
                {
                    receiptData.ContainerNo = strval;
                }

                await db.SaveChangesAsync();
                response.result = true;
                response.vat = receiptData.Vat.ToString("###0.00");
                response.net_total = receiptData.NetTotal.ToString("###0.00");
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                await db.DisposeAsync();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }

            return Json(response);
        }

        public IActionResult IsAdmin()
        {
            var group = this.GetGroup();
            if (group == "Admin")
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public async Task<IActionResult> EditData(OrderReceipt model)
        {
            var serviceReceipt = new YardReceiptRepository(db);
            var response = await serviceReceipt.Edit(model);

            return Json(response);
        }

        public IActionResult GetReceiptHandlingDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            var _service = new YardReceiptRepository(db);
            var query = _service.GetContainerHandlingDataList(iFilter.month, iFilter.year);
            var data = query.Where(w => (iFilter.filterName == null || w.tax_name.ToUpper().Contains(iFilter.filterName.ToUpper())) &&
                                        (iFilter.filterCode == null || w.code.ToUpper().Contains(iFilter.filterCode.ToUpper())) &&
                                        (iFilter.filterContainerNo == null || w.container_no.Contains(iFilter.filterContainerNo.ToUpper())));

            Func<ContainerHandlingViewModel, string> orderingFunction = (c =>
                param.iSortCol_0 == 2 ? c.receipt_date :
                param.iSortCol_0 == 3 ? c.code :
                param.iSortCol_0 == 4 ? c.container_no :
                param.iSortCol_0 == 5 ? c.container_size :
                param.iSortCol_0 == 6 ? c.tax_name : c.code);

            IEnumerable<ContainerHandlingViewModel> listQuery;
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

        private string GetLoggedCode()
        {
            var data = User.Claims.Where(w => w.Type == "code").FirstOrDefault();
            return data.Value;
        }

        private string GetGroup()
        {
            var role = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .FirstOrDefault();

            return role;
        }
    }
}
