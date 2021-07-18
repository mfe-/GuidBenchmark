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
            guidComparision.GuidComparision_EqualsGuidEdge();
            guidComparision.GuidComparision_EqualsGuidUnsafeEdge();
            return;
#endif
            BenchmarkRunner.Run<GuidComparision>();
        }
    }
}
