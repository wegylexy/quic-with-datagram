# MsQuic Wrapper with Datagram

- Derived from .NET
- Extended with datagram
- Uses Schannel on Windows 11 by default, and OpenSSL on older versions of Windows, Linux, and OSX
  - Allows opting out of Schannel by setting the environment variable `QUIC_NO_SCHANNEL` to `true`
- Supports x64, x86, arm64, and arm on Windows; only x64 on Linux