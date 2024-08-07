﻿// <auto-generated>
// This code was partially generated by a tool.
// </auto-generated>

using System;

namespace YoutubeDLSharp.Options
{
    public partial class OptionSet
    {
        private Option<string> proxy = new Option<string>("--proxy");
        private Option<int?> socketTimeout = new Option<int?>("--socket-timeout");
        private Option<string> sourceAddress = new Option<string>("--source-address");
        private Option<string> impersonate = new Option<string>("--impersonate");
        private Option<bool> listImpersonateTargets = new Option<bool>("--list-impersonate-targets");
        private Option<bool> forceIPv4 = new Option<bool>("-4", "--force-ipv4");
        private Option<bool> forceIPv6 = new Option<bool>("-6", "--force-ipv6");
        private Option<bool> enableFileUrls = new Option<bool>("--enable-file-urls");

        /// <summary>
        /// Use the specified HTTP/HTTPS/SOCKS proxy. To
        /// enable SOCKS proxy, specify a proper scheme,
        /// e.g. socks5://user:pass@127.0.0.1:1080/.
        /// Pass in an empty string (--proxy &quot;&quot;) for
        /// direct connection
        /// </summary>
        public string Proxy { get => proxy.Value; set => proxy.Value = value; }
        /// <summary>
        /// Time to wait before giving up, in seconds
        /// </summary>
        public int? SocketTimeout { get => socketTimeout.Value; set => socketTimeout.Value = value; }
        /// <summary>
        /// Client-side IP address to bind to
        /// </summary>
        public string SourceAddress { get => sourceAddress.Value; set => sourceAddress.Value = value; }
        /// <summary>
        /// Client to impersonate for requests. E.g.
        /// chrome, chrome-110, chrome:windows-10. Pass
        /// --impersonate=&quot;&quot; to impersonate any client.
        /// Note that forcing impersonation for all
        /// requests may have a detrimental impact on
        /// download speed and stability
        /// </summary>
        public string Impersonate { get => impersonate.Value; set => impersonate.Value = value; }
        /// <summary>
        /// List available clients to impersonate.
        /// </summary>
        public bool ListImpersonateTargets { get => listImpersonateTargets.Value; set => listImpersonateTargets.Value = value; }
        /// <summary>
        /// Make all connections via IPv4
        /// </summary>
        public bool ForceIPv4 { get => forceIPv4.Value; set => forceIPv4.Value = value; }
        /// <summary>
        /// Make all connections via IPv6
        /// </summary>
        public bool ForceIPv6 { get => forceIPv6.Value; set => forceIPv6.Value = value; }
        /// <summary>
        /// Enable file:// URLs. This is disabled by
        /// default for security reasons.
        /// </summary>
        public bool EnableFileUrls { get => enableFileUrls.Value; set => enableFileUrls.Value = value; }
    }
}
