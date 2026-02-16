using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Transactions;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class OrderContainerController : Controller
    {
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;

        public OrderContainerController(DbavayardContext context)
        {
            db = context;
        }

        public IActionResult Drop()
        {
            return View();
        }

        public IActionResult Match()
        {
            return View();
        }

        public IActionResult DropIssue()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label };

            return View();
        }

        public IActionResult IssueMatchPickup()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label };

            return View();
        }

        public IActionResult IssueMatchReturn()
        {
            var serviceDropDown = new DropListRepository(db);

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label };

            return View();
        }

        [HttpPost]
        public IActionResult AddDropData(OrderContainer model)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using (TransactionScope tr = new TransactionScope())
            {
                try
                {
                    model.OrderCode = serviceCode.GetOrderContainerCode();
                    model.IssueType = "DROP";
                    model.ContainerStatus = "PN";
                    model.ContainerNo = model.ContainerNo.ToUpper();
                    model.IssueDate = DateTime.Now;
                    model.AgentCode = "001";
                    model.SealNo = "-";
                    model.TareWeight = 0;
                    model.PaymentStageCode = "B";

                    model.IsReceipt = false;
                    model.IsEnabled = true;
                    model.CreateDate = DateTime.Now;
                    model.CreateBy = this.LoggedInUser;

                    OrderContainerMatchdetail matchDetail = new OrderContainerMatchdetail();
                    matchDetail.OrderCode = model.OrderCode;
                    matchDetail.DetentionDate = DateOnly.FromDateTime(DateTime.Now);
                    matchDetail.MatchType = "RETURN";
                    model.OrderContainerMatchdetail = matchDetail;

                    db.OrderContainers.Add(model);
                    db.SaveChanges();
                    tr.Complete();
                    response.result = true;
                    response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
                }
                catch (Exception ex)
                {
                    db.Dispose();
                    response.result = false;
                    response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                    response.errorException = ex;
                }
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddMatchData(OrderContainer model, OrderContainerMatchdetail matchDetail)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using var tr = await db.Database.BeginTransactionAsync();
            try
            {
                if (matchDetail.MatchType == "RETURN")
                {
                    model.OrderCode = serviceCode.GetOrderContainerCode();
                    model.IssueType = "MATCH";
                    model.ContainerStatus = "PN";
                    model.PaymentStageCode = model.IsExchange == true ? model.PaymentStageCode : "A";
                    model.ContainerNo = model.ContainerNo.ToUpper();
                    model.IssueDate = DateTime.Now;
                    model.SealNo = model.SealNo != null ? model.SealNo : "-";
                    model.TransportationCode = model.TransportationCode != null ? model.TransportationCode : "25000";
                    model.TareWeight = model.TareWeight != null ? model.TareWeight : 0;

                    model.IsReceipt = false;
                    model.IsEnabled = true;
                    model.CreateDate = DateTime.Now;
                    model.CreateBy = this.LoggedInUser;

                    matchDetail.OrderCode = model.OrderCode;
                    model.OrderContainerMatchdetail = matchDetail;
                }
                else
                {
                    var reuseData = await (from a in db.OrderContainers
                                           join b in db.OrderContainerLocations on a.OrderCode equals b.OrderCode
                                           where b.ContainerNo == model.ContainerNo
                                           select a).FirstOrDefaultAsync();

                    model.OrderCode = serviceCode.GetOrderContainerCode();
                    model.IssueType = "MATCH";
                    model.ContainerStatus = "PN";
                    model.ContainerNo = model.ContainerNo.ToUpper();
                    model.IssueDate = DateTime.Now;
                    model.SealNo = model.SealNo != null ? model.SealNo : "-";
                    model.TransportationCode = model.TransportationCode != null ? model.TransportationCode : "25000";
                    if (model.IsExchange)
                    {
                        model.PaymentStageCode = reuseData.PaymentStageCode == "A" ? "B" : "A";
                    }
                    else
                    {
                        model.PaymentStageCode = reuseData.PaymentStageCode = "A";
                    }

                    model.IsReceipt = false;
                    model.IsEnabled = true;
                    model.CreateDate = DateTime.Now;
                    model.CreateBy = this.LoggedInUser;

                    matchDetail.OrderCode = model.OrderCode;
                    matchDetail.DetentionDate = DateOnly.FromDateTime(DateTime.Now);
                    matchDetail.MatchType = "PICKUP";
                    model.OrderContainerMatchdetail = matchDetail;
                }

                db.OrderContainers.Add(model);
                await db.SaveChangesAsync();
                await tr.CommitAsync();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                await tr.RollbackAsync();
                await tr.DisposeAsync();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public async Task<IActionResult> EditMatchPickup(string code)
        {
            var serviceDropDown = new DropListRepository(db);
            var orderData = await db.OrderContainers.Where(w => w.OrderCode == code).Include(i => i.AgentCodeNavigation).Include(i => i.TransportationCodeNavigation).Include(i => i.ContainerSizeCodeNavigation).FirstOrDefaultAsync();
            orderData.OrderContainerMatchdetail = await db.OrderContainerMatchdetails.Where(w => w.OrderCode == code).FirstOrDefaultAsync();

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.ContainerSizeCode };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.AgentCode };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.TransportationCode };

            return View(orderData);
        }

        public async Task<IActionResult> EditMatchReturn(string code)
        {
            var serviceDropDown = new DropListRepository(db);
            var orderData = await db.OrderContainers.Where(w => w.OrderCode == code).Include(i => i.AgentCodeNavigation).Include(i => i.TransportationCodeNavigation).FirstOrDefaultAsync();
            orderData.OrderContainerMatchdetail = await db.OrderContainerMatchdetails.Where(w => w.OrderCode == code).FirstOrDefaultAsync();

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.ContainerSizeCode };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.AgentCode };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == orderData.TransportationCode };

            return View(orderData);
        }

        [HttpPost]
        public async Task<IActionResult> EditMatchData(OrderContainer model, OrderContainerMatchdetail matchDetail)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using var tr = await db.Database.BeginTransactionAsync();
            try
            {
                if (matchDetail.MatchType == "PICKUP")
                {
                    model.BookingNo = model.BookingNo.ToUpper();
                    model.SealNo = model.SealNo.ToUpper();
                }
                db.OrderContainers.Update(model);
                await db.SaveChangesAsync();
                await tr.CommitAsync();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                await tr.RollbackAsync();
                await tr.DisposeAsync();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public IActionResult DropEdit(string code)
        {
            var serviceDropDown = new DropListRepository(db);
            var model = db.OrderContainers.Where(w => w.OrderCode == code).FirstOrDefault();

            ViewData["ContainerSizeCode"] = from a in serviceDropDown.GetContainerSize() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == model.ContainerSizeCode };
            ViewData["AgentCode"] = from a in serviceDropDown.GetAgent() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == model.AgentCode };
            ViewData["TransportationCode"] = from a in serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == model.TransportationCode };

            return View(model);
        }

        public IActionResult DropEditData(OrderContainer model)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using (TransactionScope tr = new TransactionScope())
            {
                try
                {
                    model.ContainerNo = model.ContainerNo.ToUpper();
                    model.SealNo = model.SealNo != null ? model.SealNo : "-";
                    model.TransportationCode = model.TransportationCode != null ? model.TransportationCode : "25000";
                    model.AgentCode = model.AgentCode != null ? model.AgentCode : "001";
                    model.TareWeight = model.TareWeight != null ? model.TareWeight : 0;
                    db.OrderContainers.Update(model);
                    db.SaveChanges();
                    tr.Complete();
                    response.result = true;
                    response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
                }
                catch (Exception ex)
                {
                    db.Dispose();
                    response.result = false;
                    response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                    response.errorException = ex;
                }
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public IActionResult MatchEditData(OrderContainer model, OrderContainerMatchdetail matchDetail)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using (TransactionScope tr = new TransactionScope())
            {
                try
                {
                    model.ContainerNo = model.ContainerNo.ToUpper();
                    model.SealNo = model.SealNo != null ? model.SealNo : "-";
                    model.PaymentStageCode = model.IsExchange == true ? model.PaymentStageCode : "C";
                    model.TransportationCode = model.TransportationCode != null ? model.TransportationCode : "25000";
                    model.AgentCode = model.AgentCode != null ? model.AgentCode : "001";
                    model.TareWeight = model.TareWeight != null ? model.TareWeight : 0;

                    matchDetail.OrderCode = model.OrderCode;
                    model.OrderContainerMatchdetail = matchDetail;

                    db.OrderContainers.Update(model);
                    db.SaveChanges();
                    tr.Complete();
                    response.result = true;
                    response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
                }
                catch (Exception ex)
                {
                    db.Dispose();
                    response.result = false;
                    response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                    response.errorException = ex;
                }
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public IActionResult SetToDrop(string code)
        {
            var serviceCode = new CodeRepository(db);
            ResponseViewModel response = new ResponseViewModel();
            using (TransactionScope tr = new TransactionScope())
            {
                try
                {
                    var data = db.OrderContainers.FirstOrDefault(w => w.OrderCode == code);
                    data.IsApprove = true;
                    db.SaveChanges();

                    var newOrder = data;
                    newOrder.OrderCode = serviceCode.GetOrderContainerCode();
                    newOrder.IssueType = "DROP";
                    newOrder.IsApprove = false;
                    newOrder.ContainerStatus = "PN";

                    db.OrderContainers.Add(newOrder);
                    db.SaveChanges();
                    tr.Complete();
                    response.result = true;
                    response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
                }
                catch (Exception ex)
                {
                    db.Dispose();
                    response.result = false;
                    response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                    response.errorException = ex;
                }
            }

            Thread.Sleep(2000);
            return Json(response);
        }

        public async Task<IActionResult> GetMatchContainer(string order_code)
        {
            var containerData = await (from a in db.OrderContainers
                                       join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                                       join c in db.OrderContainerMatchdetails on a.OrderCode equals c.OrderCode
                                       join d in db.TransAgents on a.AgentCode equals d.AgentCode
                                       where a.OrderCode == order_code
                                       select new
                                       {
                                           container_no = a.ContainerNo,
                                           container_size_code = a.ContainerSizeCode,
                                           container_size_name = b.ContainerSizeName,
                                           agent_code = a.AgentCode,
                                           agent_name = d.AgentName,
                                           is_exchange = a.IsExchange,
                                           is_paid = a.PaymentStageCode,
                                           tareweight = a.TareWeight
                                       }).FirstOrDefaultAsync();

            return Json(containerData);
        }

        public IActionResult GetDropContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            var _orderData = (from a in db.OrderContainers
                              join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                              join f in db.TransContainerSizes on a.ContainerSizeCode equals f.ContainerSizeCode
                              where a.IsApprove == false && a.IssueType == "DROP"
                              select new OrderContainerModel
                              {
                                  order_code = a.OrderCode,
                                  issue_type = a.IssueType,
                                  container_no = a.ContainerNo,
                                  container_type = f.ContainerSizeName,
                                  truck_license = a.TruckLicense,
                                  transportation_name = d.TransportationName,
                                  container_status = a.ContainerStatus,
                                  is_receipt = a.IsReceipt,
                              }).ToList();
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

        public IActionResult GetMatchContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
        {
            var _orderData = (from a in db.OrderContainers
                              join c in db.TransContainerSizes on a.ContainerSizeCode equals c.ContainerSizeCode
                              join d in db.OrderContainerMatchdetails on a.OrderCode equals d.OrderCode
                              join e in db.TransTransportations on a.TransportationCode equals e.TransportationCode
                              join b in db.TransAgents on a.AgentCode equals b.AgentCode
                              where a.IsApprove == false && a.IssueType == "MATCH" && a.IsEnabled == true
                              select new OrderContainerModel
                              {
                                  order_code = a.OrderCode,
                                  issue_type = a.IssueType,
                                  transportation_name = e.TransportationName,
                                  container_no = a.ContainerNo,
                                  container_type = c.ContainerSizeName,
                                  truck_license = a.TruckLicense,
                                  container_status = a.ContainerStatus,
                                  match_type = d.MatchType,
                                  is_exchange = a.IsExchange,
                                  is_receipt = a.IsReceipt,
                                  agent_name = b.AgentName,
                                  payment_stage = a.PaymentStageCode
                              }).ToList();
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

        public async Task<IActionResult> GetTransportationByCode(string code)
        {
            var data = await db.TransTransportations.Where(w => w.TransportationCode == code).FirstOrDefaultAsync();
            return Json(data);
        }

        public async Task<IActionResult> Approve(string code)
        {
            var log = new LogRepository(db);
            var serviceCode = new CodeRepository(db);

            ResponseViewModel response = new ResponseViewModel();
            using var tr = await db.Database.BeginTransactionAsync();
            try
            {
                var orderData = await db.OrderContainers.Include(i => i.OrderContainerMatchdetail).FirstOrDefaultAsync(w => w.OrderCode == code);
                if (orderData.IssueType == "MATCH")
                {
                    if (orderData.OrderContainerMatchdetail.MatchType == "RETURN")
                    {
                        orderData.IsApprove = true;
                        orderData.IsReceipt = true;
                        orderData.ContainerStatus = "AC";

                        OrderContainerLocation locationData = new OrderContainerLocation();
                        locationData.OrderCode = code;
                        locationData.LocationStatus = "IN";
                        locationData.OrderCode = orderData.OrderCode;
                        locationData.ContainerNo = orderData.ContainerNo;
                        locationData.ContainerSizeCode = orderData.ContainerSizeCode;
                        locationData.CreateDate = DateTime.Now;
                        db.OrderContainerLocations.Add(locationData);

                        OrderContainerRepair repairData = new OrderContainerRepair();
                        repairData.OrderCode = orderData.OrderCode;
                        repairData.ContainerNo = orderData.ContainerNo;
                        repairData.RepairStatusCode = "P";
                        db.OrderContainerRepairs.Add(repairData);
                    }
                    else
                    {
                        orderData.IsApprove = true;
                        orderData.IsReceipt = true;
                        orderData.ContainerStatus = "DO";

                        var locationData = await db.OrderContainerLocations.FirstOrDefaultAsync(w => w.ContainerNo == orderData.ContainerNo);
                        db.OrderContainerLocations.Remove(locationData);
                    }
                }
                else
                {
                    if (orderData.OrderContainerMatchdetail.MatchType == "RETURN")
                    {
                        orderData.IsApprove = true;
                        orderData.ContainerStatus = "AC";

                        OrderContainerLocation locationData = new OrderContainerLocation();
                        locationData.OrderCode = code;
                        locationData.LocationStatus = "IN";
                        locationData.OrderCode = orderData.OrderCode;
                        locationData.ContainerNo = orderData.ContainerNo;
                        locationData.ContainerSizeCode = orderData.ContainerSizeCode;
                        locationData.CreateDate = DateTime.Now;
                        db.OrderContainerLocations.Add(locationData);
                    }
                    else
                    {
                        orderData.IsApprove = true;
                        orderData.IsReceipt = true;
                        orderData.ContainerStatus = "DO";

                        var locationData = await db.OrderContainerLocations.FirstOrDefaultAsync(w => w.ContainerNo == orderData.ContainerNo);
                        db.OrderContainerLocations.Remove(locationData);
                    }
                }

                await db.SaveChangesAsync();
                await tr.CommitAsync();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                await tr.RollbackAsync();
                await tr.DisposeAsync();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex.InnerException;
            }

            Thread.Sleep(2000);
            return Json(response);
        }
    }
}
