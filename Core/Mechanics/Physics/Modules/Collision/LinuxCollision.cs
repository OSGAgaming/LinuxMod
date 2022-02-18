using LinuxMod.Core;
using LinuxMod.Core.Helper.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;

namespace LinuxMod.Core.Mechanics
{
    public enum Bound
    {
        None,
        Top,
        Bottom,
        Left,
        Right
    }
    public struct CollisionInfo
    {
        public Bound AABB;
        public Vector2 d;
        public Vector2 Axis;
        public float Depth;
        public static CollisionInfo Default => new CollisionInfo(Vector2.Zero, Bound.None);
        public CollisionInfo(Vector2 d, Bound AABB = Bound.None, Vector2 Axis = default, float Depth = 0)
        {
            this.AABB = AABB;
            this.d = d;
            this.Axis = Axis;
            this.Depth = Depth;
        }
    }
    public struct Circle
    {
        public float radius;
        public Vector2 position;
        public Circle(float radius, Vector2 position)
        {
            this.radius = radius;
            this.position = position;
        }
    }

    public struct Polygon
    {
        public Vector2[] points;
        public Vector2[] varpoints
        {
            get
            {
                Vector2[] bufferArray = new Vector2[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    bufferArray[i] = points[i] + Center;
                }
                return bufferArray;
            }
        }
        public Vector2 Center;
        public int numberOfPoints;
        public static Polygon Null => new Polygon(new Vector2[] { }, Vector2.Zero);
        public Rectangle Rectangle
        {
            get
            {
                if (points.Length == 4)
                {
                    return LinuxTechTips.Rect(
                        varpoints[0].ToPoint(),
                        new Point((int)(varpoints[1].X - varpoints[0].X),
                        (int)(varpoints[2].Y - varpoints[0].Y)));
                }
                return Rectangle.Empty;
            }
        }

        public static Polygon RectToPoly(Rectangle rect)
        {
            return new Polygon(new Vector2[] {
                new Vector2(-rect.Width/2, - rect.Height/2),
                new Vector2(rect.Width/2, - rect.Height/2),
                new Vector2(rect.Width/2, rect.Height/2),
                new Vector2(-rect.Width/2, rect.Height/2)},
                rect.Center.ToVector2());
        }
        public void Draw()
        {
            for (int i = 0; i < points.Length; i++)
            {
                LinuxTechTips.DrawLine(varpoints[i].ForDraw(), varpoints[(i + 1) % numberOfPoints].ForDraw(), Color.Green, 1);
                LinuxTechTips.DrawLine(varpoints[i].ForDraw(), Center.ForDraw(), Color.Green, 1);
            }
        }
        public static Polygon operator +(Polygon x, Polygon y)
        {
            if (x.numberOfPoints == y.numberOfPoints)
            {
                Vector2[] points = new Vector2[x.numberOfPoints];
                for (int i = 0; i < x.numberOfPoints; i++)
                {
                    points[i] = x.points[i] + y.points[i];
                }
                return new Polygon(points, x.Center + y.Center);
            }
            return new Polygon(new Vector2[] { }, Vector2.Zero);
        }

