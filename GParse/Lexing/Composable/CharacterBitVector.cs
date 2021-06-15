using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A <see cref="Char"/> bitvector.
    /// </summary>
    public class CharacterBitVector : IEquatable<CharacterBitVector>
    {
        private readonly Range<Char> _range;
        private readonly ReadOnlyMemory<Byte> _vector;

        [SuppressMessage("Style", "IDE0056:Use index operator", Justification = "Not available in all target frameworks.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Valid for some target frameworks.")]
        internal CharacterBitVector(IEnumerable<Char> characters)
        {
            if (characters is null)
                throw new ArgumentNullException(nameof(characters));

            var chars = characters.ToArray();
            if (chars.Length < 1)
                throw new ArgumentException("Not enough characters were provided.");

            Array.Sort(chars);

            var range = new Range<Char>(chars[0], chars[chars.Length - 1]);
            this._range = range;

            var bitsRequired = range.End - range.Start + 1;
            var bytesRequired = GetByteVectorIndexAndOffset(bitsRequired, out var remainder);
            if (remainder > 0) bytesRequired++;

            Memory<Byte> vector = new Byte[bytesRequired];
            ref var firstElem = ref MemoryMarshal.GetReference(vector.Span);
            foreach (var ch in chars)
            {
                var chOffset = ch - range.Start;
                var index = GetByteVectorIndexAndOffset(chOffset, out var bitOffset);
                // With this we avoid the bounds check since we know we'll always be in bounds.
                Unsafe.Add(ref firstElem, index) |= (Byte) (1 << bitOffset);
            }
            this._vector = vector;
        }

        /// <summary>
        /// Checks whether this bit vector contains the provided character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Boolean Contains(Char ch)
        {
            if (!this._range.ValueIn(ch))
                return false;
            var index = GetByteVectorIndexAndOffset(ch - this._range.Start, out var offset);
            var value = Unsafe.Add(ref MemoryMarshal.GetReference(this._vector.Span), index);
            return (value & (1 << offset)) != 0;
        }

        /// <inheritdoc/>
        public override Boolean Equals(Object? obj) =>
            this.Equals(obj as CharacterBitVector);

        /// <inheritdoc/>
        public Boolean Equals(CharacterBitVector? other) =>
            other is not null
            && this._range.Equals(other._range)
            && this._vector.Span.SequenceEqual(other._vector.Span);

        /// <inheritdoc/>
        public override Int32 GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this._range);
            foreach (var b in this._vector.Span) hash.Add(b);
            return hash.ToHashCode();
        }

        /// <summary>
        /// Checks whether two bit vectors are the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(CharacterBitVector? left, CharacterBitVector? right)
        {
            if (right is null)
                return left is null;
            return ReferenceEquals(left, right) || right.Equals(left);
        }

        /// <summary>
        /// Checks whether two bit vectors are not the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(CharacterBitVector? left, CharacterBitVector? right) => !(left == right);

        /// <summary>
        /// Gets the index and the offset of the provided bit index on a <see cref="Byte"/>-backed bit vector.
        /// </summary>
        /// <param name="bitIndex"></param>
        /// <param name="offset">The offset inside the <see cref="Byte"/>.</param>
        /// <returns>The index of the <see cref="Byte"/> in the array/span/memory.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 GetByteVectorIndexAndOffset(Int32 bitIndex, out Int32 offset)
        {
            const Int32 ByteShiftAmount = 3;
            const Int32 ByteRemainderMask = 0b111;
            offset = bitIndex & ByteRemainderMask;
            return bitIndex >> ByteShiftAmount;
        }
    }
}