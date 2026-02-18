using AVAYardWeb.Components;
using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AVAYardWeb.Repositories;

public class PaymentRepository
{
    private DbavayardContext db;

    public PaymentRepository(DbavayardContext context)
    {
        db = context;
    }

    public async Task<OrderPayment> GetDropReceiptDataByOrder(string order_code)
    {
        var model = new OrderPayment();
        var orderDetail = new OrderPaymentDetail();

        var orderData = await (from a in db.OrderContainers
                               join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                               join c in db.RateDropServices on b.ContainerTypeCode equals c.ContainerTypeCode
                               where a.OrderCode == order_code
                               select new
                               {
                                   a.OrderCode,
                                   a.ContainerNo,
                                   b.ContainerSizeCode,
                                   b.ContainerSizeName,
                                   b.ContainerTypeCode,
                                   c.RateServiceCharge,
                                   a.IssueDate
                               }).FirstOrDefaultAsync();

        model.ContainerNo = orderData.ContainerNo;
        model.ContainerSizeCode = orderData.ContainerSizeCode;
        model.Total = orderData.RateServiceCharge;
        model.Vat = (orderData.RateServiceCharge * 7) / 100;
        model.NetTotal = model.Total + model.Vat;
        model.OrderCode = orderData.OrderCode;
        model.CreateDate = orderData.IssueDate;
        model.ContainerSizeCodeNavigation = await db.TransContainerSizes.Where(w => w.ContainerSizeCode == orderData.ContainerSizeCode).FirstOrDefaultAsync();

        orderDetail.ServiceCharge = orderData.RateServiceCharge;
        orderDetail.StayDays = (DateTime.Now.Date - orderData.IssueDate.Date).Days;

        if (orderDetail.StayDays > 7 && orderDetail.StayDays <= 10)
            orderDetail.Overstay7Days = (orderDetail.StayDays - 7);
        orderDetail.Cost7Days = orderDetail.Overstay7Days * 50;

        if (orderDetail.StayDays > 7 && orderDetail.StayDays > 10)
        {
            orderDetail.Overstay7Days = 3;
            orderDetail.Cost7Days = 150;
            orderDetail.Overstay10Days = (orderDetail.StayDays - 10);
            orderDetail.Cost10Days = (orderDetail.StayDays - 10) * 100;
        }

        model.OrderCodeNavigation = await db.OrderContainers.Where(w => w.OrderCode == orderData.OrderCode).FirstOrDefaultAsync();
        model.OrderPaymentDetail = orderDetail;
        return model;
    }

    public async Task<OrderPayment> GetMatchReceiptDataByOrder(string order_code)
    {
        var paymentData = new OrderPayment();
        var paymentDetail = new OrderPaymentDetail();

        var orderData = await (from m in db.OrderContainerMatchdetails
                               join oc in db.OrderContainers
                                     on m.OrderCode equals oc.OrderCode
                               join cs in db.TransContainerSizes
                                     on oc.ContainerSizeCode equals cs.ContainerSizeCode
                               join r in db.RateMatchServices
                                     on new
                                     {
                                         MatchType = m.MatchType,
                                         ContainerTypeCode = cs.ContainerTypeCode,
                                         AgentCode = oc.AgentCode
                                     }
                                     equals new
                                     {
                                         MatchType = r.MatchType,
                                         ContainerTypeCode = r.ContainerTypeCode,
                                         AgentCode = r.AgentCode
                                     }
                               where m.OrderCode == order_code
                               select new
                               {
                                   m.OrderCode,
                                   oc.ContainerNo,
                                   cs.ContainerSizeCode,
                                   cs.ContainerSizeName,
                                   r.ContainerTypeCode,
                                   r.RateServiceCharge,
                                   oc.IssueDate
                               }).FirstOrDefaultAsync();

        paymentData.ContainerNo = orderData.ContainerNo;
        paymentData.ContainerSizeCode = orderData.ContainerSizeCode;
        paymentData.Total = orderData.RateServiceCharge;
        paymentData.Vat = (orderData.RateServiceCharge * 7) / 100;
        paymentData.NetTotal = paymentData.Total + paymentData.Vat;
        paymentData.PaymentTypeCode = "02";
        paymentData.OrderCode = orderData.OrderCode;
        paymentData.CreateDate = orderData.IssueDate;
        paymentData.ContainerSizeCodeNavigation = await db.TransContainerSizes.Where(w => w.ContainerSizeCode == orderData.ContainerSizeCode).FirstOrDefaultAsync();

        paymentDetail.StayDays = (DateTime.Now.Date - orderData.IssueDate.Date).Days;
        paymentDetail.ServiceCharge = orderData.RateServiceCharge;
        paymentDetail.Overstay7Days = 0;
        paymentDetail.Cost7Days = 0;
        paymentDetail.Overstay10Days = 0;
        paymentDetail.Cost10Days = 0;

        paymentData.OrderPaymentDetail = paymentDetail;
        return paymentData;
    }