        public Polygon(Vector2[] points, Vector2 position)
        {
            this.points = points;
            Center = position;
            numberOfPoints = points.Length;
        }
    }
    public static class LinuxCollision
    {
        public static bool LineIntersectsRect(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }
        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
            if (d == 0)
                return false;
            float r = q / d;
            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1)
                return false;
            return true;
        }

        public static Vector2 ReturnIntersectionLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
            if (d == 0)
                return Vector2.Zero;
            float r = q / d;
            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;
            if (r < 0 || r > 1 || s < 0 || s > 1)
                return Vector2.Zero;
            return Vector2.Lerp(l1p1, l1p2, r);
        }
        public static Vector2 ReturnIntersectRect(Vector2 p1, Vector2 p2, Rectangle r)
        {
            Vector2[] a = { ReturnIntersectionLine(p1, p2, new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y)),
            ReturnIntersectionLine(p1, p2, new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height)),
            ReturnIntersectionLine(p1, p2, new Vector2(r.X + r.Width, r.Y + r.Height), new Vector2(r.X, r.Y + r.Height)),
            ReturnIntersectionLine(p1, p2, new Vector2(r.X, r.Y + r.Height), new Vector2(r.X, r.Y)) };
            Vector2 chosen = Vector2.Zero;
            for (int i = 0; i < a.Length; i++)
            {
                if (i == 0)
                    chosen = a[0];
                else if (Vector2.Distance(a[i], p1) < Vector2.Distance(chosen, p1))
                {
                    chosen = a[i];
                }
            }
            return chosen;

        }
        public static Polygon Add(this Polygon x, Vector2[] y)
        {
            Vector2[] points = new Vector2[x.numberOfPoints];
            for (int i = 0; i < x.numberOfPoints; i++)
            {
                points[i] = x.points[i] + y[i];
            }
            x.points = points;
            return x;
        }
        public static Polygon ToPolygon(this Rectangle r)
        {
            Vector2[] points = {new Vector2(-r.Width / 2, -r.Height / 2),
              new Vector2(r.Width / 2, -r.Height / 2),
              new Vector2(r.Width / 2, r.Height / 2),
              new Vector2(-r.Width / 2, r.Height / 2)};
            return new Polygon(points, r.Center.ToVector2());
        }
        public static CollisionInfo Raycast(Polygon shape1, Polygon shape2, int LengthOfRaycast, int Disp)
        {
            Vector2 CollisionPoint = ReturnIntersectionLine(shape1.Center, shape1.Center + new Vector2(0, LengthOfRaycast), shape2.varpoints[0], shape2.varpoints[1]);
            Bound b = Bound.None;
            Vector2 v = Vector2.Zero;
            float d = shape1.Center.Y + Disp;
            if (CollisionPoint != Vector2.Zero)
            {
                if (d > CollisionPoint.Y)
                {
                    v = new Vector2(0, CollisionPoint.Y - d);
                }
                if (d > CollisionPoint.Y - 1)
                {
                    b = Bound.Top;
                }
            }

            return new CollisionInfo(v, b);
        }

        public static float IntervalDistance(float MinA, float MaxA, float MinB, float MaxB)
        {
            if (MinA < MinB)
                return MinB - MaxA;
            else
                return MinA - MaxB;
        }

        public static CollisionInfo SAT(Polygon shape1, Polygon shape2)
        {
            Polygon[] shapes = new Polygon[] { shape1, shape2 };
            float overlap = float.PositiveInfinity;
            float MinDistance = float.PositiveInfinity;
            Vector2 Axis = Vector2.Zero;

            for (int a = 0; a < 2; a++)
            {
                for (int i = 0; i < shapes[a].numberOfPoints; i++)
                {
                    int b = (i + 1) % shapes[a].numberOfPoints;
                    Vector2 axis = shapes[a].varpoints[b] - shapes[a].varpoints[i];
                    Vector2 axisNormal = Vector2.Normalize(new Vector2(-axis.Y, axis.X));
                    float aMax = float.NegativeInfinity;
                    float aMin = float.PositiveInfinity;
                    for (int j = 0; j < shape1.numberOfPoints; j++)
                    {
                        float projection = Vector2.Dot(axisNormal, shape1.varpoints[j]);
                        aMax = Math.Max(projection, aMax);
                        aMin = Math.Min(projection, aMin);
                    }
                    float bMax = float.NegativeInfinity;
                    float bMin = float.PositiveInfinity;
                    for (int j = 0; j < shape2.numberOfPoints; j++)
                    {
                        float projection = Vector2.Dot(axisNormal, shape2.varpoints[j]);
                        bMax = Math.Max(projection, bMax);
                        bMin = Math.Min(projection, bMin);
                    }
                    overlap = Math.Min(Math.Min(bMax, aMax) - Math.Max(bMin, aMin), overlap);
                    float Distance = IntervalDistance(aMin, aMax, bMin, bMax);

                    if (Distance > 0)
                        return new CollisionInfo(Vector2.Zero, Bound.None);
                    else if (Math.Abs(Distance) < MinDistance && a == 1)
                    {
                        MinDistance = Math.Abs(Distance);
                        Axis = axisNormal;
                    }
                }
            }
            
            int Sign = Math.Sign(Vector2.Dot(Axis, shape1.Center - shape2.Center));

            if (Sign == 1)
                Axis *= -1; 

            Vector2 disp = shape2.Center - shape1.Center;
            float s = disp.Length();
            return new CollisionInfo(new Vector2(overlap * disp.X / s, overlap * disp.Y / s), Bound.Top, Axis, MinDistance);
        }

        public static Vector2 TestForCollisionsDiag(Polygon shape1, Polygon shape2)
        {
            Polygon[] shapes = new Polygon[] { shape1, shape2 };
            Vector2 displacement = Vector2.Zero;
            for (int a = 0; a < 2; a++)
            {
                for (int i = 0; i < shapes[a].numberOfPoints; i++)
                {
                    for (int j = 0; j < shapes[(a + 1) % 2].numberOfPoints; j++)
                    {
                        Vector2 intersectionPoint = ReturnIntersectionLine(shapes[a].varpoints[i], shapes[a].Center, shapes[(a + 1) % 2].varpoints[j], shapes[(a + 1) % 2].varpoints[(j + 1) % shapes[(a + 1) % 2].numberOfPoints]);
                        if (intersectionPoint != Vector2.Zero)
                        {
                            displacement += intersectionPoint - shapes[a].varpoints[i];
                            if (a == 1)
                            {
                                displacement *= -1;
                            }
                        }
                    }
                }
            }
            return displacement;
        }

        public static bool AABB(Rectangle A, Rectangle B)
        {
            return A.Intersects(B);
        }
        public static CollisionInfo AABBResolvePoly(Polygon _A, Polygon _ALast, Polygon _B)
        {
            Vector2 d = Vector2.Zero;
            Bound bound = Bound.None;
            Rectangle A = _A.Rectangle;
            Rectangle ALast = _ALast.Rectangle;
            Rectangle B = _B.Rectangle;
            if (!A.Intersects(new Rectangle(B.X, B.Y - 1, B.Width, B.Height + 2)))
                return CollisionInfo.Default;
            else
            {
                if (ALast.Bottom > B.Top && ALast.Top < B.Bottom)
                {
                    if (ALast.Left > B.Center.X)
                    {
                        bound = Bound.Left;
                        d = new Vector2(B.Right - A.Left, 0);
                    }
                    if (ALast.Right < B.Center.X)
                    {
                        bound = Bound.Right;
                        d = new Vector2(B.Left - A.Right, 0);
                    }
                }
                if (ALast.Left < B.Right && ALast.Right > B.Left)
                {
                    if (ALast.Top > B.Center.Y)
                    {
                        bound = Bound.Bottom;
                        d = new Vector2(0, B.Bottom - A.Top);
                    }
                    if (ALast.Bottom < B.Center.Y)
                    {
                        bound = Bound.Top;
                        d = new Vector2(0, B.Top - A.Bottom);
                    }
                }
            }
            return new CollisionInfo(d, bound);
        }
        public static bool CirclevsCircle(Circle a, Circle b)
        {
            float r = a.radius + b.radius;
            r *= r;
            return r < (a.position.X + b.position.X) * (a.position.X + b.position.X) +
                       (a.position.Y + b.position.Y) * (a.position.Y + b.position.Y);
        }
    }
}
