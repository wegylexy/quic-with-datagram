# MsQuic Wrapper with Datagram

- Derived from `System.Net.Quic`
- Extended with datagram
- Uses Schannel on Windows 11 when TLS 1.3 is not disabled, and OpenSSL otherwise
- Supports x64, x86, arm64, and arm on Windows; only x64 on Linux

## Reference

When preview features are enabled in .NET 7, use an alias to resolve name conflicts:

```xml
<PackageReference Include="FlyByWireless.Quic" Version="$(Version)" Aliases="FlyByWireless" />
```

```cs
extern alias FlyByWireless;
using FlyByWireless.System.Net.Quic;
// using System.Net.Quic;
```