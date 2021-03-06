﻿using System.Collections.Generic;

namespace WinBox
{
    public class MachineConfig
    {
        public int Cpus { get; set; }

        public int Vram { get; set; }

        public int Memory { get; set; }

        public List<Forwarding> Forwardings { get; set; }

        public GuestOS OperatingSystem { get; set; }

        public MachineConfig()
        {
            Cpus = 2;
            Vram = 48;
            Memory = 2048;
            Forwardings = new List<Forwarding> {
                new Forwarding("winrm", "tcp", "127.0.0.1", 55991, null, 5985)
            };
            OperatingSystem = GuestOS.Windows7_64;
        }
    }
}