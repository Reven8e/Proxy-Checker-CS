using Leaf.xNet;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using HttpStatusCode = Leaf.xNet.HttpStatusCode;
using System.Diagnostics;


namespace Proxy_Checker
{
    class Checker
    {
        private static int Good = 0;
        private static int Bad = 0;
        private static int ProxyInt = 0;
        private static int THR = 0;
        private static int CPM = 0;
        private static List<string> Proxies = new List<string>();
        private static StreamReader file = new StreamReader("proxies.txt");
        private static StreamWriter Good_Proxies = new StreamWriter("good_proxies.txt");
        private static int Option = 0;


        public static void Mainer()
        {
            Logo();
            Console.Write("[1] HTTP\n[2] SOCKS4\n[3] SOCKS5\n\n: ");
            Option = Int32.Parse(Console.ReadLine());

            Checker.Loader();

            Thread thr = new Thread(Checker.Printer);
            thr.Start();

            while (Checker.ProxyInt < Proxies.Count - 1)
            {
                THR = Process.GetCurrentProcess().Threads.Count;
                if (THR <= 150)
                {
                    string proxy = Proxies[ProxyInt];
                    Thread t = new Thread(() => Keker(proxy));
                    t.Start();
                    ProxyInt++;
                }
            }
            while (true)
            {
                int toclose = Process.GetCurrentProcess().Threads.Count;
                if (toclose < 50)
                {
                    Good_Proxies.Close();
                    Environment.Exit(-1);
                }
            }
        }

        private static void Loader()
        {
            while (file.Peek() >= 0)
            {
                Checker.Proxies.Add(file.ReadLine());
            }

            try
            {
                string check = Proxies[0];
            }
            catch
            {
                Console.WriteLine("\n\nPlease Add Proxies!");
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }
        }

        private static void Printer()
        {
            while (true)
            {
                Console.Clear();
                Logo();
                Console.WriteLine("Good Proxies: " + Good);
                Console.WriteLine("Bad Proxies: " + Bad);
                Console.WriteLine("CPM: " + (CPM * 60));
                Console.WriteLine("Threads: " + THR);
                CPM = 0;
                Thread.Sleep(1000);
            }
        }

        private static void Keker(string proxy)
        {
            try
            {
                using (HttpRequest requests = new HttpRequest())
                {
                    switch (Checker.Option)
                    {
                        case 1:
                            requests.Proxy = (ProxyClient)HttpProxyClient.Parse(proxy);
                            break;
                        case 2:
                            requests.Proxy = (ProxyClient)Socks4ProxyClient.Parse(proxy);
                            break;
                        case 3:
                            requests.Proxy = (ProxyClient)Socks5ProxyClient.Parse(proxy);
                            break;
                        default:
                            Console.WriteLine("Invaild proxy type, Press a key to select again.");
                            Thread.Sleep(5000);
                            Environment.Exit(-1);
                            break;
                    }
                    requests.IgnoreProtocolErrors = true;
                    var request = requests.Get("https://www.google.com");
                    string[] cut = proxy.Split(':');
                    if (request.StatusCode == HttpStatusCode.OK)
                    {
                        Good++;
                        Good_Proxies.WriteLine(proxy);
                        CPM++;
                    }
                    else
                    {
                        Bad++;
                        CPM++;
                    }
                }
            }
            catch (Exception e)
            {
                Bad++;
                CPM++;
            }
        }

        private static void Logo()
        {
            Console.WriteLine(" ██▓███  ██▀███  ▒█████ ▒██   ██▓██   ██▓    ▄████▄  ██░ ██▓█████ ▄████▄  ██ ▄█▓█████ ██▀███  ");
            Console.WriteLine("▓██░  ██▓██ ▒ ██▒██▒  ██▒▒ █ █ ▒░▒██  ██▒   ▒██▀ ▀█ ▓██░ ██▓█   ▀▒██▀ ▀█  ██▄█▒▓█   ▀▓██ ▒ ██▒");
            Console.WriteLine("▓██░ ██▓▓██ ░▄█ ▒██░  ██░░  █   ░ ▒██ ██░   ▒▓█    ▄▒██▀▀██▒███  ▒▓█    ▄▓███▄░▒███  ▓██ ░▄█ ▒");
            Console.WriteLine("▒██▄█▓▒ ▒██▀▀█▄ ▒██   ██░░ █ █ ▒  ░ ▐██▓░   ▒▓▓▄ ▄██░▓█ ░██▒▓█  ▄▒▓▓▄ ▄██▓██ █▄▒▓█  ▄▒██▀▀█▄  ");
            Console.WriteLine("▒██▒ ░  ░██▓ ▒██░ ████▓▒▒██▒ ▒██▒ ░ ██▒▓░   ▒ ▓███▀ ░▓█▒░██░▒████▒ ▓███▀ ▒██▒ █░▒████░██▓ ▒██▒");
            Console.WriteLine("▒▓▒░ ░  ░ ▒▓ ░▒▓░ ▒░▒░▒░▒▒ ░ ░▓ ░  ██▒▒▒    ░ ░▒ ▒  ░▒ ░░▒░░░ ▒░ ░ ░▒ ▒  ▒ ▒▒ ▓░░ ▒░ ░ ▒▓ ░▒▓░");
            Console.WriteLine("░▒ ░      ░▒ ░ ▒░ ░ ▒ ▒░░░   ░▒ ░▓██ ░▒░      ░  ▒   ▒ ░▒░ ░░ ░  ░ ░  ▒  ░ ░▒ ▒░░ ░  ░ ░▒ ░ ▒░");
            Console.WriteLine("░░        ░░   ░░ ░ ░ ▒  ░    ░  ▒ ▒ ░░     ░        ░  ░░ ░  ░  ░       ░ ░░ ░   ░    ░░   ░ ");
            Console.WriteLine("           ░        ░ ░  ░    ░  ░ ░        ░ ░      ░  ░  ░  ░  ░ ░     ░  ░     ░  ░  ░     ");
            Console.WriteLine("                                 ░ ░        ░                    ░                            ");
            Console.WriteLine("");
        }
    }
}