    public async Task<ResponseViewModel> AddData(OrderPayment model)
    {
        var log = new LogRepository(db);
        var serviceCode = new CodeRepository(db);
        var serviceReceipt = new YardReceiptRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using var tr = await db.Database.BeginTransactionAsync();
        try
        {
            model.PaymentCode = await serviceCode.GetPaymentCode();
            model.Total = (model.Total * 100m) / 107m;
            model.Vat = (model.Total * 7m) / 100m;
            model.NetTotal = model.Total + model.Vat;

            db.OrderPayments.Add(model);

            await db.SaveChangesAsync();
            await tr.CommitAsync();
            response.code = model.PaymentCode;
            response.result = true;
            response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
        }
        catch (Exception ex)
        {
            await tr.RollbackAsync();
            await tr.DisposeAsync();
            response.result = false;
            response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            response.errorException = ex.InnerException;
        }

        return response;
    }

    public async Task<OrderPayment> GetPaymentByCode(string code)
    {
        var paymentData = await db.OrderPayments.Where(w => w.PaymentCode == code).FirstOrDefaultAsync();
        return paymentData;
    }

    public async Task<ResponseViewModel> Cancel(string code)
    {
        var log = new LogRepository(db);
        ResponseViewModel response = new ResponseViewModel();
        using var tr = await db.Database.BeginTransactionAsync();
        try
        {
            var paymentData = await db.OrderPayments.FirstOrDefaultAsync(w => w.PaymentCode == code);
            var paymentDatail = await db.OrderPaymentDetails.FirstOrDefaultAsync(w => w.PaymentCode == code);
            db.OrderPaymentDetails.Remove(paymentDatail);
            db.OrderPayments.Remove(paymentData);

            await db.SaveChangesAsync();
            await tr.CommitAsync();
            response.result = true;
            response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
        }
        catch (Exception ex)
        {
            await tr.RollbackAsync();
            await tr.DisposeAsync();
            response.result = false;
            response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            response.errorException = ex.InnerException;
        }

        return response;
    }

