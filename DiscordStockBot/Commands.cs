
using DiscordRPC;
using DiscordStockBot;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Modes;
using System.Linq;
using Renci.SshNet.Messages;
using ThreeFourteen.Finnhub.Client;

namespace Discord_Bot.src.Modules.Commands
{
    public class Commands : BaseCommandModule
    {

        [Command("help")]
        public async Task StockHelp(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("```\n" + Bot.prefix + string.Join(Bot.prefix, 
                new string[] { 
                    "counter [@user]\n", 
                    "counter++\n",
                    "dict add <word>\n", 
                    "dict rm <word>\n", 
                    "dict list\n", 
                    "dict list [n]\n",
                    "help\n",
                }) + "```");
        }

        [Command("stocks")]
        [Aliases("stonks")]
        [Description("Lists of symbols")]
        public async Task StockBot(CommandContext ctx)
        {
            try
            {
                string[] args = ctx.Message.Content.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                int pageSize = 300;
                var finn = new FinnhubClient(Bot.finnKey);
                var symbols = await finn.Stock.GetSymbols("US");
                List<string> list = new List<string>();
                foreach (var symbol in symbols)
                {
                   list.Add(symbol.DisplaySymbol);
                }
                int page = 0;
                if (args.Length > 1 && int.TryParse(args[1], out int x))
                {
                    page = x - 1;
                }
                string message = " US stocks : Page " + (page + 1).ToString() + " of " + ((list.Count() / pageSize) + 1).ToString() + "\n```\n";
                if (page + 1 > (list.Count() / 25) + 1)
                {
                    return;
                }
                for (int i = page * pageSize; i < (page * pageSize) + pageSize; i++)
                {
                    if (i < list.Count)
                    {
                        message += list[i] + ", ";
                    }
                }
                message += "```";
                await ctx.Channel.SendMessageAsync(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Command("stock")]
        [Aliases("stonk")]
        [Description("Lists of symbols")]
        public async Task Stock(CommandContext ctx)
        {
            try
            {
                if (ctx.Message.Author.Username == "AlperenT")
                {
                    await ctx.Channel.SendMessageAsync("lmao");
                    return;
                } 
                string[] args = ctx.Message.Content.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(ctx.Message.Author.Username + ": " + ctx.Message.Content);
                int pageSize = 300;
                var finn = new FinnhubClient(Bot.finnKey);
                if (args.Length > 1 && args[1].Length < 5)
                {
                    var quote = await finn.Stock.GetQuote(args[1]);
                    var company = await finn.Stock.GetCompany(args[1]);

                    string message = args[1] + " - " + company.Name + " - " + company.Weburl + "\n" +
                        "Current Price: " + quote.Current.ToString("C") +
                        " - High: " + quote.High.ToString("C") +
                        " - Low:  " + quote.Current.ToString("C");
                    await ctx.Channel.SendMessageAsync(message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
