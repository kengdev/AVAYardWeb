using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public static class KShopQrTemplateEditor
{
    // ----------------------------
    // Public: update amount + crc
    // ----------------------------
    public static string UpdateAmountAndRebuild(string kshopPayloadTemplate, decimal newAmount)
    {
        if (string.IsNullOrWhiteSpace(kshopPayloadTemplate))
            throw new ArgumentException("Payload template is required.");

        if (newAmount <= 0)
            throw new ArgumentException("Amount must be > 0");

        // 1) Parse TLV (top level)
        var nodes = ParseTopLevelTlv(kshopPayloadTemplate);

        // 2) Update Tag 54 (amount) — format 0.00 (no comma)
        string amountStr = newAmount.ToString("0.00", CultureInfo.InvariantCulture);

        Upsert(nodes, "54", amountStr);

        // 3) Rebuild WITHOUT CRC first (Tag 63 must be recalculated)
        // remove existing CRC tag if present
        nodes.RemoveAll(n => n.Tag == "63");

        string withoutCrc = Build(nodes) + "6304";
        string crc = Crc16Ccitt(withoutCrc);

        // 4) Append CRC (Tag 63 length 04)
        return Build(nodes) + $"63" + "04" + crc;
    }

    // (Optional) ถ้าเก่งอยาก “ใส่เลขงาน” ลง Tag 62 ด้วย
    // ต้องรู้ก่อนว่า Subtag ไหนคือ ref ที่ต้องการ (ใน payload เก่งมี 62 24 ... 05 08 ... 07 08 ...)
    // ตัวอย่างนี้: replace Subtag "05" และ/หรือ "07" ภายใน Tag 62 (ถ้ามี)
    public static string UpdateAmountAndRefAndRebuild(string kshopPayloadTemplate, decimal newAmount, string refValue)
    {
        if (string.IsNullOrWhiteSpace(refValue))
            throw new ArgumentException("refValue is required.");

        var nodes = ParseTopLevelTlv(kshopPayloadTemplate);

        string amountStr = newAmount.ToString("0.00", CultureInfo.InvariantCulture);
        Upsert(nodes, "54", amountStr);

        // Update Tag 62 sub-tags (05 / 07) if Tag62 exists
        var tag62 = nodes.FirstOrDefault(n => n.Tag == "62");
        if (tag62 != null)
        {
            var sub = ParseTopLevelTlv(tag62.Value);

            // ตัวอย่าง: ใส่ refValue แบบตัดให้พอดีกับความยาวที่ต้องการ (แนะนำให้เป็น 8–20 ตัวอักษร/ตัวเลข)
            // ที่ payload เก่งเดิม: 05 length 08, 07 length 08
            // เราจะ “แทนค่า” แล้วปรับ length ให้ตรงอัตโนมัติ
            Upsert(sub, "05", refValue);
            Upsert(sub, "07", refValue);

            tag62.Value = Build(sub);
        }

        nodes.RemoveAll(n => n.Tag == "63");

        string withoutCrc = Build(nodes) + "6304";
        string crc = Crc16Ccitt(withoutCrc);

        return Build(nodes) + $"63" + "04" + crc;
    }

    // ----------------------------
    // TLV model
    // ----------------------------
    private class TlvNode
    {
        public string Tag { get; set; } = "";
        public string Value { get; set; } = "";
    }

    // Parse Tag(2) Length(2) Value(length)
    private static List<TlvNode> ParseTopLevelTlv(string s)
    {
        var list = new List<TlvNode>();
        int i = 0;

        while (i < s.Length)
        {
            if (i + 4 > s.Length) throw new Exception("Invalid TLV: truncated header.");

            string tag = s.Substring(i, 2);
            int len = int.Parse(s.Substring(i + 2, 2));
            i += 4;

            if (i + len > s.Length) throw new Exception($"Invalid TLV: length overflow at tag {tag}.");

            string val = s.Substring(i, len);
            i += len;

            list.Add(new TlvNode { Tag = tag, Value = val });
        }

        return list;
    }

    private static void Upsert(List<TlvNode> nodes, string tag, string newValue)
    {
        var node = nodes.FirstOrDefault(n => n.Tag == tag);
        if (node == null) nodes.Add(new TlvNode { Tag = tag, Value = newValue });
        else node.Value = newValue;
    }

    private static string Build(List<TlvNode> nodes)
    {
        // Preserve original order as much as possible: keep existing order, and any newly added tags go at end.
        var sb = new StringBuilder();
        foreach (var n in nodes)
        {
            sb.Append(n.Tag);
            sb.Append(n.Value.Length.ToString("D2"));
            sb.Append(n.Value);
        }
        return sb.ToString();
    }

    // CRC16-CCITT (poly 0x1021), init 0xFFFF
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
}
