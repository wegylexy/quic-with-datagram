﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace System.Net.Quic;

public sealed class QuicDatagramSendOptions

{
    public bool Priority { get; set; }

    public QuicDatagramSendStateChangedHandler? StateChanged { get; set; }
}
