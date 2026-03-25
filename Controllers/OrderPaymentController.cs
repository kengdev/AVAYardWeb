using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace AVAYardWeb.Controllers
{
    public class OrderPaymentController : Controller
    {
        private readonly DbavayardContext db;
        private readonly IDisplayService displayService;
        private string LoggedInUser => User.Identity.Name;
        private const string KSHOP_TEMPLATE = "0002010102110216478772000245472004155303920002454991531343007640052044640122067749700130810016A00000067701011201150107536000315080214KB0000018904640320KPS004KB00000189046431690016A00000067701011301030040214KB0000018904640420KPS004KB00000189046451430014A0000000041010010641697102111234567890152045631530376454041.005802TH5910NOKKY SHOP6004CITY62240508854304870708420677496304E809";

        public OrderPaymentController(DbavayardContext context, IDisplayService _displayService)
        {
            db = context;
            displayService = _displayService;
        }

        public async Task<IActionResult> IssueDropWithVat(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var serviceDropDown = new DropListRepository(db);
            var paymentData = await servicePayment.GetDropPaymentDataByOrder(code);
            paymentData.IssueType = "DROP";

            ViewData["TransportationCode"] = from a in await serviceDropDown.GetTransportation() select new SelectListItem { Value = a.key.ToString(), Text = a.label };
            return View(paymentData);
        }

        public async Task<IActionResult> IssueDropWithOutVat(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var serviceDropDown = new DropListRepository(db);
            var paymentData = await servicePayment.GetDropVoucherDataByOrder(code);
            paymentData.IssueType = "DROP";

            return View(paymentData);
        }

        public async Task<IActionResult> IssueMatch(string code)
        {
            var servicePayment = new PaymentRepository(db);
            var paymentData = await servicePayment.GetMatchPaymentDataByOrder(code);
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
        public async Task<IActionResult> AddDataWithoutVat(OrderPaymentVoucher model, OrderPaymentVoucherDetail detail)
        {
            var _service = new PaymentRepository(db);
            model.NetTotal = detail.ServiceCharge + detail.Cost7Days + detail.Cost10Days;
            model.IsPaid = false;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;
            model.PaymentTypeCode = "02";
            model.OrderPaymentVoucherDetail = detail;

            var res = await _service.AddDataWithoutVat(model);
            if (this.GetGroup() == "POS")
            {
                var url = $"https://yardweb.avagloballogistics.com/orderpayment/displaypaymentpos?code={res.code}&is_vat=FALSE";
                await displayService.ShowAsync(url);
            }

            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddDataWithVat(OrderPayment model, OrderPaymentDetail detail)
        {
            var _service = new PaymentRepository(db);
            model.Total = detail.ServiceCharge + detail.Cost7Days + detail.Cost10Days + detail.RepairCharge;
            model.IsPaid = false;
            model.CreateDate = DateTime.Now;
            model.CreateBy = this.LoggedInUser;
            model.OrderPaymentDetail = detail;
            model.PaymentTypeCode = "02";

            model.OrderPaymentDetail = detail;

            var res = await _service.AddDataWithVat(model);
            if (this.GetGroup() == "POS")
            {
                var url = $"https://yardweb.avagloballogistics.com/orderpayment/displaypaymentpos?code={res.code}&is_vat=TRUE";
                await displayService.ShowAsync(url);
            }

            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string PaymentCode)
        {
            var servicePayment = new PaymentRepository(db);
            var response = await servicePayment.Cancel(PaymentCode);
            if (this.GetGroup() == "POS")
            {
                await displayService.HideAsync();
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string PaymentCode, bool IsVat)
        {
            var servicePayment = new PaymentRepository(db);
            var response = new ResponseViewModel();
            if (IsVat)
            {
                response = await servicePayment.ApproveVat(PaymentCode);
            }
            else
            {
                response = await servicePayment.ApproveWithoutVat(PaymentCode);
            }

            if (this.GetGroup() == "POS")
            {
                await displayService.HideAsync();
            }

            return Json(response);
        }

        #region Display payment box
        [AllowAnonymous]
        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> DisplayPaymentPOS(string code, bool is_vat)

        {
            var model = new PaymentViewModel();
            var servicePayment = new PaymentRepository(db);
            if (is_vat)
            {
                var orderData = await servicePayment.GetPaymentByCode(code);
                model.ContainerNo = orderData.ContainerNo;
                model.ContainerSize = orderData.ContainerSizeCodeNavigation.ContainerSizeName;
                model.TaxId = orderData.TaxId;
                model.Name = orderData.TaxName;
                model.Address = orderData.TaxAddress;
                model.Amount = orderData.NetTotal;
                model.BankCode = orderData.BankCode;
                model.IsTaxInvoice = orderData.IsTaxInvoice;
            }
            else
            {
                var orderData = await servicePayment.GetVoucherByCode(code);
                model.ContainerNo = orderData.ContainerNo;
                model.ContainerSize = orderData.ContainerSizeCodeNavigation.ContainerSizeName;
                model.Name = orderData.TransportationName;
                model.Amount = orderData.NetTotal;
                model.BankCode = orderData.BankCode;
            }

            string newPayload = "";
            if (model.BankCode == "01")
            {
                newPayload = SCBShopQrCode.BuildFixedAmountPayload(model.Amount);
            }
            else
            {
                newPayload =
                    KShopQrTemplateEditor.UpdateAmountAndRebuild(
                        KSHOP_TEMPLATE,
                        model.Amount
                    );
                // 2) สร้าง QR เป็น Base64
            }
            string base64 = CreateQrBase64(newPayload);

            model.RefNo = code;
            model.IsVat = is_vat;
            model.QrBase64 = base64;

            return View(model);
        }

        [SupportedOSPlatform("windows")]
        public async Task<IActionResult> DisplayPayment(string code, bool is_vat)
        {
            var model = new PaymentViewModel();
            var servicePayment = new PaymentRepository(db);
            if (is_vat)
            {
                var orderData = await servicePayment.GetPaymentByCode(code);
                model.TaxId = orderData.TaxId;
                model.Name = orderData.TaxName;
                model.Address = orderData.TaxAddress;
                model.Amount = orderData.NetTotal;
                model.BankCode = orderData.BankCode;
            }
            else
            {
                var orderData = await servicePayment.GetVoucherByCode(code);
                model.Name = orderData.TransportationName;
                model.Amount = orderData.NetTotal;
                model.BankCode = orderData.BankCode;
            }

            string newPayload = "";
            if (model.BankCode == "01")
            {
                newPayload = SCBShopQrCode.BuildFixedAmountPayload(model.Amount);
            }
            else
            {
                newPayload =
                    KShopQrTemplateEditor.UpdateAmountAndRebuild(
                        KSHOP_TEMPLATE,
                        model.Amount
                    );
                // 2) สร้าง QR เป็น Base64
            }
            string base64 = CreateQrBase64(newPayload);

            model.RefNo = code;
            model.IsVat = is_vat;
            model.QrBase64 = base64;

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

        private string GetGroup()
        {
            var role = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .FirstOrDefault();

            return role;
        }
    }
}
