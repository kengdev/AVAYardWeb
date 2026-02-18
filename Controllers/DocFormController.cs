using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Transactions;

namespace AVAYardWeb.Controllers
{
    [Authorize]
    public class DocFormController : Controller
    {
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;

        public DocFormController(DbavayardContext context)
        {
            db = context;
        }

        public async Task<IActionResult> PrintReceiptByPayment(string code)
        {
            List<OrderReceipt> orderReceipt = new List<OrderReceipt>();
            var receiptData = await db.OrderReceipts.Where(w => w.PaymentCode == code).ToListAsync();
            var paymentData = await db.OrderPayments.Where(w => w.PaymentCode == code).Include(i => i.OrderPaymentDetail).FirstOrDefaultAsync();
            foreach (var item in receiptData)
            {
                item.PaymentCodeNavigation = paymentData;
                orderReceipt.Add(item);
            }

            return new ViewAsPdf("PrintListReceipt", receiptData)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 5, Bottom = 5, Right = 5, Top = 5 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        public IActionResult PrintReceipt(string[] boxCode)
        {
            List<OrderReceipt> orderReceipt = new List<OrderReceipt>();
            foreach (var item in boxCode)
            {
                var receiptData = db.OrderReceipts.Where(w => w.ReceiptCode == item).FirstOrDefault();
                orderReceipt.Add(receiptData);
            }

            return new ViewAsPdf("PrintReceipt", orderReceipt)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 5, Bottom = 5, Right = 5, Top = 5 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        public IActionResult ContainerCheck(string code)
        {
            var _orderData = (from a in db.OrderContainers
                              join c in db.TransAgents on a.AgentCode equals c.AgentCode
                              join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                              join f in db.OrderTransactions on a.OrderCode equals f.OrderCode
                              join g in db.TransContainerSizes on a.ContainerSizeCode equals g.ContainerSizeCode
                              where a.OrderCode == code
                              select new OrderContainerModel
                              {
                                  order_code = a.OrderCode,
                                  container_no = a.ContainerNo,
                                  seal_no = a.SealNo,
                                  agent_name = c.AgentName,
                                  container_size = g.ContainerSizeName,
                                  truck_license = a.TruckLicense,
                                  transportation_name = d.TransportationName,
                                  issue_date = a.IssueDate,
                                  transaction_code = f.TransactionCode,
                                  tare_weight = a.TareWeight
                              }).FirstOrDefault();
            _orderData.check_result = db.OrderTransactionDetails.Where(w => w.TransactionCode == _orderData.transaction_code).ToList();
            _orderData.file_result = db.OrderTransactionFiles.Where(w => w.TransactionCode == _orderData.transaction_code && w.FileType == "drawn").ToList();

            return new ViewAsPdf("_ContainerCheck", _orderData)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 2, Bottom = 2, Right = 10, Top = 2 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        public IActionResult ContainerKeep(string code)
        {
            var _orderData = (from a in db.OrderContainers
                              join c in db.TransAgents on a.AgentCode equals c.AgentCode
                              join d in db.TransTransportations on a.TransportationCode equals d.TransportationCode
                              join g in db.TransContainerSizes on a.ContainerSizeCode equals g.ContainerSizeCode
                              where a.OrderCode == code
                              select new OrderContainerModel
                              {
                                  order_code = a.OrderCode,
                                  container_no = a.ContainerNo,
                                  agent_name = c.AgentName,
                                  container_size = g.ContainerSizeName,
                                  truck_license = a.TruckLicense,
                                  transportation_name = d.TransportationName,
                                  issue_date = a.IssueDate,
                                  tare_weight = a.TareWeight,
                                  remark = a.Remark
                              }).FirstOrDefault();

            return new ViewAsPdf("_ContainerKeep", _orderData)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 2, Bottom = 2, Right = 10, Top = 2 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        /*public IActionResult ReceiptContainer(string code)
        {
            var data = (from a in db.TransContainerSizes 
                        join b in db.RateServices on a.RateId equals c.Id
                        join d in db.OrderContainers on a.OrderCode equals d.OrderCode
                        where d. == code
                        select new ContainerHandlingViewModel
                        {
                            code = a.ReceiptCode,
                            customer_name = a.CustomerName,
                            customer_tax = a.CustomerTax,
                            customer_address = a.CustomerAddress,
                            receipt_date = a.ReceiptDate.ToString("dd-MM-yyyy"),
                            total = a.Total,
                            vat = a.Vat,
                            net_total = a.NetTotal,
                            payment_type = a.PaymentType,
                            bank_name = a.BankName,
                            branch = a.BankBranchName,
                            cheque_no = a.ChequeNo,
                            cheque_date = a.ChequeDate,
                            container_no = a.ContainerNo,
                            container_size = b.ContainerSizeName,
                            rate_id = a.RateId,
                            doc_type = d.IssueType
                        }).FirstOrDefault();

            data.rate = db.RateServices.Where(w => w.Id == data.rate_id).FirstOrDefault();
            if (data.doc_type == "OTHER")
            {
                data.other_cost = db.OrderReceiptOthers.Where(w => w.ReceiptCode == code).ToList();
            }
            else if (data.doc_type == "GATE")
            {
                data.gate_cost = db.OrderReceiptGatecharges.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }
            else if (data.doc_type == "LIFT")
            {
                data.lift_cost = db.OrderReceiptLiftcharges.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }
            else if (data.doc_type == "DROP")
            {
                data.overstay_cost = db.OrderReceiptOverstays.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }

            return new ViewAsPdf("ReceiptContainer", data)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 5, Bottom = 5, Right = 5, Top = 5 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }*/

        /*public IActionResult ReceiptContainerList(string code, string doc_type)
        {
            var data = (from a in db.OrderReceipts
                        join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                        join c in db.RateServices on a.RateId equals c.Id
                        where a.ReceiptCode == code
                        select new ContainerHandlingViewModel
                        {
                            code = a.ReceiptCode,
                            customer_name = a.CustomerName,
                            customer_tax = a.CustomerTax,
                            customer_address = a.CustomerAddress,
                            receipt_date = a.ReceiptDate.ToString("dd-MM-yyyy"),
                            total = a.Total,
                            vat = a.Vat,
                            net_total = a.NetTotal,
                            payment_type = a.PaymentType,
                            bank_name = a.BankName,
                            branch = a.BankBranchName,
                            cheque_no = a.ChequeNo,
                            cheque_date = a.ChequeDate,
                            container_no = a.ContainerNo,
                            container_size = b.ContainerSizeName,
                            rate_id = a.RateId
                        }).FirstOrDefault();

            data.rate = db.RateServices.Where(w => w.Id == data.rate_id).FirstOrDefault();
            data.doc_type = doc_type;
            if (data.doc_type == "OTHER")
            {
                data.other_cost = db.OrderReceiptOthers.Where(w => w.ReceiptCode == code).ToList();
            }
            else if (data.doc_type == "GATE")
            {
                data.gate_cost = db.OrderReceiptGatecharges.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }
            else if (data.doc_type == "LIFT")
            {
                data.lift_cost = db.OrderReceiptLiftcharges.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }
            else if (data.doc_type == "DROP")
            {
                data.overstay_cost = db.OrderReceiptOverstays.Where(w => w.ReceiptCode == code).FirstOrDefault();
            }

            return PartialView("ReceiptContainer", data);
            /*return new ViewAsPdf("ReceiptContainer", data)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 5, Bottom = 5, Right = 5, Top = 5 },
                CustomSwitches = "--disable-smart-shrinking"
            };
        }*/
    }
}
