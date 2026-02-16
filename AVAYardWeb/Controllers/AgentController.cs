using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace AVAYardWeb.Controllers;
[Authorize]
public class AgentController : Controller
{
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public AgentController(DbavayardContext context)
    {
        db = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GetAgentDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var dataAgent = (from a in db.TransAgents
                         where a.IsEnabled == true
                         select a).ToList();

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

    public IActionResult AddData(TransAgent model)
    {
        var serviceCode = new CodeRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            model.AgentCode = serviceCode.GetAgentCode();
            model.AgentName = model.AgentName.ToUpper();
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;

            db.TransAgents.Add(model);
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
        var model = db.TransAgents.Where(w => w.AgentCode == code).FirstOrDefault();
        return View(model);
    }

    public IActionResult EditData(TransAgent dataToEdit)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using (TransactionScope tr = new TransactionScope())
        {
            try
            {
                dataToEdit.AgentName = dataToEdit.AgentName.ToUpper();
                db.TransAgents.Update(dataToEdit);
                log.AddSystemLog(dataToEdit.AgentName, "Update agent.", this.LoggedInUser);

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
                var agent = db.TransAgents.FirstOrDefault(w => w.AgentCode == code);
                agent.IsActived = false;
                agent.IsEnabled = false;

                log.AddSystemLog(agent.AgentName, "Remove agent.", this.LoggedInUser);
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
                var agent = db.TransAgents.FirstOrDefault(w => w.AgentCode == code);
                agent.IsActived = true;

                log.AddSystemLog(agent.AgentName, "Active agent.", this.LoggedInUser);
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
                var agent = db.TransAgents.FirstOrDefault(w => w.AgentCode == code);
                agent.IsActived = false;

                log.AddSystemLog(agent.AgentName, "Inactive agent.", this.LoggedInUser);
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