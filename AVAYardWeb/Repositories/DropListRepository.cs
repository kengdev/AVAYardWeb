using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AVAYardWeb.Repositories;
public class DropListRepository
{
    private DbavayardContext db;

    public DropListRepository(DbavayardContext context)
    {
        db = context;
    }

    public async Task<List<DropDownViewModel>> GetContainerSize()
    {
        var query = await (from a in db.TransContainerSizes
                    select new DropDownViewModel
                    {
                        key = a.ContainerSizeCode,
                        label = a.ContainerSizeName
                    }).ToListAsync();
        
        return query;
    }

    public async Task<List<DropDownViewModel>> GetAgent()
    {
        var query = await (from a in db.TransAgents
                    where a.IsActived == true
                    select new DropDownViewModel
                    {
                        key = a.AgentCode,
                        label = a.AgentName
                    }).ToListAsync();
        
        return query;
    }

    public async Task<List<DropDownViewModel>> GetTransportation()
    {
        var query = await (from a in db.TransTransportations
                    where a.IsActived == true 
                    select new DropDownViewModel
                    {
                        key = a.TransportationCode,
                        label = a.TransportationName
                    }).ToListAsync();

        return query;
    }

    public async Task<List<DropDownViewModel>> GetBank()
    {
        var query = await (from a in db.OrderBanks
                    select new DropDownViewModel
                    {
                        key = a.BankCode,
                        label = a.BankName
                    }).ToListAsync();
        
        return query;
    }

    public List<DropDownViewModel> GetGroup()
    {
        List<DropDownViewModel> list = new List<DropDownViewModel>();
        var item = new DropDownViewModel();
        item.key = "Admin";
        item.label = "Admin";
        list.Add(item);

        item = new DropDownViewModel();
        item.key = "Officer";
        item.label = "Officer";
        list.Add(item);

        item = new DropDownViewModel();
        item.key = "POS";
        item.label = "POS";
        list.Add(item);

        return list;
    }

    public List<DropDownViewModel> BranchType()
    {
        List<DropDownViewModel> list = new List<DropDownViewModel>();
        var item = new DropDownViewModel();
        item.key = "01";
        item.label = "สำนักงานใหญ่";
        list.Add(item);

        item = new DropDownViewModel();
        item.key = "02";
        item.label = "สาขา";
        list.Add(item);

        return list;
    }

    public List<DropDownViewModel> GetReceptType()
    {
        List<DropDownViewModel> list = new List<DropDownViewModel>();
        var item = new DropDownViewModel();
        item.key = "01";
        item.label = "เงินสด";
        list.Add(item);

        item = new DropDownViewModel();
        item.key = "02";
        item.label = "โอนเงิน";
        list.Add(item);

        item = new DropDownViewModel();
        item.key = "03";
        item.label = "เช็คธนาคาร";
        list.Add(item);

        return list;
    }

    public List<DropDownViewModel> GetMonth()
    {
        List<DropDownViewModel> list = new List<DropDownViewModel>();
        for (int i = 1; i <= 12; i++)
        {
            DropDownViewModel item = new DropDownViewModel();
            switch (i)
            {
                case 1:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "มกราคม";
                    break;
                case 2:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "กุมภาพันธ์";
                    break;
                case 3:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "มีนาคม";
                    break;
                case 4:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "เมษายน";
                    break;
                case 5:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "พฤษภาคม";
                    break;
                case 6:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "มิถุนายน";
                    break;
                case 7:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "กรกฎาคม";
                    break;
                case 8:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "สิงหาคม";
                    break;
                case 9:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "กันยายน";
                    break;
                case 10:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "ตุลาคม";
                    break;
                case 11:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "พฤศจิกายน";
                    break;
                case 12:
                    item.key = i.ToString().PadLeft(2, '0');
                    item.label = "ธันวาคม";
                    break;
            }

            list.Add(item);
        }

        return list;
    }

    public List<DropDownViewModel> GetYear()
    {
        List<DropDownViewModel> list = new List<DropDownViewModel>();
        DropDownViewModel item = new DropDownViewModel();
        string year = DateTime.Now.ToString("yyyy");
        int current_year = int.Parse(year);

        string last = DateTime.Now.AddYears(-1).ToString("yyyy");
        int last_year = int.Parse(last);

        string next = DateTime.Now.AddYears(1).ToString("yyyy");
        int next_year = int.Parse(next);

        item.key = next_year.ToString();
        item.label = next_year.ToString();
        list.Add(item);

        item = new DropDownViewModel();
        item.key = current_year.ToString();
        item.label = current_year.ToString();
        list.Add(item);

        item = new DropDownViewModel();
        item.key = last_year.ToString();
        item.label = last_year.ToString();
        list.Add(item);

        string last_2 = DateTime.Now.AddYears(-2).ToString("yyyy");
        int last_2_year = int.Parse(last_2);

        item = new DropDownViewModel();
        item.key = last_2_year.ToString();
        item.label = last_2_year.ToString();
        list.Add(item);

        return list;
    }
}