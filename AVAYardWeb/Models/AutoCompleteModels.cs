namespace AVAYardWeb.Models;
public class DefaultAutoCompleteViewModel
{
    public string code { get; set; }
    public string key { get; set; }
    public string value { get; set; }
    public string title { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public string attr1 { get; set; }
    public string attr2 { get; set; }
}

public class ContainerAutoCompleteViewModel
{
    public string code { get; set; }
    public string container { get; set; }
    public string size { get; set; }
    public decimal service_charge { get; set; }
    public decimal empty_cost { get; set; }
    public decimal loaded_cost { get; set; }
}