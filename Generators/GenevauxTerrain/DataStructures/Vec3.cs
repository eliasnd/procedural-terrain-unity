using System;

public class Vec3
{
    public float x, y, z;
    public float magnitude { get { return (float)Math.Sqrt(Math.Sqrt(x*x + y*y) * Math.Sqrt(x*x + y*y) + z*z); } }

    public static readonly Vec3 zero = new Vec3(0,0,0);
    public Vec3 normalized { get { return new Vec3(x / magnitude, y / magnitude, z / magnitude); }}
    // Start is called before the first frame update
    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vec3(Vec2 vec)
    {
        x = vec.x;
        y = vec.y;
        z = 0;
    }

    public override bool Equals(object obj) {
        if (!(obj is Vec3)) { return false; }
        Vec3 other = (Vec3)obj;
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode () {
		return x.GetHashCode () ^ y.GetHashCode () ^ z.GetHashCode() << 2;
	}

    public override string ToString() {
        return string.Format("Vec3: " + x + ", " + y + ", " + z);
    }

    public static float Distance(Vec3 a, Vec3 b)
    {
        return (a-b).magnitude;
    }

    public static float Dot(Vec3 a, Vec3 b)
    {
        return a.x*b.x + a.y*b.y + a.z*b.z;
    }

    public static bool operator == (Vec3 a, Vec3 b) {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator != (Vec3 a, Vec3 b) {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }

    public static Vec3 operator + (Vec3 a, Vec3 b) {
        return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vec3 operator - (Vec3 a, Vec3 b) {
        return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vec3 operator * (Vec3 vec, float i) {
        return new Vec3(vec.x * i, vec.y * i, vec.z * i);
    }

    public static Vec3 operator * (float i, Vec3 vec) {
        return new Vec3(vec.x * i, vec.y * i, vec.z * i);
    }

    public static Vec3 operator * (Vec3 a, Vec3 b) {
        return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vec3 operator / (Vec3 a, Vec3 b) {
        return new Vec3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vec3 operator / (Vec3 vec, float i) {
        return new Vec3(vec.x / i, vec.y / i, vec.z / i);
    }
}
