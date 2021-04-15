using System;

public class Vec2
{
    const double eq_margin = 1E-03;

    public float x, y;
    public float magnitude { get { return (float)Math.Sqrt(x * x + y * y); } }

    public static readonly Vec2 zero = new Vec2(0,0);

    public Vec2 normalized { get { return new Vec2(x / magnitude, y / magnitude); }}
    // Start is called before the first frame update
    public Vec2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj) {
        if (!(obj is Vec2)) { return false; }
        Vec2 other = (Vec2)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode () {
		return x.GetHashCode () ^ y.GetHashCode () << 2;
	}

    public override string ToString() {
        return string.Format("Vec2: " + x + ", " + y);
    }

    public static float Distance(Vec2 a, Vec2 b)
    {
        return (a-b).magnitude;
    }

    public static float Dot(Vec2 a, Vec2 b)
    {
        return a.x*b.x + a.y*b.y;
    }

    public static Vec2 Lerp(Vec2 a, Vec2 b, float f)
    {
        return new Vec2(MathOps.lerp(a.x, b.x, f), MathOps.lerp(a.y, b.y, f));
    }

    public static bool operator == (Vec2 a, Vec2 b) {
        Vec2 diff = new Vec2(Math.Abs(a.x-b.x), Math.Abs(a.y-b.y));
        return diff.magnitude * diff.magnitude < eq_margin;
    }

    public static bool operator != (Vec2 a, Vec2 b) {
        Vec2 diff = new Vec2(Math.Abs(a.x-b.x), Math.Abs(a.y-b.y));
        return diff.magnitude >= eq_margin;
    }

    public static Vec2 operator + (Vec2 a, Vec2 b) {
        return new Vec2(a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator - (Vec2 a, Vec2 b) {
        return new Vec2(a.x - b.x, a.y - b.y);
    }

    public static Vec2 operator * (Vec2 vec, float i) {
        return new Vec2(vec.x * i, vec.y * i);
    }

    public static Vec2 operator * (float i, Vec2 vec) {
        return new Vec2(vec.x * i, vec.y * i);
    }

    public static Vec2 operator * (Vec2 a, Vec2 b) {
        return new Vec2(a.x * b.x, a.y * b.y);
    }

    public static Vec2 operator / (Vec2 a, Vec2 b) {
        return new Vec2(a.x / b.x, a.y / b.y);
    }

    public static Vec2 operator / (Vec2 vec, float i) {
        return new Vec2(vec.x / i, vec.y / i);
    }
}
