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
public class TaxController : Controller
{
    private readonly ILogService log;
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public TaxController(DbavayardContext context, ILogService _log)
    {
        db = context;
        log = _log;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetTransportationByCode(string code)
    {
        var dataTransportation = await db.TransTransportations.Where(w => w.TransportationCode == code).FirstOrDefaultAsync();
        return Json(dataTransportation);
    }

    public async Task<IActionResult> GetTaxDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var dataAgent = await (from a in db.TaxAddresses
                               where a.IsEnabled == true
                               select a).ToListAsync();

        var data = dataAgent.Where(w => (iFilter.filterName == null || w.Name.ToUpper().Contains(iFilter.filterName.ToUpper())));

        Func<TaxAddress, string> orderingFunction = (c => param.iSortCol_0 == 1 ? c.Name : c.Name);

        IEnumerable<TaxAddress> listQuery;
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

    public async Task<IActionResult> AddData(TaxAddress model)
    {
        var serviceCode = new CodeRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            model.Name = model.Name.ToUpper();
            model.Acronym = model.Acronym.ToUpper();
            model.Phone = model.Phone != null ? model.Phone : "-";
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;

            db.TaxAddresses.Add(model);
            await db.SaveChangesAsync();

            log.AddLog("Add", "TaxAddress", model.TaxId, null, model, this.LoggedInUser);
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
        var model = await db.TaxAddresses.Where(w => w.TaxId == code).FirstOrDefaultAsync();

        ViewData["BranchType"] = from a in serviceDropDown.BranchType() select new SelectListItem { Value = a.key.ToString(), Text = a.label, Selected = a.key == model.BranchName };
        return View(model);
    }

    public async Task<IActionResult> EditData(TaxAddress model)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTax = await db.TaxAddresses.Where(w => w.TaxId == model.TaxId).AsNoTracking().FirstOrDefaultAsync();

            model.Name = model.Name.ToUpper();
            db.TaxAddresses.Update(model);
            await db.SaveChangesAsync();

            log.AddLog("Edit", "TaxAddress", model.TaxId, oldTax, model, this.LoggedInUser);
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
            var oldTax = await db.TaxAddresses.Where(w => w.TaxId == code).AsNoTracking().FirstOrDefaultAsync();

            var tax = db.TaxAddresses.FirstOrDefault(w => w.TaxId == code);
            tax.IsActived = false;
            tax.IsEnabled = false;
            await db.SaveChangesAsync();

            log.AddLog("Remove", "TaxAddress", code, oldTax, tax, this.LoggedInUser);
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
            var oldTax = await db.TaxAddresses.Where(w => w.TaxId == code).AsNoTracking().FirstOrDefaultAsync();

            var tax = db.TaxAddresses.FirstOrDefault(w => w.TaxId == code);
            tax.IsActived = true;
            await db.SaveChangesAsync();

            log.AddLog("Active", "TaxAddress", code, oldTax, tax, this.LoggedInUser);
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
            var oldTax = await db.TaxAddresses.Where(w => w.TaxId == code).AsNoTracking().FirstOrDefaultAsync();

            var tax = db.TaxAddresses.FirstOrDefault(w => w.TaxId == code);
            tax.IsActived = false;
            await db.SaveChangesAsync();

            log.AddLog("Inactive", "TaxAddress", code, oldTax, tax, this.LoggedInUser);
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
}