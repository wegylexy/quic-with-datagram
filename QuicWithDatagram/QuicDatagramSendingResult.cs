// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net.Quic
{
    public sealed class QuicDatagramSendingResult
    {
        public Task Completion { get; }

        public Task LostSuspect { get; }

        internal QuicDatagramSendingResult(Task completion, Task lostSuspect)
        {
            Completion = completion;
            LostSuspect = lostSuspect;
        }
    }
}
