using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using AVAYardWeb.Services;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AVAYardWeb.Controllers;
[Authorize]
public class TransportationController : Controller
{
    private ILogService log;
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public TransportationController(DbavayardContext _context, ILogService _log)
    {
        db = _context;
        log = _log;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetTransportationByCode(string code)
    {
        var dataTransportation = await  db.TransTransportations.Where(w => w.TransportationCode == code).FirstOrDefaultAsync();
        return Json(dataTransportation);
    }

    public async Task<IActionResult> GetTransportationDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var dataAgent = await (from a in db.TransTransportations
                               where a.IsEnabled == true
                               select a).ToListAsync();

        var data = dataAgent.Where(w => (iFilter.filterName == null || w.TransportationName.ToUpper().Contains(iFilter.filterName.ToUpper())));

        Func<TransTransportation, string> orderingFunction = (c => param.iSortCol_0 == 0 ? c.TransportationAcronym :
                                                                   param.iSortCol_0 == 1 ? c.TransportationName : c.TransportationName);

        IEnumerable<TransTransportation> listQuery;
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
        var serviceDropDown = new DropListRepository(db);
        ViewData["BranchType"] = from a in serviceDropDown.BranchType() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
        return View();
    }

    public async Task<IActionResult> AddData(TransTransportation model)
    {
        var serviceCode = new CodeRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            model.TransportationCode = await serviceCode.GetTransportationCode();
            model.TransportationName = model.TransportationName.ToUpper();
            model.TransportationAcronym = model.TransportationAcronym.ToUpper();
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;

            db.TransTransportations.Add(model);
            await db.SaveChangesAsync();

            log.AddLog("Add", "Transportation", model.TransportationCode, null, model, this.LoggedInUser);
            await log.SaveAsync();
            response.result = true;
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

    public async Task<IActionResult> Edit(string code)
    {
        var serviceDropDown = new DropListRepository(db);
        var model = await db.TransTransportations.Where(w => w.TransportationCode == code).FirstOrDefaultAsync();


        return View(model);
    }

    public async Task<IActionResult> EditData(TransTransportation model)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTransportation = await db.TransTransportations.Where(w => w.TransportationCode == model.TransportationCode).AsNoTracking().FirstOrDefaultAsync();

            model.TransportationName = model.TransportationName.ToUpper();
            model.TransportationAcronym = model.TransportationAcronym.ToUpper();
            db.TransTransportations.Update(model);
            await db.SaveChangesAsync();

            log.AddLog("Edit", "Transportation", model.TransportationCode, oldTransportation, model, this.LoggedInUser);
            await log.SaveAsync();
            response.result = true;
            response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
        }
        catch (Exception ex)
        {
            await db.DisposeAsync();
            response.result = false;
            response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            response.errorException = ex.InnerException;
        }

        return Json(response);
    }

    public async Task<IActionResult> Remove(string code)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTransTransportation = await db.TransTransportations.Where(w => w.TransportationCode == code).AsNoTracking().FirstOrDefaultAsync(); ;

            var transTransportation = await db.TransTransportations.FirstOrDefaultAsync(w => w.TransportationCode == code);
            transTransportation.IsActived = false;
            transTransportation.IsEnabled = false;
            await db.SaveChangesAsync();

            log.AddLog("Remove", "Transportation", code, oldTransTransportation, transTransportation, this.LoggedInUser);
            await log.SaveAsync();
            response.result = true;
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

    public async Task<IActionResult> Active(string code)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTransTransportation = await db.TransTransportations.Where(w => w.TransportationCode == code).AsNoTracking().FirstOrDefaultAsync(); ;

            var transTransportation = await db.TransTransportations.FirstOrDefaultAsync(w => w.TransportationCode == code);
            transTransportation.IsActived = true;
            await db.SaveChangesAsync();

            log.AddLog("Active", "Transportation", code, oldTransTransportation, transTransportation, this.LoggedInUser);
            await log.SaveAsync();
            response.result = true;
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

    public async Task<IActionResult> Inactive(string code)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTransTransportation = await db.TransTransportations.Where(w => w.TransportationCode == code).AsNoTracking().FirstOrDefaultAsync(); ;

            var transTransportation = await db.TransTransportations.FirstOrDefaultAsync(w => w.TransportationCode == code);
            transTransportation.IsActived = false;
            await db.SaveChangesAsync();

            log.AddLog("Inactive", "Transportation", code, oldTransTransportation, transTransportation, this.LoggedInUser);
            await log.SaveAsync();
            response.result = true;
            response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
        }
        catch (Exception ex)
        {
            response.result = false;
            response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            response.errorException = ex;
        }

        return Json(response);
    }
}