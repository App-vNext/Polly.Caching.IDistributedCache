using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Polly.Caching.IDistributedCache")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: CLSCompliant(false)] // Because Microsoft.Extensions.Caching.Memory.IDistributedCache, on which Polly.Caching.IDistributedCache.NetStandard11 depends, is not CLSCompliant.

[assembly: InternalsVisibleTo("Polly.Caching.IDistributedCache.NetStandard11.Specs")]