using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AVAYardWeb.Repositories;
public class AutoCompleteRepository
{
    private DbavayardContext db;

    public AutoCompleteRepository(DbavayardContext context)
    {
        db = context;
    }

    public async Task<List<DefaultAutoCompleteViewModel>> GetTransportation(string term)
    {
        var query = await (from a in db.TransTransportations
                           where (a.IsActived == true && a.TransportationName.Contains(term)) ||
                                 (a.IsActived == true && a.TransportationAcronym.Contains(term))
                           select new DefaultAutoCompleteViewModel
                           {
                               code = a.TransportationCode,
                               name = a.TransportationName,
                               title = "[" + a.TransportationAcronym + "] " + a.TransportationName
                           }).Take(30).ToListAsync();

        return query;
    }


    public async Task<List<DefaultAutoCompleteViewModel>> GetPendingContainer(string term)
    {
        var query = await  (from a in db.OrderContainers
                     where (a.IsReceipt == false && a.IsExchange == false && a.ContainerStatus == "AC" && a.ContainerNo.Contains(term))
                     select new DefaultAutoCompleteViewModel
                     {
                         code = a.OrderCode,
                         name = a.ContainerNo
                     }).Take(30).ToListAsync();

        return query;
    }

    public async Task<List<DefaultAutoCompleteViewModel>> GetContainerInYard(string term)
    {
        var query = await (from a in db.OrderContainers
                           join b in db.OrderContainerLocations on a.OrderCode equals b.OrderCode
                           join c in db.TransContainerSizes on a.ContainerSizeCode equals c.ContainerSizeCode
                           join d in db.TransAgents on a.AgentCode equals d.AgentCode
                           where (a.IsEnabled == true && a.IssueType == "MATCH" && a.ContainerNo.Contains(term))
                           select new DefaultAutoCompleteViewModel
                           {
                               code = a.OrderCode,
                               name = a.ContainerNo,
                               title = a.ContainerNo + " (" + c.ContainerSizeName + ")",
                               attr1 = c.ContainerSizeName,
                               attr2 = d.AgentName
                           }).Take(30).ToListAsync();

        return query;
    }

    public async Task<List<DefaultAutoCompleteViewModel>> Customer(string term)
    {
        var query = await (from a in db.TransTransportations
                     where (a.IsActived == true && a.TransportationName.Contains(term))
                     select new DefaultAutoCompleteViewModel
                     {
                         code = a.TransportationCode,
                         name = a.TransportationName,
                         value = a.TransportationCode
                     }).Take(30).ToListAsync();

        return query;
    }

    public async Task<List<DefaultAutoCompleteViewModel>> TaxAddress(string term)
    {
        var query = await (from a in db.TaxAddresses
                     where (a.IsActived == true && a.TaxId.Contains(term)) ||(a.IsActived == true && a.Name.Contains(term))
                     select new DefaultAutoCompleteViewModel
                     {
                         code = a.TaxId,
                         name = a.Name + " ",
                         address = a.Address,
                         title = a.TaxId + " (" + a.Name + ")",
                         attr1 = a.BranchType,
                         attr2 = a.BranchName
                     }).Take(30).ToListAsync();

        return query;
    }
}