    public async Task<ResponseViewModel> Approve(string code)
    {
        var log = new LogRepository(db);
        var serviceCode = new CodeRepository(db);

        ResponseViewModel response = new ResponseViewModel();
        using var tr = await db.Database.BeginTransactionAsync();
        try
        {
            var receiptEntity = new OrderReceipt();
            var paymentData = await db.OrderPayments.Where(w => w.PaymentCode == code).Include(i => i.OrderPaymentDetail).AsNoTracking().FirstOrDefaultAsync();
            var orderData = await db.OrderContainers.FirstOrDefaultAsync(w => w.OrderCode == paymentData.OrderCode);
            var orderMatch = await db.OrderContainerMatchdetails.Where(w => w.OrderCode == paymentData.OrderCode).FirstOrDefaultAsync();

            var balance = paymentData.NetTotal;
            int loopCount = (int)Math.Ceiling(paymentData.NetTotal / 1000);
            if (!paymentData.IsTaxInvoice)
            {
                for (int i = 0; i < loopCount; i++)
                {
                    receiptEntity = new OrderReceipt();

                    receiptEntity.ReceiptCode = await serviceCode.GetReceiptCode(1);
                    if (loopCount == 1)
                    {
                        receiptEntity.Total = (paymentData.NetTotal * 100m) / 107m;
                    }
                    else
                    {
                        if (balance > 1000)
                        {
                            balance = balance - 1000m;
                            receiptEntity.Total = (1000m * 100m) / 107m;
                        }
                        else
                        {
                            receiptEntity.Total = (balance * 100m) / 107m;
                        }
                    }

                    receiptEntity.PaymentCode = paymentData.PaymentCode;
                    receiptEntity.ContainerSizeCode = paymentData.ContainerSizeCode;
                    receiptEntity.ContainerNo = paymentData.ContainerNo;
                    receiptEntity.Description = "Container handling.";
                    receiptEntity.TaxId = paymentData.TaxId;
                    receiptEntity.TaxName = paymentData.TaxName;
                    receiptEntity.TaxAddress = paymentData.TaxAddress;
                    receiptEntity.IsTaxInvoice = paymentData.IsTaxInvoice;
                    receiptEntity.IssueSession = paymentData.IssueType;
                    receiptEntity.OrderCode = paymentData.OrderCode;
                    receiptEntity.Vat = (receiptEntity.Total * 7) / 100;
                    receiptEntity.NetTotal = receiptEntity.Total + receiptEntity.Vat;
                    receiptEntity.ReceiptDate = DateOnly.FromDateTime(DateTime.Now);
                    receiptEntity.PaymentType = "02";
                    receiptEntity.IsEnabled = true;
                    receiptEntity.CreateDate = DateTime.Now;
                    receiptEntity.CreateBy = paymentData.CreateBy;

                    db.OrderReceipts.Add(receiptEntity);
                    await db.SaveChangesAsync();
                }

            }
            else
            {
                receiptEntity = new OrderReceipt();
                receiptEntity.ReceiptCode = await serviceCode.GetReceiptCode(1);
                receiptEntity.PaymentCode = paymentData.PaymentCode;
                receiptEntity.ContainerSizeCode = paymentData.ContainerSizeCode;
                receiptEntity.ContainerNo = paymentData.ContainerNo;
                receiptEntity.Description = "Container handling.";
                receiptEntity.TaxId = paymentData.TaxId;
                receiptEntity.TaxName = paymentData.TaxName;
                receiptEntity.TaxAddress = paymentData.TaxAddress;
                receiptEntity.IsTaxInvoice = paymentData.IsTaxInvoice;
                receiptEntity.IssueSession = paymentData.IssueType;
                receiptEntity.OrderCode = paymentData.OrderCode;
                receiptEntity.IssueSession = paymentData.IssueType;
                receiptEntity.Total = paymentData.Total;
                receiptEntity.Vat = (receiptEntity.Total * 7m) / 100m;
                receiptEntity.NetTotal = receiptEntity.Total + receiptEntity.Vat;

                receiptEntity.NetTotal = paymentData.NetTotal + paymentData.Vat;
                receiptEntity.ReceiptDate = DateOnly.FromDateTime(DateTime.Now);
                receiptEntity.PaymentType = "02";
                receiptEntity.PaymentCode = paymentData.PaymentCode;
                receiptEntity.IsEnabled = true;
                receiptEntity.CreateDate = DateTime.Now;
                receiptEntity.CreateBy = paymentData.CreateBy;

                db.OrderReceipts.Add(receiptEntity);
                await db.SaveChangesAsync();
            }

            // เพิ่มค่าซ่อมตู้
            if (orderMatch.MatchType == "RETURN" && orderData.IsExchange == false && paymentData.OrderPaymentDetail.RepairCharge > 0)
            {
                receiptEntity = new OrderReceipt();
                receiptEntity.ReceiptCode = await serviceCode.GetReceiptCode(1);
                receiptEntity.PaymentCode = paymentData.PaymentCode;
                receiptEntity.ContainerSizeCode = paymentData.ContainerSizeCode;
                receiptEntity.ContainerNo = paymentData.ContainerNo;
                receiptEntity.Description = "Container repairing.";
                receiptEntity.TaxId = paymentData.TaxId;
                receiptEntity.TaxName = paymentData.TaxName;
                receiptEntity.TaxAddress = paymentData.TaxAddress;
                receiptEntity.IsTaxInvoice = paymentData.IsTaxInvoice;
                receiptEntity.IssueSession = paymentData.IssueType;
                receiptEntity.OrderCode = paymentData.OrderCode;
                receiptEntity.IssueSession = paymentData.IssueType;
                receiptEntity.Total = paymentData.OrderPaymentDetail.RepairCharge;
                receiptEntity.Vat = (receiptEntity.Total * 7m) / 100m;
                receiptEntity.NetTotal = receiptEntity.Total + receiptEntity.Vat;
                receiptEntity.ReceiptDate = DateOnly.FromDateTime(DateTime.Now);
                receiptEntity.PaymentType = "02";
                receiptEntity.PaymentCode = paymentData.PaymentCode;
                receiptEntity.IsEnabled = true;
                receiptEntity.CreateDate = DateTime.Now;
                receiptEntity.CreateBy = paymentData.CreateBy;

                db.OrderReceipts.Add(receiptEntity);
            }

            if (orderData.IssueType == "MATCH")
            {
                if (orderMatch.MatchType == "RETURN")
                {
                    orderData.IsApprove = true;
                    orderData.IsReceipt = true;
                    orderData.ContainerStatus = "AC";

                    OrderContainerLocation locationData = new OrderContainerLocation();
                    locationData.OrderCode = paymentData.OrderCode;
                    locationData.LocationStatus = "IN";
                    locationData.OrderCode = orderData.OrderCode;
                    locationData.ContainerNo = orderData.ContainerNo;
                    locationData.ContainerSizeCode = orderData.ContainerSizeCode;
                    locationData.CreateDate = DateTime.Now;
                    db.OrderContainerLocations.Add(locationData);

                    OrderContainerRepair repairData = new OrderContainerRepair();
                    repairData.OrderCode = paymentData.OrderCode;
                    repairData.ContainerNo = paymentData.ContainerNo;
                    repairData.RepairStatusCode = "P";
                    db.OrderContainerRepairs.Add(repairData);
                }
                else
                {
                    orderData.IsApprove = true;
                    orderData.IsReceipt = true;
                    orderData.ContainerStatus = "DO";

                    var locationData = await db.OrderContainerLocations.FirstOrDefaultAsync(w => w.ContainerNo == paymentData.ContainerNo);
                    db.OrderContainerLocations.Remove(locationData);
                }
            }
            else
            {
                orderData.IsApprove = true;
                orderData.IsReceipt = true;
                orderData.ContainerStatus = "DO";

                var locationData = await db.OrderContainerLocations.FirstOrDefaultAsync(w => w.ContainerNo == paymentData.ContainerNo);
                db.OrderContainerLocations.Remove(locationData);
            }

            var paymentModel = await db.OrderPayments.FirstOrDefaultAsync(w => w.PaymentCode == code);
            paymentModel.IsPaid = true;

            await db.SaveChangesAsync();
            await tr.CommitAsync();
            response.result = true;
            response.resultMessage = "บันทึกข้อมูลเรียบร้อยแล้ว";
        }
        catch (Exception ex)
        {
            await tr.RollbackAsync();
            await tr.DisposeAsync();
            response.result = false;
            response.resultMessage = "<div>เกิดข้อผิดพลาดระหว่างการทำงาน</div><div style='margin-top:-30px'> กรุณาลองใหม่อีกครั้ง หรือติดต่อผู้ดูแลระบบ</div>";
            response.errorException = ex.InnerException;
        }

        return response;
    }
}