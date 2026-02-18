using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using AVAYardWeb.Services;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AVAYardWeb.Controllers;
[Authorize]
public class AgentController : Controller
{
    private ILogService log;
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public AgentController(DbavayardContext _context, ILogService _log)
    {
        db = _context;
        log = _log;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetAgentDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var dataAgent = await (from a in db.TransAgents
                               where a.IsEnabled == true
                               select a).ToListAsync();

        var data = dataAgent.Where(w => (iFilter.filterName == null || w.AgentName.ToUpper().Contains(iFilter.filterName.ToUpper())));

        Func<TransAgent, string> orderingFunction = (c => param.iSortCol_0 == 1 ? c.AgentName : c.AgentName);

        IEnumerable<TransAgent> listQuery;
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

    public async Task<IActionResult> AddData(TransAgent model)
    {
        var serviceCode = new CodeRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            model.AgentCode = await serviceCode.GetAgentCode();
            model.AgentName = model.AgentName.ToUpper();
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;

            db.TransAgents.Add(model);
            await db.SaveChangesAsync();

            log.AddLog("Add", "Agent", model.AgentCode, null, model, this.LoggedInUser);
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
        var model = await db.TransAgents.Where(w => w.AgentCode == code).FirstOrDefaultAsync();
        return View(model);
    }

    public async Task<IActionResult> EditData(TransAgent model)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldTransAgent = await db.TransAgents.Where(w => w.AgentCode == model.AgentCode).AsNoTracking().FirstOrDefaultAsync();

            model.AgentName = model.AgentName.ToUpper();
            db.TransAgents.Update(model);
            await db.SaveChangesAsync();

            log.AddLog("Edit", "Agent", model.AgentCode, oldTransAgent, model, this.LoggedInUser);
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

    public async Task<IActionResult> Remove(string code)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            var oldAgent = await db.TransAgents.Where(w => w.AgentCode == code).AsNoTracking().FirstOrDefaultAsync();

            var agent = await db.TransAgents.FirstOrDefaultAsync(w => w.AgentCode == code);
            agent.IsActived = false;
            agent.IsEnabled = false;
            await db.SaveChangesAsync();

            log.AddLog("Remove", "Agent", code, oldAgent, agent, this.LoggedInUser);
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
            var oldAgent = await db.TransAgents.Where(w => w.AgentCode == code).AsNoTracking().FirstOrDefaultAsync(); ;

            var agent = await db.TransAgents.FirstOrDefaultAsync(w => w.AgentCode == code);
            agent.IsActived = true;
            await db.SaveChangesAsync();

            log.AddLog("Active", "Agent", code, oldAgent, agent, this.LoggedInUser);
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
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                var oldAgent = await db.TransAgents.Where(w => w.AgentCode == code).AsNoTracking().FirstOrDefaultAsync(); ;

                var agent = await db.TransAgents.FirstOrDefaultAsync(w => w.AgentCode == code);
                agent.IsActived = false;
                await db.SaveChangesAsync();

                log.AddLog("Inactive", "Agent", code, oldAgent, agent, this.LoggedInUser);
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
        }

        return Json(response);
    }
}