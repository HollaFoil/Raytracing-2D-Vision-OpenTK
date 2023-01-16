using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_2d_RayTracing
{
    public class Line
    {
        public Vector2 P1;
        public Vector2 P2;
        public Vector2 Direction;
        public Vector2 Origin;
        public double Length;
        public int Id = -1;
        public Line(Vector2 p1, Vector2 p2, int id)
        {
            P1 = p1;
            P2 = p2;
            Origin = p1;
            Length = (p2 - p1).Length;
            Direction = (p2 - p1).Normalized();
            Id = id;
        }

        public Line(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;
            Origin = p1;
            Length = (p2 - p1).Length;
            Direction = (p2 - p1).Normalized();
        }
        public static Vector2? GetLineIntersection(Line line1, Line line2)
        {
            Vector2 P1 = line1.P1, P2 = line1.P2;
            Vector2 P3 = line2.P1, P4 = line2.P2;
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = P2.X - P1.X; s1_y = P2.Y - P1.Y;
            s2_x = P4.X - P3.X; s2_y = P4.Y - P3.Y;

            float s, t;
            s = (-s1_y * (P1.X - P3.X) + s1_x * (P1.Y - P3.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (P1.Y - P3.Y) - s2_y * (P1.X - P3.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                var x = P1.X + (t * s1_x);
                var y = P1.Y + (t * s1_y);
                return new Vector2(x, y);
            }

            return null; // No collision
        }
    }
}