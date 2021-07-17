using System;
namespace GuidBenchmark
{
    public interface IEdge
    {
        /// <summary>
        /// Get or sets the vertex
        /// </summary>
        Guid U { get; set; }
        /// <summary>
        /// Get or sets the vertex
        /// </summary>
        Guid V { get; set; }
        /// <summary>
        /// Get or sets the weight of the edge
        /// </summary>
        double Weighted { get; set; }
    }
}