using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


public static class SCBShopQrCode
{
    // Payload จาก QR ร้าน (ของภาพที่คุณส่งมา)
    private const string BasePayload =
        "00020101021130650016A000000677010112011501075360001028602150140000018610790303SCB5802TH530376462200716000000000021587263040FA0";

    public static string BuildFixedAmountPayload(decimal amount)
    {
        // 1) parse TLV
        var fields = ParseTlv(BasePayload);

        // 2) เปลี่ยน Point of Initiation Method เป็น 12 (dynamic)
        Upsert(fields, "01", "12");

        // 3) ใส่ amount (Tag 54) รูปแบบ 0.00 เสมอ
        string amt = amount.ToString("0.00", CultureInfo.InvariantCulture);
        Upsert(fields, "54", amt);

        // 4) ลบ CRC เดิม (Tag 63) แล้วประกอบใหม่
        fields.RemoveAll(f => f.Id == "63");

        string withoutCrc = BuildTlv(fields);

        // 5) คำนวณ CRC16-CCITT ของ (payload + "6304")
        string toCrc = withoutCrc + "6304";
        string crc = Crc16CcittFalseHex(toCrc);

        return withoutCrc + "63" + "04" + crc;
    }

    private static void Upsert(List<Tlv> fields, string id, string value)
    {
        var idx = fields.FindIndex(f => f.Id == id);
        if (idx >= 0) fields[idx] = new Tlv(id, value);
        else
        {
            // โดยทั่วไป 54 จะอยู่หลัง 53 แต่ QR บางใบเรียงไม่เหมือนกัน
            // เราจะแทรกหลัง 53 ถ้ามี ไม่งั้นใส่ท้ายก่อน CRC
            if (id == "54")
            {
                int after53 = fields.FindIndex(f => f.Id == "53");
                if (after53 >= 0) fields.Insert(after53 + 1, new Tlv(id, value));
                else fields.Add(new Tlv(id, value));
            }
            else fields.Add(new Tlv(id, value));
        }
    }

    private static List<Tlv> ParseTlv(string payload)
    {
        var list = new List<Tlv>();
        int i = 0;
        while (i + 4 <= payload.Length)
        {
            string id = payload.Substring(i, 2);
            int len = int.Parse(payload.Substring(i + 2, 2), CultureInfo.InvariantCulture);
            i += 4;

            if (i + len > payload.Length) break;
            string value = payload.Substring(i, len);
            i += len;

            list.Add(new Tlv(id, value));

            // ถ้าเจอ CRC ก็หยุด (กันข้อมูลต่อท้ายแปลก ๆ)
            if (id == "63") break;
        }
        return list;
    }

    private static string BuildTlv(List<Tlv> fields)
    {
        var sb = new StringBuilder();
        foreach (var f in fields)
        {
            sb.Append(f.Id);
            sb.Append(f.Value.Length.ToString("00", CultureInfo.InvariantCulture));
            sb.Append(f.Value);
        }
        return sb.ToString();
    }

    // CRC16-CCITT-FALSE (poly 0x1021, init 0xFFFF, xorout 0x0000)
    private static string Crc16CcittFalseHex(string ascii)
    {
        ushort crc = 0xFFFF;
        byte[] bytes = Encoding.ASCII.GetBytes(ascii);

        foreach (byte b in bytes)
        {
            crc ^= (ushort)(b << 8);
            for (int i = 0; i < 8; i++)
            {
                bool msb = (crc & 0x8000) != 0;
                crc <<= 1;
                if (msb) crc ^= 0x1021;
            }
        }
        return crc.ToString("X4", CultureInfo.InvariantCulture);
    }

    private readonly record struct Tlv(string Id, string Value);
}
