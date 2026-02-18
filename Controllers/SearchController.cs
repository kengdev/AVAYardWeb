using Microsoft.AspNetCore.Mvc;
using AVAYardWeb.Models;
using AVAYardWeb.Repositories;
using AVAYardWeb.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace AVAWeb.Controllers;
public class SearchController : Controller
{
    private readonly DbavayardContext db;

    public SearchController(DbavayardContext context)
    {
        db = context;
    }

    public IActionResult Customer()
    {
        return View();
    }

    public IActionResult GetCustomerDataList(jQueryDataTableParamModel param)
    {
        var _orderData = (from a in db.TransTransportations
                          where a.IsActived == true 
                          select new CustomerModel
                          {
                              customer_code = a.TransportationCode,
                              customer_name = a.TransportationName
                          }).ToList();

        var data = _orderData.Where(w => (param.sSearch == null || w.customer_name.Contains(param.sSearch.ToUpper())));

        IEnumerable<CustomerModel> listQuery;
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

    public IActionResult Transportation()
    {
        return View();
    }

    public IActionResult GetTransportationDataList(jQueryDataTableParamModel param)
    {
        var _orderData = (from a in db.TransTransportations
                          where a.IsActived == true
                          select new CustomerModel
                          {
                              customer_code = a.TransportationCode,
                              customer_name = a.TransportationName
                          }).ToList();

        var data = _orderData.Where(w => (param.sSearch == null || w.customer_name.Contains(param.sSearch.ToUpper())));

        IEnumerable<CustomerModel> listQuery;
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

    public IActionResult Container()
    {
        return View();
    }

    public IActionResult GetReceiptContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var _orderData = (from a in db.OrderContainers
                          join b in db.OrderContainerLocations on a.OrderCode equals b.OrderCode
                          join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                          join f in db.TransContainerSizes on a.ContainerSizeCode equals f.ContainerSizeCode
                          where a.IsReceipt == false && a.IsExchange == false && a.ContainerStatus == "AC"
                          select new OrderContainerModel
                          {
                              order_code = a.OrderCode,
                              container_no = a.ContainerNo,
                              container_size = f.ContainerSizeName,
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

    public IActionResult MatchContainer()
    {
        return View();
    }

    public IActionResult GetMatchContainerList(jQueryDataTableParamModel param, avaDataTableParamModel iFilter)
    {
        var _orderData = (from a in db.OrderContainers
                          join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                          join c in db.OrderContainerMatchdetails on a.OrderCode equals c.OrderCode
                          join e in db.TransAgents on a.AgentCode equals e.AgentCode
                          join f in db.OrderContainerLocations on a.OrderCode equals f.OrderCode
                          where a.ContainerStatus == "AC" && a.IsEnabled == true && a.IsApprove == true
                          select new OrderContainerModel
                          {
                              order_code = a.OrderCode,
                              container_no = a.ContainerNo,
                              truck_license = a.TruckLicense,
                              container_status = a.ContainerStatus,
                              is_receipt = a.IsReceipt,
                              agent_name = e.AgentName,
                              container_size = b.ContainerSizeName
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


}