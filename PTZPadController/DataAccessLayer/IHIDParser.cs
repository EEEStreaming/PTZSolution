// Licensed under the Apache License, Version 2.0 (the "License").
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace PTZPadController
{
    public interface IHIDParser
    {
        Task ExecuteAsync(CancellationToken token = default);
        void StopAsync();
    }
}