using System;
using BenchmarkDotNet.Attributes;


namespace GuidBenchmark
{
    [MemoryDiagnoser]
    public class GuidComparision
    {
        Guid U;
        Guid V;
        Edge edge1;
        public void GlobalSetup()
        {
            U = Guid.NewGuid();
            V = Guid.NewGuid();
            edge1 = new Edge() { U = this.V, V = this.U };
            // V = Guid.NewGuid();
        }
        public bool EqualsString(object? obj)
        {
            if (!(obj is IEdge)) return false;
            IEdge edge = (IEdge)obj;
            return ($"{U}{V}" == $"{edge.U}{edge.V}" || $"{U}{V}" == $"{edge.V}{edge.U}");
        }
        [Benchmark]
        public void StringGuidComparision_EqualsString()
        {
            var result = EqualsString(edge1);
        }
        public bool EqualsStringIsEdge(object? obj)
        {
            if (obj is IEdge edge)
            {
                return ($"{U}{V}" == $"{edge.U}{edge.V}" || $"{U}{V}" == $"{edge.V}{edge.U}");
            }
            return false;
        }
        [Benchmark]
        public void StringGuidComparision_EqualsStringIsEdge()
        {
            var result = EqualsStringIsEdge(edge1);
        }
        public bool EqualsGuidEdge(object? obj)
        {
            // if (!(obj is IEdge)) return false;
            // IEdge edge = (IEdge)obj;
            return true;
        }
        [Benchmark]
        public void GuidComparision_EqualsGuidEdge()
        {
            var result = EqualsGuidEdge(edge1);
        }
    }
}