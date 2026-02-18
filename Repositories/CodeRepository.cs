using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AVAYardWeb.Repositories;
public class CodeRepository
{
    private DbavayardContext db;

    public CodeRepository(DbavayardContext context)
    {
        db = context;
    }

    public async Task<string> GetPaymentCode()
    {
        string code = "";
        string prefix_year = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM");
        string prefix = prefix_year + "P";

        var query = await db.GenerateCodes
            .FromSqlRaw("SELECT gen_code, gen_type, create_date FROM generate_code WITH (UPDLOCK, ROWLOCK) WHERE gen_type = 'Container Payment'")
            .AsNoTracking()
            .OrderByDescending(o => o.GenCode)
            .FirstOrDefaultAsync();

        if (query != null)
        {
            string id = query.GenCode;
            string preinitial = id.Substring(0, 5);
            string initial = id.Substring(5, 4);
            if (prefix == preinitial)
            {
                id = (int.Parse(initial) + 1).ToString();
                id = id.PadLeft(4, '0');

                code = prefix + id;
            }
            else
            {
                code = prefix + "0001";
            }
        }
        else
        {
            code = prefix + "0001";
        }

        GenerateCode genData = new GenerateCode();
        genData.GenCode = code;
        genData.GenType = "Container Payment";
        genData.CreateDate = DateTime.Now;

        db.GenerateCodes.Add(genData);
        await db.SaveChangesAsync();
        return code;
    }

    public async Task<string> GetReceiptCode(int round)
    {
        string code = "";
        string prefix_type = "AVA";
        string prefix_year = DateTime.Now.ToString("yy") + "R" + DateTime.Now.ToString("MM");
        string prefix = prefix_type + prefix_year + "Y";

        var query = await db.GenerateCodes
            .FromSqlRaw("SELECT gen_code, gen_type, create_date FROM generate_code WITH (UPDLOCK, ROWLOCK) WHERE gen_type = 'Container Receipt'")
            .AsNoTracking()
            .OrderByDescending(o => o.GenCode)
            .FirstOrDefaultAsync();

        if (query != null)
        {
            string id = query.GenCode;
            string preinitial = id.Substring(0, 9);
            string initial = id.Substring(9, 4);
            if (prefix == preinitial)
            {
                id = (int.Parse(initial) + round).ToString();
                id = id.PadLeft(4, '0');

                code = prefix + id;
            }
            else
            {
                code = prefix + round.ToString().PadLeft(4, '0');
            }
        }
        else
        {
            code = prefix + round.ToString().PadLeft(4, '0');
        }

        GenerateCode genData = new GenerateCode();
        genData.GenCode = code;
        genData.GenType = "Container Receipt";
        genData.CreateDate = DateTime.Now;

        db.GenerateCodes.Add(genData);
        await db.SaveChangesAsync();
        return code;
    }

    public async Task<string> GetTransactionCode()
    {
        string code = "";
        string prefix = DateTime.Now.ToString("yyMM") + "T";
        var query = await db.OrderTransactions.OrderByDescending(o => o.TransactionCode).FirstOrDefaultAsync();
        if (query != null)
        {
            string id = query.TransactionCode;
            string preinitial = id.Substring(0, 5);
            string initial = id.Substring(5, 4);
            if (prefix == preinitial)
            {
                id = (int.Parse(initial) + 1).ToString();
                id = id.PadLeft(4, '0');

                code = prefix + id;
            }
            else
            {
                code = prefix + "0001";
            }
        }
        else
        {
            code = prefix + "0001";
        }

        return code;
    }
    
    public async Task<string> GetAgentCode()
    {
        string code = "";
        var query = await db.TransAgents.OrderByDescending(o => o.AgentCode).FirstOrDefaultAsync();
        if (query != null)
        {
            string id = query.AgentCode;
            id = (int.Parse(id) + 1).ToString();
            id = id.PadLeft(3, '0');
        }
        else
        {
            code = "001";
        }

        return code;
    }

    public async Task<string> GetContainerTypeCode()
    {
        string code = "";
        var query = await db.TransContainerSizes.OrderByDescending(o => o.ContainerSizeCode).FirstOrDefaultAsync();
        if (query != null)
        {
            string id = query.ContainerSizeCode;
            id = (int.Parse(id) + 1).ToString();
            id = id.PadLeft(2, '0');
        }
        else
        {
            code = "01";
        }

        return code;
    }

    public async Task<string> GetTransportationCode()
    {
        string code = "";
        string prefix = DateTime.Now.ToString("yy");
        var query = await db.TransTransportations.OrderByDescending(o => o.TransportationCode).FirstOrDefaultAsync();
        if (query != null)
        {
            string id = query.TransportationCode;
            string preinitial = id.Substring(0, 2);
            string initial = id.Substring(2, 3);
            if (prefix == preinitial)
            {
                id = (int.Parse(initial) + 1).ToString();
                id = id.PadLeft(3, '0');

                code = prefix + id;
            }
            else
            {
                code = prefix + "001";
            }
        }
        else
        {
            code = prefix + "001";
        }

        return code;
    }

    public async Task<string> GetOrderContainerCode()
    {
        string code = "";
        string prefix = "AVA" + DateTime.Now.ToString("yyMM") + "Y";
        var query = await db.OrderContainers.OrderByDescending(o => o.OrderCode).FirstOrDefaultAsync();
        if (query != null)
        {
            string id = query.OrderCode;
            string preinitial = id.Substring(0, 8);
            string initial = id.Substring(8, 4);
            if (prefix == preinitial)
            {
                id = (int.Parse(initial) + 1).ToString();
                id = id.PadLeft(4, '0');

                code = prefix + id;
            }
            else
            {
                code = prefix + "0001";
            }
        }
        else
        {
            code = prefix + "0010";
        }

        return code;
    }
}