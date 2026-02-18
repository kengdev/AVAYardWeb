using AVAYardWeb.Components;
using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AVAYardWeb.Repositories;
public static class DecimalExtensions
{
    public static decimal Truncate(this decimal value, int digits)
    {
        decimal factor = (decimal)Math.Pow(10, digits);
        return Math.Truncate(value * factor) / factor;
    }
}

public class YardReceiptRepository
{
    private DbavayardContext db;

    public YardReceiptRepository(DbavayardContext context)
    {
        db = context;
    }

    public async Task<OrderReceipt> GetOrderReceiptByCode(string code)
    {
        var receiptData = await db.OrderReceipts.Where(w => w.ReceiptCode == code).Include(i => i.ContainerSizeCodeNavigation).FirstOrDefaultAsync();
        return receiptData;
    }

    public async Task<ResponseViewModel> Edit(OrderReceipt model)
    {
        ResponseViewModel response = new ResponseViewModel();
        try
        {
            db.OrderReceipts.Update(model);
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

        return response;
    }

    public List<ContainerHandlingViewModel> GetContainerHandlingDataList(string month, string year)
    {
        var data = (from a in db.OrderReceipts
                    join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                    where a.IsEnabled == true
                    && a.ReceiptDate.Month == int.Parse(month)
                    && a.ReceiptDate.Year == int.Parse(year)
                    select new ContainerHandlingViewModel
                    {
                        code = a.ReceiptCode,
                        container_size = b.ContainerSizeName,
                        container_no = a.ContainerNo,
                        tax_id = a.TaxId,
                        tax_name = a.TaxName,
                        tax_address = a.TaxAddress,
                        net_total = a.NetTotal,
                        vat = a.Vat,
                        total = a.Total,
                        receipt_date = a.ReceiptDate.ToString("yyyy-MM-dd")
                    }).ToList();
        return data;
    }
}