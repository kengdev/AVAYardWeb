using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using QRCoder;

public class PromptPayQrPrinter
{
    // =========================
    //  Public API
    // =========================

    /// <summary>
    /// สร้าง PromptPay QR (ฝังยอดเงิน) + พิมพ์ออกเครื่องพิมพ์
    /// </summary>
    /// <param name="promptPayPhone">เบอร์มือถือพร้อมเพย์ เช่น 0812345678</param>
    /// <param name="amount">ยอดเงิน เช่น 1250.00</param>
    /// <param name="printerName">ชื่อ Printer ใน Windows (Control Panel > Printers)</param>
    /// <param name="headerText">ข้อความหัวใบเสร็จ (optional)</param>
    /// <param name="footerText">ข้อความท้าย (optional)</param>
    public static string BuildForMerchantIdWithAmount(
    string merchantId,
    decimal amount)
    {
        // Merchant ID ไม่ต้องแปลง format แบบเบอร์โทร
        string aid = TLV("00", "A000000677010111");
        string mid = TLV("01", merchantId);

        string merchantAccount = TLV("29", aid + mid);

        string amountStr = amount.ToString("0.00",
            System.Globalization.CultureInfo.InvariantCulture);

        string data =
            TLV("00", "01") +
            TLV("01", "12") +
            merchantAccount +
            TLV("53", "764") +
            TLV("54", amountStr) +
            TLV("58", "TH");

        string toCrc = data + "6304";
        string crc = Crc16Ccitt(toCrc);

        return data + TLV("63", crc);
    }

    /// <summary>
    /// ถ้าเก่งอยากเอา payload ไปใช้ที่อื่น (เช่นแสดงบนจอ) ก็เรียกอันนี้ได้
    /// </summary>
    public static string BuildPromptPayPayload(string promptPayPhone, decimal amount)
        => BuildPayloadForPhoneWithAmount(promptPayPhone, amount);

    // =========================
    //  Payload builder (PromptPay / Thai QR)
    // =========================

    // TLV helper: Tag(2) + Length(2) + Value
    private static string TLV(string tag, string value)
        => tag + value.Length.ToString("D2") + value;

    // PromptPay phone format: 0812345678 -> 0066812345678 (remove leading 0, prefix 0066)
    private static string FormatPromptPayPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("promptPayPhone is required.");

        phone = new string(phone.Where(char.IsDigit).ToArray());
        if (phone.StartsWith("0")) phone = phone.Substring(1);

        if (phone.Length < 8)
            throw new ArgumentException("Invalid phone number for PromptPay.");

        return "0066" + phone;
    }

    // CRC16-CCITT (0x1021), init 0xFFFF
    private static string Crc16Ccitt(string input)
    {
        ushort crc = 0xFFFF;
        byte[] bytes = Encoding.ASCII.GetBytes(input);

        foreach (byte b in bytes)
        {
            crc ^= (ushort)(b << 8);
            for (int i = 0; i < 8; i++)
            {
                crc = (crc & 0x8000) != 0
                    ? (ushort)((crc << 1) ^ 0x1021)
                    : (ushort)(crc << 1);
            }
        }
        return crc.ToString("X4");
    }

    private static string BuildPayloadForPhoneWithAmount(string phone, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("amount must be > 0");

        // PromptPay Merchant Account Information (Tag 29)
        // Subtag 00 = AID, 01 = PromptPay ID (phone)
        string aid = TLV("00", "A000000677010111");
        string ppId = TLV("01", FormatPromptPayPhone(phone));
        string merchantAccount = TLV("29", aid + ppId);

        // Amount with 2 decimals (invariant culture)
        string amountStr = amount.ToString("0.00", CultureInfo.InvariantCulture);

        // EMVCo basics:
        // 00 Payload Format Indicator = "01"
        // 01 Point of Initiation Method: "12" (static)  -> ใช้งานแบบพิมพ์ใบได้
        // 53 Currency = "764" (THB)
        // 54 Amount
        // 58 Country = "TH"
        string data =
            TLV("00", "01") +
            TLV("01", "12") +
            merchantAccount +
            TLV("53", "764") +
            TLV("54", amountStr) +
            TLV("58", "TH");

        // CRC tag must be computed by appending "6304" first
        string toCrc = data + "6304";
        string crc = Crc16Ccitt(toCrc);

        return data + TLV("63", crc);
    }

    // =========================
    //  QR image generation
    // =========================

    private static Bitmap CreateQrBitmap(string payload, int pixelsPerModule = 6)
    {
        var gen = new QRCodeGenerator();
        var qrData = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        var qr = new QRCode(qrData);

        // Use default black/white
        return qr.GetGraphic(pixelsPerModule);
    }

    // =========================
    //  Printing (Windows)
    // =========================

    private static void PrintQrBitmap(
        string printerName,
        Bitmap qrBitmap,
        string headerText,
        string footerText,
        decimal amount
    )
    {
        if (string.IsNullOrWhiteSpace(printerName))
            throw new ArgumentException("printerName is required.");

        var pd = new PrintDocument();
        pd.PrinterSettings.PrinterName = printerName;

        pd.PrintPage += (s, e) =>
        {
            // Basic layout
            int marginLeft = 10;
            int y = 10;

            using var headerFont = new Font("Arial", 10, FontStyle.Bold);
            using var normalFont = new Font("Arial", 9, FontStyle.Regular);

            // Header
            if (!string.IsNullOrWhiteSpace(headerText))
            {
                e.Graphics.DrawString(headerText, headerFont, Brushes.Black, marginLeft, y);
                y += 22;
            }

            // Amount line
            e.Graphics.DrawString($"ยอดชำระ: {amount:0.00} บาท", normalFont, Brushes.Black, marginLeft, y);
            y += 18;

            // Center QR
            int pageWidth = e.PageBounds.Width;
            int xQr = Math.Max(marginLeft, (pageWidth - qrBitmap.Width) / 2);
            e.Graphics.DrawImage(qrBitmap, xQr, y, qrBitmap.Width, qrBitmap.Height);
            y += qrBitmap.Height + 10;

            // Footer
            if (!string.IsNullOrWhiteSpace(footerText))
            {
                e.Graphics.DrawString(footerText, normalFont, Brushes.Black, marginLeft, y);
                y += 18;
            }
        };

        pd.Print();
    }

    public static string GenerateQrBase64(string promptPayPhone, decimal amount)
    {
        string payload = BuildPayloadForPhoneWithAmount(promptPayPhone, amount);

        using Bitmap bmp = CreateQrBitmap(payload, 6);

        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        return Convert.ToBase64String(ms.ToArray());
    }
}
