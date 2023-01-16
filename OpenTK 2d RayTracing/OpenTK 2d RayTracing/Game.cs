using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OpenTK_2d_RayTracing
{
    internal class Game : GameWindow
    {
        private int _objectCount = 0;

        private Shader _shaderWalls;
        private Shader _shaderRays;
        private int _centerUniformLocation;
        private int _colorUniformLocation;

        private int _wallsVertexBuffer;
        private int _wallsVertexArray;
        private int _visionVertexBufferR;
        private int _visionVertexBufferG;
        private int _visionVertexBufferB;
        private int _visionVertexArrayR;
        private int _visionVertexArrayG;
        private int _visionVertexArrayB;
        private Vector4 ColorR = new Vector4(1, 0, 0, 1);
        private Vector4 ColorG = new Vector4(0, 1, 0, 0.5f);
        private Vector4 ColorB = new Vector4(0, 0, 1, 0.8f);
        private Vector2 CenterR = new Vector2(310, 301);
        private Vector2 CenterG = new Vector2(310, 450);
        private Vector2 CenterB = new Vector2(250, 160);
        private Vector2 CenterScaledR;
        private Vector2 CenterScaledG;
        private Vector2 CenterScaledB;
        private int ActiveCenter = 1;


        private int _width, _height;

        private List<Line> Walls;

        private float[] _verticesWalls;
        private float[] _verticesVisionR;
        private float[] _verticesVisionG;
        private float[] _verticesVisionB;
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, NumberOfSamples = 32 })
        {
            _width = width;
            _height = height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha);
            GL.Enable(EnableCap.Blend);

            CreateWorld();

            GL.ClearColor(0.15f, 0.15f, 0.20f, 1.0f);
            _wallsVertexBuffer = GL.GenBuffer();
            _wallsVertexArray = GL.GenVertexArray();
            _visionVertexBufferR = GL.GenBuffer();
            _visionVertexBufferG = GL.GenBuffer();
            _visionVertexBufferB = GL.GenBuffer();
            _visionVertexArrayR = GL.GenVertexArray();
            _visionVertexArrayG = GL.GenVertexArray();
            _visionVertexArrayB = GL.GenVertexArray();

            GL.BindVertexArray(_wallsVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _wallsVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticesWalls.Length * sizeof(float), _verticesWalls, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(_visionVertexArrayR);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferR);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(_visionVertexArrayG);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferG);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(_visionVertexArrayB);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferB);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shaderWalls = new Shader("Assets/vertexWalls.glsl", "Assets/fragmentWalls.glsl");
            _shaderWalls.Use();

            _shaderRays = new Shader("Assets/vertexRays.glsl", "Assets/fragmentRays.glsl");
            _shaderRays.Use();

            _centerUniformLocation = GL.GetUniformLocation(_shaderRays.Handle, "center");
            _colorUniformLocation = GL.GetUniformLocation(_shaderRays.Handle, "color");
        }

        protected void CreateWorld()
        {
            Walls = new List<Line>();
            // Boundary - ID 0
            AddWallsToWorld(CreateSquare((100, 100), (500, 500)));
            AddWallsToWorld(CreateSquare((120, 120), (170, 170)));
            AddWallsToWorld(CreateSquare((380, 200), (450, 300)));
            AddWallsToWorld(CreateSquare((200, 200), (230, 230)));
            AddWallsToWorld(CreateSquare((170, 330), (240, 430)));
            AddWallsToWorld(CreateSquare((470, 470), (380, 380)));


            float[] vertices = new float[Walls.Count * 4];
            for (int i = 0; i < Walls.Count; i++)
            {
                Line line = Walls[i];
                vertices[i * 4] = line.P1.X / _width * 2 - 1;
                vertices[i * 4 + 1] = line.P1.Y / _height * 2 - 1;
                vertices[i * 4 + 2] = line.P2.X / _width * 2 - 1;
                vertices[i * 4 + 3] = line.P2.Y / _height * 2 - 1;
            }
            _verticesWalls = vertices;
        }

        private void BufferRays()
        {
            var RaysR = RayCastVision(CenterR);
            var RaysG = RayCastVision(CenterG);
            var RaysB = RayCastVision(CenterB);

            _verticesVisionR = MakeVisionVertexArray(CenterR, RaysR);
            _verticesVisionG = MakeVisionVertexArray(CenterG, RaysG);
            _verticesVisionB = MakeVisionVertexArray(CenterB, RaysB);

            GL.BindVertexArray(_visionVertexArrayR);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferR);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticesVisionR.Length * sizeof(float), _verticesVisionR, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(_visionVertexArrayG);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferG);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticesVisionG.Length * sizeof(float), _verticesVisionG, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(_visionVertexArrayB);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferB);
            GL.BufferData(BufferTarget.ArrayBuffer, _verticesVisionB.Length * sizeof(float), _verticesVisionB, BufferUsageHint.DynamicDraw);
        }

        private float[] MakeVisionVertexArray(Vector2 Center, List<Intersection> Rays)
        {
            float[] vertices = new float[Rays.Count * 6];
            for (int i = 0; i < Rays.Count; i++)
            {
                var p1 = Center;
                Vector2 p2;
                Vector2 p3;
                var intersection1 = Rays[i];
                var intersection2 = Rays[(i + 1) % Rays.Count];
                if (intersection1.Target.WallID == intersection2.Target.WallID)
                {
                    p2 = intersection1.Target.pos;
                    p3 = intersection2.Target.pos;
                }
                else if (intersection1.Hit.WallID == intersection2.Target.WallID)
                {
                    p2 = intersection1.Hit.pos;
                    p3 = intersection2.Target.pos;
                }
                else if (intersection1.Target.WallID == intersection2.Hit.WallID)
                {
                    p2 = intersection1.Target.pos;
                    p3 = intersection2.Hit.pos;
                }
                else
                {
                    p2 = intersection1.Hit.pos;
                    p3 = intersection2.Hit.pos;
                }

                vertices[i * 6] = p1.X / _width * 2 - 1;
                vertices[i * 6 + 1] = p1.Y / _height * 2 - 1;
                vertices[i * 6 + 2] = p2.X / _width * 2 - 1;
                vertices[i * 6 + 3] = p2.Y / _height * 2 - 1;
                vertices[i * 6 + 4] = p3.X / _width * 2 - 1;
                vertices[i * 6 + 5] = p3.Y / _height * 2 - 1;
            }
            return vertices;
        }

        private void AddWallsToWorld(List<Line> NewWalls)
        {
            foreach (Line wall in NewWalls) Walls.Add(wall);
        }

        private List<Line> CreateSquare(Vector2 P1, Vector2 P2)
        {
            int id = _objectCount++;
            List<Line> Walls = new List<Line>();
            Walls.Add(new Line((P1.X, P1.Y), (P2.X, P1.Y), id));
            Walls.Add(new Line((P2.X, P1.Y), (P2.X, P2.Y), id));
            Walls.Add(new Line((P2.X, P2.Y), (P1.X, P2.Y), id));
            Walls.Add(new Line((P1.X, P2.Y), (P1.X, P1.Y), id));
            return Walls;
        }

        private List<Point> SortPointsByAngle(Vector2 Center)
        {
            HashSet<Point> Points = new HashSet<Point>();
            foreach (Line line in Walls)
            {
                Point p1 = new Point()
                {
                    pos = line.P1,
                    WallID = line.Id
                };
                Point p2 = new Point()
                {
                    pos = line.P2,
                    WallID = line.Id
                };
                Points.Add(p1);
                Points.Add(p2);
            }

            List<Point> PointsSorted = Points.ToList();

            PointsSorted.Sort((a, b) => {
                var r1 = GetRadians(a.pos, Center);
                var r2 = GetRadians(b.pos, Center);

                return r1 < r2 ? -1 : r1 == r2 ? 0 : 1;
            });

            return PointsSorted;
        }

        private List<Intersection> RayCastVision(Vector2 Center)
        {
            var Points = SortPointsByAngle(Center);
            List<Intersection> Intersections = new List<Intersection>();
            foreach (Point p in Points)
            {
                var Intersection = CastRayToPoint(p, Center);
                if (Intersection.Hit == null) Intersection.Hit = Intersection.Target;
                if ((Intersection.Target.pos - Center).LengthSquared - 3 > (Intersection.Hit.pos - Center).LengthSquared) continue;
                //if (Intersection.Target.WallID == Intersection.Hit.WallID) continue;
                Intersections.Add(Intersection);
            }
            return Intersections;
        }

        private Intersection CastRayToPoint(Point To, Vector2 Center)
        {
            Line ray = new Line(Center, (To.pos - Center) * 10000000);
            Point? Closest = null;
            foreach (Line line in Walls)
            {
                Vector2? result = Line.GetLineIntersection(line, ray);
                if (!result.HasValue) continue;
                Vector2 PointOfIntersection = result.Value;
                if ((PointOfIntersection - To.pos).LengthSquared < 0.01) continue;
                if (Closest == null)
                {
                    Closest = new Point(PointOfIntersection, line.Id);
                }
                else if ((Closest.pos - Center).LengthSquared > (PointOfIntersection - Center).LengthSquared)
                {
                    Closest = new Point(PointOfIntersection, line.Id);
                }
            }

            var Intersection = new Intersection()
            {
                Target = To,
                Hit = Closest
            };
            return Intersection;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var left = KeyboardState[Keys.Left];
            var right = KeyboardState[Keys.Right];
            var up = KeyboardState[Keys.Up];
            var down = KeyboardState[Keys.Down];
            var one = KeyboardState[Keys.D1];
            var two = KeyboardState[Keys.D2];
            var three = KeyboardState[Keys.D3];

            if (one) ActiveCenter = 1;
            if (two) ActiveCenter = 2;
            if (three) ActiveCenter = 3;

            Vector2 direction = new Vector2(0, 0);
            float speed = 2.48014f;
            if (left) direction += (-1, 0);
            if (right) direction += (1, 0);
            if (up) direction += (0, 1);
            if (down) direction += (0, -1);

            if (ActiveCenter == 1)
            {
                CenterR += direction * speed;
            }
            else if (ActiveCenter == 2)
            {
                CenterG += direction * speed;
            }
            else
            {
                CenterB += direction * speed;
            }

            CenterScaledR = CenterR / _width * 2 - new Vector2(1, 1);
            CenterScaledG = CenterG / _width * 2 - new Vector2(1, 1);
            CenterScaledB = CenterB / _width * 2 - new Vector2(1, 1);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            BufferRays();
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shaderWalls.Use();
            GL.BindVertexArray(_wallsVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _wallsVertexBuffer);
            GL.DrawArrays(PrimitiveType.Lines, 0, _verticesWalls.Length / 2);

            _shaderRays.Use();
            GL.BindVertexArray(_visionVertexArrayR);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferR);
            GL.Uniform2(_centerUniformLocation, CenterScaledR);
            GL.Uniform4(_colorUniformLocation, ColorR);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _verticesVisionR.Length / 2);

            GL.BindVertexArray(_visionVertexArrayG);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferG);
            GL.Uniform2(_centerUniformLocation, CenterScaledG);
            GL.Uniform4(_colorUniformLocation, ColorG);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _verticesVisionG.Length / 2);

            GL.BindVertexArray(_visionVertexArrayB);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _visionVertexBufferB);
            GL.Uniform2(_centerUniformLocation, CenterScaledB);
            GL.Uniform4(_colorUniformLocation, ColorB);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _verticesVisionB.Length / 2);

            SwapBuffers();

            Thread.Sleep(15);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);

        }

        private float GetRadians(Vector2 Point, Vector2 Center)
        {
            Vector2 normalized = (Point - Center).Normalized();
            return MathF.Atan2(normalized.Y, normalized.X);
        }
        private class Point
        {
            public Vector2 pos;
            public int WallID;
            public Point()
            {

            }
            public Point(Vector2 pos, int wallID)
            {
                this.pos = pos;
                WallID = wallID;
            }
            public override int GetHashCode()
            {
                return (int)MathF.Round(1337 * pos.X) + (int)MathF.Round(pos.Y);
            }
            public override bool Equals(object? obj)
            {
                return obj is Point && Equals((Point)obj);
            }
            public bool Equals(Point p)
            {
                return (p.pos-pos).LengthSquared < 0.01;
            }
        }
        private class Intersection
        {
            public Point Target;
            public Point Hit;
        }
    }
}
