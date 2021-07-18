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
        //guids of edge2
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
        public bool EqualsStringIsEdge(object? obj)
        {
            if (obj is IEdge edge)
            {
                return ($"{U}{V}" == $"{edge.U}{edge.V}" || $"{U}{V}" == $"{edge.V}{edge.U}");
            }
            return false;
        }
        [Benchmark]
        public bool StringGuidComparision_EqualsStringBoxingEdge()
        {
            return EqualsString(edge1);
        }
        [Benchmark]
        public bool StringGuidComparision_EqualsStringIsEdge()
        {
            return EqualsStringIsEdge(edge1);
        }
        [Benchmark]
        public bool GuidComparision_EqualsSpanTryWriteBytesGuidEdge()
        {
            //edge1.V = Guid.NewGuid(); -uncommenting this should return false
            return EqualsSpanTryWriteBytesGuidEdge(edge1);
        }
        public bool EqualsSpanTryWriteBytesGuidEdge(object? obj)
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

        [Benchmark]
        public bool GuidComparision_TransitiveComparisionSse2IsSupported()
        {
            return EqualsTransitiveComparisionSse2IsSupported(edge1);
        }

        private bool EqualsTransitiveComparisionSse2IsSupported(object? obj)
        {
            if (!(obj is IEdge)) return false;
            IEdge edge = (IEdge)obj;

            //return TransitiveComparisionSse2IsSupported(V, U, edge.U, edge.V);
            //if (Sse2.IsSupported)
            var result = Sse2.Or(Unsafe.As<Guid, Vector128<byte>>(ref V), Unsafe.As<Guid, Vector128<byte>>(ref U));
            Guid guidU = edge.U;
            Guid guidV = edge.V;
            var result1 = Sse2.Or(Unsafe.As<Guid, Vector128<byte>>(ref guidU), Unsafe.As<Guid, Vector128<byte>>(ref guidV));
            return result.Equals(result1);

            //return Xor(V, U, edge.U, Guid.NewGuid()); //uncommenting this should evaluate to false
        }
        [Benchmark]
        public bool GuidComparision_EqualsGuidSpanEdgeMemoryMarshalCreateSpan()
        {
            return EqualsGuidSpanEdgeMemoryMarshalCreateSpan(edge1);
        }

        private bool EqualsGuidSpanEdgeMemoryMarshalCreateSpan(object? obj)
        {
            if (!(obj is IEdge)) return false;
            IEdge edge = (IEdge)obj;

            //return TransitiveComparisionMemoryMarshalCreateSpan(V, U, edge.U, edge.V);
            //Span<byte> guidspan1 = stackalloc byte[16];
            //Span<byte> guidspan2 = stackalloc byte[16];
            Span<byte> guidspanresult = stackalloc byte[16];
            Span<byte> guidspanresult1 = stackalloc byte[16];

            //edge1V.TryWriteBytes(guidspan1);
            //edge1V.TryWriteBytes(guidspan1);


            var spanA = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref V), 16);
            var spanB = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref U), 16);
            Guid guidU = edge.U;
            Guid guidV = edge.V;
            var spanC = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref guidU), 16);
            var spanD = MemoryMarshal.CreateSpan(ref Unsafe.As<Guid, byte>(ref guidV), 16);

            OrSpan(spanA, spanB, ref guidspanresult);
            OrSpan(spanC, spanD, ref guidspanresult1);

            return guidspanresult.SequenceEqual(guidspanresult1);
            //return Xor(V, U, edge.U, Guid.NewGuid()); //uncommenting this should evaluate to false
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