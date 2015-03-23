using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class RabbitConnectionSettings
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
    }
}
