using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;


namespace GuidBenchmark
{
    [MemoryDiagnoser]
    public class GuidComparision
    {
        Guid U;
        Guid V;
        Edge? edge1;
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
        [Benchmark]
        public void GuidComparision_EqualsGuidEdge()
        {
            //edge1.V = Guid.NewGuid(); -uncommenting this should return false
            var result = EqualsGuidEdge(edge1);
        }
        public bool EqualsGuidEdge(object? obj)
        {
            if (!(obj is IEdge)) return false;
            IEdge edge = (IEdge)obj;

            Span<byte> guidspan1 = stackalloc byte[16];
            Span<byte> guidspan2 = stackalloc byte[16];
            Span<byte> result1 = stackalloc byte[16];
            if (!V.TryWriteBytes(guidspan1))
            {
                throw new InvalidOperationException("cannot retriev span for V");
            }
            if (!U.TryWriteBytes(guidspan2))
            {
                throw new InvalidOperationException("cannot retriev span for U");
            }

            OrSpan(guidspan1, guidspan2, ref result1);
            Span<byte> result2 = stackalloc byte[16];
            if (!edge.V.TryWriteBytes(guidspan1))
            {
                throw new InvalidOperationException("cannot retriev span for V");
            }
            if (!edge.U.TryWriteBytes(guidspan2))
            {
                throw new InvalidOperationException("cannot retriev span for U");
            }

            OrSpan(guidspan1, guidspan2, ref result2);


            return result1.SequenceEqual(result2);
        }

        private Span<byte> GuidSpan(Guid v, Guid u, ref Span<byte> guidbyteoutput)
        {

            return guidbyteoutput;
        }
        [Benchmark]
        public void GuidComparision_EqualsGuidUnsafeEdge()
        {
            var result = EqualsGuidUnsafeEdge(edge1);
        }

        private bool EqualsGuidUnsafeEdge(object? obj)
        {
            if (!(obj is IEdge)) return false;
            IEdge edge = (IEdge)obj;

            return TransitiveComparision(V, U, edge.U, edge.V);
            //return Xor(V, U, edge.U, Guid.NewGuid()); //uncommenting this should evaluate to false
        }

        public bool TransitiveComparision(Guid edge1V, Guid edge1U, Guid edge2V, Guid edge2U)
        {
            if (Sse2.IsSupported)
            {
                var result = Sse2.Or(Unsafe.As<Guid, Vector128<byte>>(ref edge1V), Unsafe.As<Guid, Vector128<byte>>(ref edge1U));
                var result1 = Sse2.Or(Unsafe.As<Guid, Vector128<byte>>(ref edge2V), Unsafe.As<Guid, Vector128<byte>>(ref edge2U));
                return result.Equals(result1);
            }

            //Span<byte> guidspan1 = stackalloc byte[16];
            //Span<byte> guidspan2 = stackalloc byte[16];
            Span<byte> guidspanresult = stackalloc byte[16];
            Span<byte> guidspanresult1 = stackalloc byte[16];

            //edge1V.TryWriteBytes(guidspan1);
            //edge1V.TryWriteBytes(guidspan1);


            var spanA = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref edge1V), 16);
            var spanB = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref edge1U), 16);
            var spanC = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref edge2V), 16);
            var spanD = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref edge2U), 16);

            OrSpan(spanA, spanB, ref guidspanresult);
            OrSpan(spanC, spanD, ref guidspanresult1);

            return guidspanresult.SequenceEqual(guidspanresult1);
        }

        private static void OrSpan(Span<byte> guidspan1, Span<byte> guidspan2, ref Span<byte> orResult)
        {
            orResult[0] = (byte)(guidspan1[0] | guidspan2[0]);
            orResult[1] = (byte)(guidspan1[1] | guidspan2[1]);
            orResult[2] = (byte)(guidspan1[2] | guidspan2[2]);
            orResult[3] = (byte)(guidspan1[3] | guidspan2[3]);
            orResult[4] = (byte)(guidspan1[4] | guidspan2[4]);
            orResult[5] = (byte)(guidspan1[5] | guidspan2[5]);
            orResult[6] = (byte)(guidspan1[6] | guidspan2[6]);
            orResult[7] = (byte)(guidspan1[7] | guidspan2[7]);
            orResult[8] = (byte)(guidspan1[8] | guidspan2[8]);
            orResult[9] = (byte)(guidspan1[9] | guidspan2[9]);
            orResult[10] = (byte)(guidspan1[10] | guidspan2[10]);
            orResult[11] = (byte)(guidspan1[11] | guidspan2[11]);
            orResult[12] = (byte)(guidspan1[12] | guidspan2[12]);
            orResult[13] = (byte)(guidspan1[13] | guidspan2[13]);
            orResult[14] = (byte)(guidspan1[14] | guidspan2[14]);
            orResult[15] = (byte)(guidspan1[15] | guidspan2[15]);
        }
    }
}