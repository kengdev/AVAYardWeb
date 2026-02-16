namespace AVAYardWeb.Models;
public class ResponseViewModel
{
    public bool result { get; set; }
    public string code { get; set; }
    public string resultMessage { get; set; }
    public string notifMessage { get; set; }
    public Exception errorException { get; set; }
}

public class ErrorUploadModel
{
    public bool result { get; set; }

    public string message { get; set; } = "บันทึกข้อมูลเรียบร้อยแล้ว";
}