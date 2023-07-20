// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Quic;
using static Microsoft.Quic.MsQuic;

namespace System.Net.Quic;

internal static class MsQuicHelpers
{
    internal static bool TryParse(this EndPoint endPoint, out string? host, out IPAddress? address, out int port)
    {
        if (endPoint is DnsEndPoint dnsEndPoint)
        {
            host = IPAddress.TryParse(dnsEndPoint.Host, out address) ? null : dnsEndPoint.Host;
            port = dnsEndPoint.Port;
            return true;
        }

        if (endPoint is IPEndPoint ipEndPoint)
        {
            host = null;
            address = ipEndPoint.Address;
            port = ipEndPoint.Port;
            return true;
        }

        host = default;
        address = default;
        port = default;
        return false;
    }

    private static readonly MethodInfo _ToIPEndPoint = Assembly.Load("System.Net.Quic")!
        .GetType("System.Net.Quic.MsQuicHelpers")!
        .GetMethod(nameof(ToIPEndPoint), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly Type _QuicAddr = _ToIPEndPoint.GetParameters()[0].ParameterType.GetElementType()!;
    private static readonly MethodInfo _Unsafe_BitCast_QuicAddr = typeof(Unsafe).GetMethod(nameof(Unsafe.BitCast))!
        .MakeGenericMethod(typeof(QuicAddr), _QuicAddr);
    internal static unsafe IPEndPoint ToIPEndPoint(this ref QuicAddr quicAddress, AddressFamily? addressFamilyOverride = null)
    {
        var cast = _Unsafe_BitCast_QuicAddr.Invoke(null, new object[] { quicAddress });
        return (IPEndPoint)_ToIPEndPoint.Invoke(null, new object?[] { cast, addressFamilyOverride })!;
    }

    private static readonly MethodInfo _IPEndPointExtensions_Serialize = Assembly.Load("System.Net.Quic")!
        .GetType("System.Net.Sockets.IPEndPointExtensions")!
        .GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static)!;
    internal static unsafe QuicAddr ToQuicAddr(this IPEndPoint ipEndPoint)
    {
        // TODO: is the layout same for SocketAddress.Buffer and QuicAddr on all platforms?
        QuicAddr result = default;
        Span<byte> rawAddress = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref result, 1));

        var address = _IPEndPointExtensions_Serialize.Invoke(null, new object[] { ipEndPoint })!;
        var type = address.GetType();
        var size = (int)type.GetProperty("Size", BindingFlags.Public | BindingFlags.Instance)!.GetValue(address)!;
        Debug.Assert(size <= rawAddress.Length);

        var buffer = (byte[])type.GetField("Buffer", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(address)!;
        buffer.AsSpan(0, size).CopyTo(rawAddress);
        return result;
    }

    internal static unsafe T GetMsQuicParameter<T>(MsQuicSafeHandle handle, uint parameter)
        where T : unmanaged
    {
        T value;
        uint length = (uint)sizeof(T);

        int status = MsQuicApi.Api.GetParam(
            handle,
            parameter,
            &length,
            (byte*)&value);

        if (StatusFailed(status))
        {
            ThrowHelper.ThrowMsQuicException(status, $"GetParam({handle}, {parameter}) failed");
        }

        return value;
    }

    internal static unsafe void SetMsQuicParameter<T>(MsQuicSafeHandle handle, uint parameter, T value)
        where T : unmanaged
    {
        int status = MsQuicApi.Api.SetParam(
            handle,
            parameter,
            (uint)sizeof(T),
            (byte*)&value);

        if (StatusFailed(status))
        {
            ThrowHelper.ThrowMsQuicException(status, $"SetParam({handle}, {parameter}) failed");
        }
    }
}
