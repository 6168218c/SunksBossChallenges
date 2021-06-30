using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SunksBossChallenges.Effects
{
    public struct VertexStripInfo:IVertexType
    {
        public Vector2 Position;
        public Vector3 TexCoordsAndAlpha;
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(
            new VertexElement[]
            {
                new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
                new VertexElement(8,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
            });

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        public VertexStripInfo(Vector2 position,Vector3 coordsAndAlpha)
        {
            Position = position;
            TexCoordsAndAlpha = coordsAndAlpha;
        }
    }
}
