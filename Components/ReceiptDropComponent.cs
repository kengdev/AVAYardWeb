using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AVAYardWeb.Components
{
    public class ReceiptDrop : ViewComponent
    {
        private string LoggedInUser => User.Identity.Name;
        private readonly DbavayardContext db;

        public ReceiptDrop(DbavayardContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string code)
        {
            var data = await (from a in db.OrderReceipts
                        join b in db.TransContainerSizes on a.ContainerSizeCode equals b.ContainerSizeCode
                        join d in db.OrderContainers on a.OrderCode equals d.OrderCode
                        where a.ReceiptCode == code
                        select new ContainerHandlingViewModel
                        {
                            code = a.ReceiptCode,
                            tax_name = a.TaxName,
                            tax_id = a.TaxId,
                            tax_address = a.TaxAddress,
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
                            doc_type = d.IssueType
                        }).FirstOrDefaultAsync();

            //data.overstay_cost = await db.OrderPaymentDetails.Where(w => w.ReceiptCode == code).FirstOrDefaultAsync();

            return View(data);
        }
    }
}
