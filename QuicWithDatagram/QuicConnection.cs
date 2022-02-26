// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Net.Quic.Implementations;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Quic
{
    public sealed class QuicConnection : IDisposable
    {
        private readonly QuicConnectionProvider _provider;

        internal QuicConnection(QuicConnectionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Indicates whether the QuicConnection is connected.
        /// </summary>
        public bool Connected => _provider.Connected;

        public IPEndPoint? LocalEndPoint => _provider.LocalEndPoint;

        public EndPoint RemoteEndPoint => _provider.RemoteEndPoint;

        public X509Certificate? RemoteCertificate => _provider.RemoteCertificate;

        public SslApplicationProtocol NegotiatedApplicationProtocol => _provider.NegotiatedApplicationProtocol;

        /// <summary>
        /// Connect to the remote endpoint.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) => _provider.ConnectAsync(cancellationToken);

        /// <summary>
        /// Waits for available unidirectional stream capacity to be announced by the peer. If any capacity is available, returns immediately.
        /// </summary>
        /// <returns></returns>
        public ValueTask WaitForAvailableUnidirectionalStreamsAsync(CancellationToken cancellationToken = default) => _provider.WaitForAvailableUnidirectionalStreamsAsync(cancellationToken);

        /// <summary>
        /// Waits for available bidirectional stream capacity to be announced by the peer. If any capacity is available, returns immediately.
        /// </summary>
        /// <returns></returns>
        public ValueTask WaitForAvailableBidirectionalStreamsAsync(CancellationToken cancellationToken = default) => _provider.WaitForAvailableBidirectionalStreamsAsync(cancellationToken);

        /// <summary>
        /// Create an outbound unidirectional stream.
        /// </summary>
        /// <returns></returns>
        public QuicStream OpenUnidirectionalStream() => new QuicStream(_provider.OpenUnidirectionalStream());

        /// <summary>
        /// Create an outbound bidirectional stream.
        /// </summary>
        /// <returns></returns>
        public QuicStream OpenBidirectionalStream() => new QuicStream(_provider.OpenBidirectionalStream());

        /// <summary>
        /// Accept an incoming stream.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<QuicStream> AcceptStreamAsync(CancellationToken cancellationToken = default) => new QuicStream(await _provider.AcceptStreamAsync(cancellationToken).ConfigureAwait(false));

        /// <summary>
        /// Close the connection and terminate any active streams.
        /// </summary>
        public ValueTask CloseAsync(long errorCode, CancellationToken cancellationToken = default) => _provider.CloseAsync(errorCode, cancellationToken);

        public void Dispose() => _provider.Dispose();

        /// <summary>
        /// Gets the maximum number of bidirectional streams that can be made to the peer.
        /// </summary>
        public int GetRemoteAvailableUnidirectionalStreamCount() => _provider.GetRemoteAvailableUnidirectionalStreamCount();

        /// <summary>
        /// Gets the maximum number of unidirectional streams that can be made to the peer.
        /// </summary>
        public int GetRemoteAvailableBidirectionalStreamCount() => _provider.GetRemoteAvailableBidirectionalStreamCount();

        /// <summary>
        /// Whether receiving datagram is enabled.
        /// </summary>
        public bool DatagramReceiveEnabled
        {
            get => _provider.DatagramReceiveEnabled;
            set => _provider.DatagramReceiveEnabled = value;
        }

        /// <summary>
        /// Whether sending datagram is enabled.
        /// </summary>
        public bool DatagramSendEnabled
        {
            get => _provider.DatagramSendEnabled;
            set => _provider.DatagramSendEnabled = value;
        }

        /// <summary>
        /// Max length of datagram payload to be sent.
        /// </summary>
        public int DatagramMaxSendLength => _provider.DatagramMaxSendLength;

        /// <summary>
        /// Occurs when a datagram is received.
        /// </summary>
        public event QuicDatagramReceivedEventHandler? DatagramReceived
        {
            add => _provider.DatagramReceived += value;
            remove => _provider.DatagramReceived -= value;
        }

        /// <summary>
        /// Sends a datagram.
        /// </summary>
        /// <param name="buffer">Payload of the datagram.</param>
        /// <param name="priority">Whether the datagram should be prioritized.</param>
        public Task<QuicDatagramSendingResult> SendDatagramAsync(ReadOnlyMemory<byte> buffer, bool priority = false) => _provider.SendDatagramAsync(buffer, priority);

        /// <summary>
        /// Sends a datagram.
        /// </summary>
        /// <param name="buffers">Payload of the datagram.</param>
        /// <param name="priority">Whether the datagram should be prioritized.</param>
        public Task<QuicDatagramSendingResult> SendDatagramAsync(ReadOnlySequence<byte> buffers, bool priority = false) => _provider.SendDatagramAsync(buffers, priority);
    }
}
