using System;
using System.Threading;
using System.Net;
using System.Web.Script.Serialization;

namespace NiceHash_Stats
{
    class Program
    {
        static string address = null;
        static string json = null;
        static decimal btc_price;

        static void Main(string[] args)
        {

            Console.Title = "NiceHash Stats 1.2.7";
            Console.WindowWidth = 70;
            Console.WindowHeight = Console.LargestWindowHeight - 10;

            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
            Send("             NiceHash Stats 1.2.7  -  Coded by Ziga Zajc             \n", 5, ConsoleColor.Green);
            Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);

            Send("BTC Address: 3QY8DB1zfoGv3VrL36MrmwuQhj8bzYet5n (Donation)\n\n", 0, ConsoleColor.DarkGreen);

            try
            {
                GetJson("https://api.coindesk.com/v1/bpi/currentprice.json");

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);

                btc_price = (decimal) item["bpi"]["USD"]["rate_float"];
            }
            catch (Exception) {}

            Send("NiceHash Address: ", 0, ConsoleColor.Blue);
            address = Console.ReadLine();
            Console.WriteLine();

            if (!AddressLengthCheck())
            {
                Send("Invalid address", 2, ConsoleColor.Red);
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);

            Help();

            while (true)
            {
                switch (Console.ReadKey(true).KeyChar.ToString().ToLower())
                {
                    case "v":
                        Version();
                        break;
                    case "c":
                        Current_profitability();
                        break;
                    case "a":
                        Average_profitability();
                        break;
                    case "w":
                        Current_stats();
                        break;
                    case "h":
                        Help();
                        break;
                    case "m":
                        Workers();
                        break;
                    case "p":
                        Payments();
                        break;
                    default:
                        break;
                }
            }
        }

        static void GetJson(string url)
        {
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(url);
            }
        }

       static bool AddressLengthCheck()
        {
            if (address.Length == 34)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void Version()
        {
            GetJson("https://api.nicehash.com/api");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);

            Send("                                Version                              \n", 5, ConsoleColor.Magenta);
            Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);
            Send("Version: " + item["result"]["api_version"] + "\n\n", 0, ConsoleColor.Blue);
            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
        }

        static void Current_profitability()
        {
            GetJson("https://api.nicehash.com/api?method=stats.global.current");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);
            Send("                        Current Profitability                        \n", 5, ConsoleColor.Magenta);
            Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);
            for (int i = 0; i < item["result"]["stats"].Length; i++)
            {
                Send(Algorithem_Name(item["result"]["stats"][i]["algo"]) +":\n", 0, ConsoleColor.Magenta);
                Send("  id: " + item["result"]["stats"][i]["algo"] + "\n", 0, ConsoleColor.Blue);
                decimal price = 0;
                try
                {
                   price = btc_price * Convert.ToDecimal(item["result"]["stats"][i]["price"]);
                }
                catch(Exception)
                {

                }
                Send("  Price: " + item["result"]["stats"][i]["price"] + " BTC\\Day -> $" + Math.Round(price,2) + "\\Day\n", 0, ConsoleColor.Blue);
                Send("  Speed: " + Math.Round(Convert.ToDouble(item["result"]["stats"][i]["speed"]),4) + " kH\\MH\\GH\\Sol\n\n", 0, ConsoleColor.Blue);
            }
            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
        }

        static void Average_profitability()
        {
            GetJson("https://api.nicehash.com/api?method=stats.global.24h");

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);

            Send("                        Average Profitability                        \n", 5, ConsoleColor.Magenta);
            Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);
            for (int i = 0; i < item["result"]["stats"].Length; i++)
            {
                Send(Algorithem_Name(item["result"]["stats"][i]["algo"]) + ":\n", 0, ConsoleColor.Magenta);
                Send("  id: " + item["result"]["stats"][i]["algo"] + "\n", 0, ConsoleColor.Blue);
                decimal price = 0;
                try
                {
                    price = btc_price * Convert.ToDecimal(item["result"]["stats"][i]["price"]);
                }
                catch (Exception)
                {

                }
                Send("  Price: " + item["result"]["stats"][i]["price"] + " BTC\\Day -> $" + Math.Round(price, 2) + "\\Day\n", 0, ConsoleColor.Blue);
                Send("  Speed: " + Math.Round(Convert.ToDouble(item["result"]["stats"][i]["speed"]),4) + " kH\\MH\\GH\\Sol\n\n", 0, ConsoleColor.Blue);
            }
            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
        }

        static void Current_stats()
        {
            GetJson("https://api.nicehash.com/api?method=stats.provider&addr="+address);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);

            try
            {
                Send("                                Wallet                               \n", 5, ConsoleColor.Magenta);
                Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);

                decimal sum_btc = 0;
                decimal sum_price = 0;

                for (int i = 0; i < item["result"]["stats"].Length; i++)
                {

                    Send(Algorithem_Name(item["result"]["stats"][i]["algo"]) + ":\n", 0, ConsoleColor.Magenta);
                    Send("  id: " + item["result"]["stats"][i]["algo"] + "\n", 0, ConsoleColor.Blue);

                    decimal price = 0;
                    decimal btc = Convert.ToDecimal(item["result"]["stats"][i]["balance"]);

                    try
                    {
                        price = btc_price * Convert.ToDecimal(item["result"]["stats"][i]["balance"]);
                    }
                    catch (Exception)
                    {

                    }

                    sum_price += price;
                    sum_btc += btc;

                    Send("  Unpaid Balance: " + btc + " BTC -> $" + Math.Round(price,2) + "\n", 0, ConsoleColor.Green);
                    Send("  Accepted Speed: " + item["result"]["stats"][i]["accepted_speed"] + "\n", 0, ConsoleColor.Blue);
                    Send("  Rejected Speed: " + item["result"]["stats"][i]["rejected_speed"] + "\n\n", 0, ConsoleColor.Red);
                }

                Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
                Send("  Unpaid Balance: " + sum_btc + " BTC -> $" + Math.Round(sum_price,2) + "\n", 0, ConsoleColor.DarkGreen);

                Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
            }
            catch (Exception)
            {
                Send("Please make sure that Nicehash Address is correct!", 2, ConsoleColor.Red);
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }

        static void Workers()
        {
            GetJson("https://api.nicehash.com/api?method=stats.provider.workers&addr=" + address);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);

            try
            {
                Send("                                Miners                              \n", 5, ConsoleColor.Magenta);
                Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);
                for (int i = 0; i < item["result"]["workers"].Length; i++)
                {
                    try
                    {
                        Send(item["result"]["workers"][i][0] + ":\n", 0, ConsoleColor.Magenta);
                    }
                    catch (Exception)
                    {
                        Send("unknown:\n", 0, ConsoleColor.Magenta);
                    }

                    try
                    {
                        Send("  Mining: " + Algorithem_Name(item["result"]["workers"][i][6]) + "\n", 0, ConsoleColor.DarkYellow);
                    }
                    catch (Exception)
                    {
                        Send("  Mining: " + "unknown" + "\n", 0, ConsoleColor.Yellow);
                    }

                    try
                    {
                        switch (item["result"]["workers"][i][5])
                        {
                            case 0:
                                Send("  Location: Europe - Amsterdam\n", 0, ConsoleColor.DarkCyan);
                                break;
                            case 1:
                                Send("  Location: USA - San Jose\n", 0, ConsoleColor.DarkCyan);
                                break;
                            case 2:
                                Send("  Location: China - Hong Kong\n", 0, ConsoleColor.DarkCyan);
                                break;
                            case 3:
                                Send("  Location: Japan - Tokyo\n", 0, ConsoleColor.DarkCyan);
                                break;
                            case 4:
                                Send("  Location: India - Chennai\n", 0, ConsoleColor.DarkCyan);
                                break;
                            case 5:
                                Send("  Location: Brazil - Sao Paulo\n", 0, ConsoleColor.DarkCyan);
                                break;
                            default:
                                Send("  Location: Unknown\n", 0, ConsoleColor.DarkCyan);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Send("  Location: Unknown\n", 0, ConsoleColor.DarkCyan);
                    }

                    try
                    {
                        Send("  Time connected: " + item["result"]["workers"][i][2] + " min. -> " + Math.Round(Convert.ToDouble(item["result"]["workers"][i][2]) / 60,2) + " hours \n", 0, ConsoleColor.Yellow);
                    }
                    catch (Exception)
                    {
                        Send("  Time connected: " + "unknown" + " min.\n", 0, ConsoleColor.Yellow);
                    }

                    try
                    {
                        Send("  Accepted Shares: " + item["result"]["workers"][i][1]["a"] + " kH\\MH\\GH\\Sol\n", 0, ConsoleColor.Green);
                    }
                    catch (Exception)
                    {
                        Send("  Accepted Shares: " + "unknown" + " kH\\MH\\GH\\Sol\n", 0, ConsoleColor.Green);
                    }

                    try
                    {
                        Send("  Rejected Shares: " + item["result"]["workers"][i][1]["rs"] + " kH\\MH\\GH\\Sol\n", 0, ConsoleColor.Red);
                    }
                    catch (Exception)
                    {
                        Send("  Rejected Shares: " + "0" + " kH\\MH\\GH\\Sol\n", 0, ConsoleColor.Red);
                    }

                    try
                    {
                        if (item["result"]["workers"][i][3] == 1)
                        {
                            Send("  XNSUB: Enabled\n", 0, ConsoleColor.Cyan);
                        }
                        else
                        {
                            Send("  XNSUB: Disabled\n", 0, ConsoleColor.Cyan);
                        }
                    }
                    catch (Exception)
                    {
                        Send("  XNSUB: unknown" + "\n", 0, ConsoleColor.Yellow);
                    }

                    try
                    {
                        Send("  Difficulty: " + item["result"]["workers"][i][4] + "\n\n", 0, ConsoleColor.Gray);
                    }
                    catch (Exception)
                    {
                        Send("  Difficulty: " + "unknown" + "\n\n", 0, ConsoleColor.Gray);
                    }

                }
                Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
            }
            catch (Exception)
            {
                Send("Please make sure that Nicehash Address is correct!", 2, ConsoleColor.Red);
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }

        static void Payments()
        {
            GetJson("https://api.nicehash.com/api?method=stats.provider&addr=" + address);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);

            try
            {
                Send("                               Payments                              \n", 5, ConsoleColor.Magenta);
                Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);

                decimal sum_btc_amount = 0;
                decimal sum_btc_fee = 0;
                decimal sum_amount = 0;
                decimal sum_fee = 0;

                for (int i = 0; i < item["result"]["payments"].Length; i++)
                {
                    Send(item["result"]["payments"][i]["time"] + "\n\n", 0, ConsoleColor.Magenta);

                    decimal btc_amount = Convert.ToDecimal(item["result"]["payments"][i]["amount"]);
                    decimal btc_fee = Convert.ToDecimal(item["result"]["payments"][i]["fee"]);

                    decimal amount = 0;
                    try
                    {
                        amount = btc_price * btc_amount;
                    }
                    catch (Exception){}

                    decimal fee = 0;
                    try
                    {
                        fee = btc_price * btc_fee;
                    }
                    catch (Exception){}

                    sum_btc_amount += btc_amount;
                    sum_btc_fee += btc_fee;
                    sum_amount += amount;
                    sum_fee += fee;

                    Send("  Amount:    " + btc_amount + " BTC -> $" + Math.Round(amount,2) + "\n", 0, ConsoleColor.Green);
                    Send("  Fee:       " + btc_fee + " BTC -> $" + Math.Round(fee, 2) + "\n", 0, ConsoleColor.Red);
                    Send("   ----------------------------------\n", 5, ConsoleColor.Gray);
                    Send("  Received:  " + (btc_amount - btc_fee) + " BTC -> $" + Math.Round(amount-fee,2) + "\n\n", 0, ConsoleColor.Blue);
                }

                Send("Money earned:\n\n", 0, ConsoleColor.Magenta);

                Send("  Amount:    " + sum_btc_amount + " BTC -> $" + Math.Round(sum_amount, 2) + "\n", 0, ConsoleColor.Green);
                Send("  Fee:       " + sum_btc_fee + " BTC -> $" + Math.Round(sum_fee, 2) + "\n", 0, ConsoleColor.Red);
                Send("   ----------------------------------\n", 5, ConsoleColor.Gray);
                Send("  Received:  " + (sum_btc_amount - sum_btc_fee) + " BTC -> $" + Math.Round(sum_amount - sum_fee, 2) + "\n\n", 0, ConsoleColor.Blue);


                Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
            }
            catch (Exception)
            {
                Send("Please make sure that Nicehash Address is correct!", 2, ConsoleColor.Red);
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }

        static void Help()
        {
            Send("                                Help                                 \n", 5, ConsoleColor.Magenta);
            Send(" --------------------------------------------------------------------\n\n", 5, ConsoleColor.Blue);

            Send("V - API Version\n", 0, ConsoleColor.Blue);
            Send("C - current profitability and hashing speed for all algorithms\n", 0, ConsoleColor.Green);
            Send("A - average profitability and hashing speed for all algorithms\n", 0, ConsoleColor.Blue);
            Send("W - wallet - current stats from your wallet for all algorithms\n", 0, ConsoleColor.Green);
            Send("M - get detailed stats from your's miners (rigs)\n", 0, ConsoleColor.Blue);
            Send("P - get payments\n", 0, ConsoleColor.Green);
            Send("H - for help\n\n", 0, ConsoleColor.Blue);

            Send(" --------------------------------------------------------------------\n", 5, ConsoleColor.Blue);
        }

        static string Algorithem_Name(int algo)
        {
            switch (algo)
            {
                case 0:
                    return "Scrypt";
                case 1:
                    return "SHA256";
                case 2:
                    return "ScryptNf";
                case 3:
                    return "X11";
                case 4:
                    return "X13";
                case 5:
                    return "Keccak";
                case 6:
                    return "X15";
                case 7:
                    return "Nist5";
                case 8:
                    return "NeoScrypt";
                case 9:
                    return "Lyra2RE";
                case 10:
                    return "WhirlpoolX";
                case 11:
                    return "Qubit";
                case 12:
                    return "Quark";
                case 13:
                    return "Axiom";
                case 14:
                    return "Lyra2REv2";
                case 15:
                    return "ScryptJaneNf16";
                case 16:
                    return "Blake256r8";
                case 17:
                    return "Blake256r14";
                case 18:
                    return "Blake256r8vnl";
                case 19:
                    return "Hodl";
                case 20:
                    return "DaggerHashimoto";
                case 21:
                    return "Decred";
                case 22:
                    return "CryptoNight";
                case 23:
                    return "Lbry";
                case 24:
                    return "Equihash";
                case 25:
                    return "Pascal";
                case 26:
                    return "X11Gost";
                case 27:
                    return "Sia";
                case 28:
                    return "Blake2s";
                case 29:
                    return "Skunk";
                case 30:
                    return "CryptoNightV7";
                case 31:
                    return "CryptoNightHeavy";
                case 32:
                    return "Lyra2Z";
                case 33:
                    return "X16R";
                case 34:
                    return "CryptoNightV8";
                case 35:
                    return "SHA256AsicBoost";
                default:
                    return null;
            }
        }

        static void Send(string message, int type, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            switch (type)
            {
                case 0:
                    Console.Write(" " + message);
                    break;
                case 1:
                    Console.Write(" [Warning] " + message);
                    break;
                case 2:
                    Console.Write(" --------------------------------------------------------------------\n");
                    Console.WriteLine(" [Error] " + message);
                    Console.Write(" --------------------------------------------------------------------\n");
                    break;
                default:
                    Console.Write("" + message);
                    break;
            }
        }
    }
}
