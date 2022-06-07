﻿using System;

namespace Unreal.Core.Models
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/Math/Vector.h#L29
    /// </summary>
    public class FVector
    {
        public FVector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }

        public float Size()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public double DistanceTo(FVector vector)
        {
            return Math.Sqrt(DistanceSquared(vector));
        }

        private double DistanceSquared(FVector vector)
        {
            return Math.Pow(vector.X - this.X, 2) + Math.Pow(vector.Y - this.Y, 2) + Math.Pow(vector.Z - this.Z, 2);
        }

        public static FVector operator -(FVector v1, FVector v2)
        {
            return new FVector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static bool operator ==(FVector v1, FVector v2)
        {
            return v1?.X == v2?.X && v1?.Y == v2?.Y && v1?.Z == v2?.Z;
        }

        public static bool operator !=(FVector v1, FVector v2)
        {
            return v1?.X != v2?.X || v1?.Y != v2?.Y || v1?.Z != v2?.Z;
        }

        public override bool Equals(object obj)
        {
            var other = obj as FVector;
            return X == other?.X && Y == other?.Y && Z == other?.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
    }
}
