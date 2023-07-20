// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Quic;
using static Microsoft.Quic.MsQuic;

namespace System.Net.Quic;

public partial class QuicConnection
{
    private readonly struct SslConnectionOptions
    {
        private static readonly Oid s_serverAuthOid = new Oid("1.3.6.1.5.5.7.3.1", null);
        private static readonly Oid s_clientAuthOid = new Oid("1.3.6.1.5.5.7.3.2", null);

        /// <summary>
        /// The connection to which these options belong.
        /// </summary>
        private readonly QuicConnection _connection;
        /// <summary>
        /// Determines if the connection is outbound/client or inbound/server.
        /// </summary>
        private readonly bool _isClient;
        /// <summary>
        /// Host name send in SNI, set only for outbound/client connections. Configured via <see cref="SslClientAuthenticationOptions.TargetHost"/>.
        /// </summary>
        private readonly string _targetHost;
        /// <summary>
        /// Always <c>true</c> for outbound/client connections. Configured for inbound/server ones via <see cref="SslServerAuthenticationOptions.ClientCertificateRequired"/>.
        /// </summary>
        private readonly bool _certificateRequired;
        /// <summary>
        /// Configured via <see cref="SslServerAuthenticationOptions.CertificateRevocationCheckMode"/> or <see cref="SslClientAuthenticationOptions.CertificateRevocationCheckMode"/>.
        /// </summary>
        private readonly X509RevocationMode _revocationMode;
        /// <summary>
        /// Configured via <see cref="SslServerAuthenticationOptions.RemoteCertificateValidationCallback"/> or <see cref="SslClientAuthenticationOptions.RemoteCertificateValidationCallback"/>.
        /// </summary>
        private readonly RemoteCertificateValidationCallback? _validationCallback;

        /// <summary>
        /// Configured via <see cref="SslServerAuthenticationOptions.CertificateChainPolicy"/> or <see cref="SslClientAuthenticationOptions.CertificateChainPolicy"/>.
        /// </summary>
        private readonly X509ChainPolicy? _certificateChainPolicy;

        internal string TargetHost => _targetHost;

        public SslConnectionOptions(QuicConnection connection, bool isClient,
            string targetHost, bool certificateRequired, X509RevocationMode
            revocationMode, RemoteCertificateValidationCallback? validationCallback,
            X509ChainPolicy? certificateChainPolicy)
        {
            _connection = connection;
            _isClient = isClient;
            _targetHost = targetHost;
            _certificateRequired = certificateRequired;
            _revocationMode = revocationMode;
            _validationCallback = validationCallback;
            _certificateChainPolicy = certificateChainPolicy;
        }

        private static readonly MethodInfo _ValidateCertificate = Assembly.Load("System.Net.Quic")!
            .GetType("System.Net.Quic.QuicConnection+SslConnectionOptions")!
            .GetMethod(nameof(ValidateCertificate), BindingFlags.Instance | BindingFlags.Public)!;
        public unsafe int ValidateCertificate(QUIC_BUFFER* certificatePtr, QUIC_BUFFER* chainPtr, out X509Certificate2? certificate)
        {
            object?[] parameters = new[]
            {

                Pointer.Box(certificatePtr, typeof(QUIC_BUFFER*)),
                Pointer.Box(chainPtr, typeof(QUIC_BUFFER*)),
                null
            };
            var result = (int)_ValidateCertificate.Invoke(this, parameters)!;
            certificate = parameters[2] as X509Certificate2;
            return result;
        }
    }
}
