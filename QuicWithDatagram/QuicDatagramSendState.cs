﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net.Quic;

public enum QuicDatagramSendState
{
    Unknown,
    Sent,
    LostSuspect,
    LostDiscarded,
    Acknowledged,
    AcknowledgedSpurious,
    Canceled
}
