using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Polly.Caching.Distributed")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0.0")]
[assembly: CLSCompliant(false)] // Because Microsoft.Extensions.Caching.Memory.IDistributedCache, on which Polly.Caching.IDistributedCache.NetStandard11 depends, is not CLSCompliant.

[assembly: InternalsVisibleTo("Polly.Caching.Distributed.NetStandard20.Specs")]