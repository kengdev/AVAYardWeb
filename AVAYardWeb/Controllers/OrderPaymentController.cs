using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace AVAYardWeb.Controllers
{
    public class OrderPaymentController : Controller
    {
        private readonly DbavayardContext db;
        private string LoggedInUser => User.Identity.Name;
        private const string KSHOP_TEMPLATE = "0002010102110216478772000245472004155303920002454991531343007640052044640122067749700130810016A00000067701011201150107536000315080214KB0000018904640320KPS004KB00000189046431690016A00000067701011301030040214KB0000018904640420KPS004KB00000189046451430014A0000000041010010641697102111234567890152045631530376454041.005802TH5910NOKKY SHOP6004CITY62240508854304870708420677496304E809";

        public OrderPaymentController(DbavayardContext context)
        {
            db = context;
        }


        public async Task<IActionResult> IssueDrop(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var paymentData = await servicePayment.GetDropReceiptDataByOrder(code);
            paymentData.IssueType = "DROP";

            return View(paymentData);
        }

        public async Task<IActionResult> IssueMatch(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var paymentData = await servicePayment.GetMatchReceiptDataByOrder(code);
            paymentData.IssueType = "MATCH";

            var orderData = await (from a in db.OrderContainers
                                   join b in db.OrderContainerMatchdetails on a.OrderCode equals b.OrderCode
                                   join c in db.TransAgents on a.AgentCode equals c.AgentCode
                                   where a.OrderCode == code
                                   select new OrderContainerModel
                                   {
                                       agent_name = c.AgentName,
                                       issue_type = b.MatchType,
                                       is_exchange = a.IsExchange
                                   }).FirstOrDefaultAsync();

            ViewBag.OrderData = orderData;
            return View(paymentData);
        }

        [HttpPost]
        public async Task<IActionResult> AddData(OrderPayment model, OrderPaymentDetail detail)
        {
            var _service = new PaymentRepository(db);
            model.Total = detail.ServiceCharge + detail.Cost7Days + detail.Cost10Days;
            model.IsPaid = true;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;
            model.OrderPaymentDetail = detail;
            model.PaymentTypeCode = "02";

            model.OrderPaymentDetail = detail;

            var result = await _service.AddData(model);
            Thread.Sleep(2000);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string PaymentCode)
        {
            var servicePayment = new PaymentRepository(db);
            var response = await servicePayment.Cancel(PaymentCode);

            Thread.Sleep(2000);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string PaymentCode)
        {
            var servicePayment = new PaymentRepository(db);
            var response = await servicePayment.Approve(PaymentCode);

            Thread.Sleep(2000);
            return Json(response);
        }


        #region Display payment box
        [AllowAnonymous]
        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> DisplayPaymentPOS(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var orderData = await servicePayment.GetPaymentByCode(code);
            decimal amount = orderData.NetTotal;

            // 1) สร้าง payload ใหม่จาก template
            string newPayload =
                KShopQrTemplateEditor.UpdateAmountAndRebuild(
                    KSHOP_TEMPLATE,
                    amount
                );

            // 2) สร้าง QR เป็น Base64
            string base64 = CreateQrBase64(newPayload);

            var model = new PaymentViewModel
            {
                RefNo = code,
                Amount = amount,
                QrBase64 = base64,
                Payload = newPayload
            };

            return View(model);
        }

        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> DisplayPayment(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var orderData = await servicePayment.GetPaymentByCode(code);
            decimal amount = orderData.NetTotal;

            // 1) สร้าง payload ใหม่จาก template
            string newPayload =
                KShopQrTemplateEditor.UpdateAmountAndRebuild(
                    KSHOP_TEMPLATE,
                    amount
                );

            // 2) สร้าง QR เป็น Base64
            string base64 = CreateQrBase64(newPayload);

            var model = new PaymentViewModel
            {
                RefNo = code,
                Amount = amount,
                QrBase64 = base64,
                Payload = newPayload
            };

            return View(model);
        }

        [SupportedOSPlatform("windows")]
        private string CreateQrBase64(string payload)
        {
            using var bmp = CreateQrBitmap(payload);

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            return Convert.ToBase64String(ms.ToArray());
        }

        [SupportedOSPlatform("windows")]
        private Bitmap CreateQrBitmap(string payload)
        {
            var gen = new QRCoder.QRCodeGenerator();
            var data = gen.CreateQrCode(payload,
                QRCoder.QRCodeGenerator.ECCLevel.M);

            var qr = new QRCoder.QRCode(data);
            return qr.GetGraphic(6);
        }
        #endregion
    }
}
