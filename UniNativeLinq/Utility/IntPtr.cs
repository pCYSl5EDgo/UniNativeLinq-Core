using System;
using System.Runtime.InteropServices;

namespace UniNativeLinq
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct
        IntPtr<T>
        : IEquatable<IntPtr>, IEquatable<IntPtr<T>>
        where T : unmanaged
    {
        [FieldOffset(0)] public IntPtr Ptr;
        [FieldOffset(0)] public T* Value;

        public IntPtr(IntPtr ptr)
        {
            Value = default;
            Ptr = ptr;
        }

        public IntPtr(T* value)
        {
            Ptr = default;
            Value = value;
        }

        public ref T this[long index] => ref Value[index];

        public override bool Equals(object obj) => obj is IntPtr<T> other && Equals(other);
        public override int GetHashCode() => Ptr.ToInt32();
        public bool Equals(IntPtr other) => Ptr == other;
        bool IEquatable<IntPtr<T>>.Equals(IntPtr<T> other) => Value == other.Value;
        public static explicit operator IntPtr<T>(IntPtr ptr) => new IntPtr<T>(ptr);
        public static implicit operator IntPtr<T>(T* ptr) => new IntPtr<T>(ptr);
        public static implicit operator IntPtr(IntPtr<T> @this) => @this.Ptr;
        public static implicit operator T*(IntPtr<T> @this) => @this.Value;
        public static implicit operator bool(IntPtr<T> @this) => @this.Value != null;
        public static bool operator ==(IntPtr<T> left, IntPtr<T> right) => left.Value == right.Value;
        public static bool operator !=(IntPtr<T> left, IntPtr<T> right) => left.Value != right.Value;
    }
}
