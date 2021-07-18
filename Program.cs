using System;
using BenchmarkDotNet.Running;

namespace GuidBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
#if DEBUG
            var guidComparision = new GuidComparision();
            guidComparision.GlobalSetup();
            guidComparision.GuidComparision_EqualsSpanTryWriteBytesGuidEdge();
            guidComparision.GuidComparision_EqualsGuidSpanEdgeMemoryMarshalCreateSpan();
            return;
#endif
            BenchmarkRunner.Run<GuidComparision>();
        }
    }
}
