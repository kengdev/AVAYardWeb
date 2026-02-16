using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace AVAYardWeb.Controllers;
[Authorize]
public class ContainerSizeController : Controller
{
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public ContainerSizeController(DbavayardContext context)
    {
        db = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GetContainerTypeDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var dataAgent = (from a in db.TransContainerSizes
                         where a.IsEnabled == true
                         select a).ToList();

        var data = dataAgent.Where(w => (iFilter.filterContainerSize == null || w.ContainerSizeName.ToUpper().Contains(iFilter.filterContainerSize.ToUpper())));

        Func<TransContainerSize, string> orderingFunction = (c => param.iSortCol_0 == 1 ? c.ContainerSizeName : c.ContainerSizeName);

        IEnumerable<TransContainerSize> listQuery;
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

    public IActionResult Add()
    {
        return View();
    }

    public IActionResult AddData(TransContainerSize model)
    {
        var serviceCode = new CodeRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            model.ContainerSizeCode = serviceCode.GetContainerTypeCode();
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;

            db.TransContainerSizes.Add(model);
            db.SaveChanges();
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

        Thread.Sleep(2000);
        return Json(response);
    }

    public IActionResult Edit(string code)
    {
        var model = db.TransContainerSizes.Where(w => w.ContainerSizeCode == code).FirstOrDefault();
        return View(model);
    }

    public IActionResult EditData(TransContainerSize dataToEdit)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                db.TransContainerSizes.Update(dataToEdit);
                log.AddSystemLog(dataToEdit.ContainerSizeName, "Update container type.", this.LoggedInUser);

                db.SaveChanges();
                tr.Complete();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                tr.Dispose();
                db.Dispose();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }
        }
        Thread.Sleep(2000);
        return Json(response);
    }

    public IActionResult Remove(string code)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                var containerType = db.TransContainerSizes.FirstOrDefault(w => w.ContainerSizeCode == code);
                containerType.IsActived = false;
                containerType.IsEnabled = false;

                log.AddSystemLog(containerType.ContainerSizeName, "Remove container type.", this.LoggedInUser);
                db.SaveChanges();
                tr.Complete();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                tr.Dispose();
                db.Dispose();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }
        }

        Thread.Sleep(2000);
        return Json(response);
    }

    public IActionResult Active(string code)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                var containerType = db.TransContainerSizes.FirstOrDefault(w => w.ContainerSizeCode == code);
                containerType.IsActived = true;

                log.AddSystemLog(containerType.ContainerSizeName, "Active container type.", this.LoggedInUser);
                db.SaveChanges();
                tr.Complete();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                tr.Dispose();
                db.Dispose();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }
        }

        Thread.Sleep(2000);
        return Json(response);
    }

    public IActionResult Inactive(string code)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                var containerType = db.TransContainerSizes.FirstOrDefault(w => w.ContainerSizeCode == code);
                containerType.IsActived = false;

                log.AddSystemLog(containerType.ContainerSizeName, "Inactive container type.", this.LoggedInUser);
                db.SaveChanges();
                tr.Complete();
                response.result = true;
                response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
            }
            catch (Exception ex)
            {
                tr.Dispose();
                db.Dispose();
                response.result = false;
                response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
                response.errorException = ex;
            }
        }

        Thread.Sleep(2000);
        return Json(response);
    }
}