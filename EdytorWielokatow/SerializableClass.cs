using EdytorWielokatow.Edges;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EdytorWielokatow
{
    [JsonDerivedType(typeof(Edge), typeDiscriminator: Edge.ClassName)]
    [JsonDerivedType(typeof(BezierEdge), typeDiscriminator: BezierEdge.ClassName)]
    [JsonDerivedType(typeof(FixedLengthEdge), typeDiscriminator: FixedLengthEdge.ClassName)]
    [JsonDerivedType(typeof(HorizontalEdge), typeDiscriminator: HorizontalEdge.ClassName)]
    [JsonDerivedType(typeof(VerticalEdge), typeDiscriminator: VerticalEdge.ClassName)]
    [JsonDerivedType(typeof(Vertex), typeDiscriminator: Vertex.ClassName)]
    [JsonDerivedType(typeof(ControlVertex), typeDiscriminator: ControlVertex.ClassName)]
    [JsonDerivedType(typeof(ContinuityVertex), typeDiscriminator: ContinuityVertex.ClassName)]
    public class SerializableClass
    {
        public const string ClassName = "SERIALIZABLE";
    }
}
