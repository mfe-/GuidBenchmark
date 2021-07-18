# GuidBenchmark

Goal: Order of `Guid` comparison should be negligible. Background: vertices and edges from a graph. 

In an undirected graph an edge1 from U (GuidU)->V (GuidV) should be equal to an edge2 from V (GuidV)->U (GuidU). (U and V represent a vertex using a `Guid` )

In order to treat edge1 and edge2 equally currently there are the following ways to achieve that:

## Simple way:
Check equality comparing a concated String and checking for both U->V and V->U 

`return ($"{U}{V}" == $"{edge.U}{edge.V}" || $"{U}{V}" == $"{edge.V}{edge.U}")`;

## Using the Bit operation
The Guids of edge1.U and edge1.V or edge2.V and edge2.U will be summarized using the or bit operation and will than be compared.
``

## Benchmark

|                                                    Method |     Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------------------------------------- |---------:|----------:|----------:|------:|------:|------:|----------:|
|              StringGuidComparision_EqualsStringBoxingEdge | 3.055 ns | 0.0343 ns | 0.0268 ns |     - |     - |     - |         - |
|                  StringGuidComparision_EqualsStringIsEdge | 3.400 ns | 0.0894 ns | 0.1131 ns |     - |     - |     - |         - |
|           GuidComparision_EqualsSpanTryWriteBytesGuidEdge | 6.428 ns | 0.1637 ns | 0.1885 ns |     - |     - |     - |         - |
|      GuidComparision_TransitiveComparisionSse2IsSupported | 2.934 ns | 0.0937 ns | 0.1219 ns |     - |     - |     - |         - |
| GuidComparision_EqualsGuidSpanEdgeMemoryMarshalCreateSpan | 5.241 ns | 0.1351 ns | 0.1198 ns |     - |     - |     - |         - |

|                                                    Method |     Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------------------------------------- |---------:|----------:|----------:|------:|------:|------:|----------:|
|              StringGuidComparision_EqualsStringBoxingEdge | 3.037 ns | 0.0235 ns | 0.0196 ns |     - |     - |     - |         - |
|                  StringGuidComparision_EqualsStringIsEdge | 3.179 ns | 0.0246 ns | 0.0218 ns |     - |     - |     - |         - |
|           GuidComparision_EqualsSpanTryWriteBytesGuidEdge | 6.175 ns | 0.0425 ns | 0.0354 ns |     - |     - |     - |         - |
|      GuidComparision_TransitiveComparisionSse2IsSupported | 2.833 ns | 0.0309 ns | 0.0289 ns |     - |     - |     - |         - |
| GuidComparision_EqualsGuidSpanEdgeMemoryMarshalCreateSpan | 5.152 ns | 0.0325 ns | 0.0254 ns |     - |     - |     - |         - |
