
using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AVAYardWeb.Controllers;
public class AccountController : Controller
{
    private readonly DbavayardContext db;
    private string LoggedInUser => User.Identity.Name;

    public AccountController(DbavayardContext context)
    {
        db = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Add()
    {
        var dropdown = new DropListRepository(db);
        ViewData["AccGroup"] = from a in dropdown.GetGroup() select new SelectListItem { Value = a.key.ToString(), Text = a.label };

        return View();
    }

    public async Task<IActionResult> AddData(AccAccount model)
    {
        var response = new ResponseViewModel();
        try
        {
            model.AccPassword = this.SaltHash(model.AccPassword);
            model.IsActived = true;
            model.IsEnabled = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;
            
            db.AccAccounts.Add(model);
            await db.SaveChangesAsync();

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

    public async Task<IActionResult> Edit(string code)
    {
        var dropdown = new DropListRepository(db);
        var model = await db.AccAccounts.Where(w => w.AccUsername == code).FirstOrDefaultAsync();

        ViewData["GroupCode"] = from a in dropdown.GetGroup() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
        return View(model);
    }

    public async Task<IActionResult> GetAccountDataList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var query = await db.AccAccounts.Where(w => w.IsEnabled == true).ToListAsync();
        var data = query.Where(w => (iFilter.filterName == null || w.AccFullname.ToUpper().Contains(iFilter.filterName.ToUpper())));

        Func<AccAccount, string> orderingFunction = (c => param.iSortCol_0 == 1 ? c.AccUsername : c.AccUsername);

        IEnumerable<AccAccount> listQuery;
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

    private string SaltHash(string input)
    {
        Rfc2898DeriveBytes PBKDF2_hash = new Rfc2898DeriveBytes(input, Encoding.ASCII.GetBytes("rm4fSDh0sofK"), 20000);
        HMACSHA256 HMACSHA256_hash = new HMACSHA256();
        //System.Text.Encoding.UTF8.GetString(bytes);
        string hPassword = Convert.ToBase64String(PBKDF2_hash.GetBytes(32));

        return hPassword;
    }
}