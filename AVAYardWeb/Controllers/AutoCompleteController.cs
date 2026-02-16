using Microsoft.AspNetCore.Mvc;
using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using AVAYardWeb.Repositories;

namespace AVAYardWeb.Controllers;
[Authorize]
public class AutoCompleteController : Controller
{
    private readonly DbavayardContext _context;

    public AutoCompleteController(DbavayardContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Customer(string term)
    {
        var auto = new AutoCompleteRepository(_context);
        var query = await auto.Customer(term);
        return Json(query);
    }

    public async Task<IActionResult> TaxAddress(string term)
    {
        var auto = new AutoCompleteRepository(_context);
        var query = await auto.TaxAddress(term);

        return Json(query);
    }

    public async Task<IActionResult> ContainerInYard(string term)
    {
        var auto = new AutoCompleteRepository(_context);
        var query = await auto.GetContainerInYard(term);

        return Json(query);
    }

    public async Task<IActionResult> Transportation(string term)
    {
        var auto = new AutoCompleteRepository(_context);
        var query = await auto.GetTransportation(term);

        return Json(query);
    }
}