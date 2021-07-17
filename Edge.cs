using System;

namespace GuidBenchmark
{
    public class Edge : IEdge
    {
        public Guid U { get; set; }
        /// <summary>
        /// Get or sets the vertex
        /// </summary>
        public Guid V { get; set; }
        /// <summary>
        /// Get or sets the weight of the edge
        /// </summary>
        public double Weighted { get; set; }
    }
}