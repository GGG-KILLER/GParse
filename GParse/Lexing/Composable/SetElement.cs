using System;
using System.Globalization;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The type of the <see cref="SetElement"/>.
    /// </summary>
    public enum SetElementType : Byte
    {
        /// <summary>
        /// An invalid element.
        /// </summary>
        Invalid,
        /// <summary>
        /// A character element.
        /// </summary>
        Character,
        /// <summary>
        /// A range element.
        /// </summary>
        Range,
        /// <summary>
        /// An unicode category element.
        /// </summary>
        UnicodeCategory
    }

    /// <summary>
    /// A union type of all the possible types of a regex set.
    /// </summary>
    public readonly struct SetElement
    {
        private readonly Char _character;
        private readonly Range<Char> _range;
        private readonly UnicodeCategory _unicodeCategory;

        /// <summary>
        /// The type of this set element.
        /// </summary>
        public SetElementType Type { get; }

        /// <summary>
        /// Obtains the character value of this element. Throws if this is not a character element.
        /// </summary>
        public Char Character
        {
            get
            {
                if ( this.Type != SetElementType.Character )
                    throw new InvalidOperationException ( "This is not a character element." );
                return this._character;
            }
        }

        /// <summary>
        /// Obtains the range value of this element. Throws if this is not a range element.
        /// </summary>
        public Range<Char> Range
        {
            get
            {
                if ( this.Type != SetElementType.Range )
                    throw new InvalidOperationException ( "This is not a range element." );
                return this._range;
            }
        }

        /// <summary>
        /// Obtains the unicode category value of this element. Throws if this is not a unicode category element.
        /// </summary>
        public UnicodeCategory UnicodeCategory
        {
            get
            {
                if ( this.Type != SetElementType.UnicodeCategory )
                    throw new InvalidOperationException ( "This is not a unicode category element." );
                return this._unicodeCategory;
            }
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="character"></param>
        public SetElement ( Char character )
        {
            this.Type = SetElementType.Character;
            this._character = character;
            this._range = default;
            this._unicodeCategory = default;
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="range"></param>
        public SetElement ( Range<Char> range )
        {
            this.Type = SetElementType.Range;
            this._character = default;
            this._range = range;
            this._unicodeCategory = default;
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="unicodeCategory"></param>
        public SetElement ( UnicodeCategory unicodeCategory )
        {
            this.Type = SetElementType.UnicodeCategory;
            this._character = default;
            this._range = default;
            this._unicodeCategory = unicodeCategory;
        }

        /// <summary>
        /// Creates a new element from a character.
        /// </summary>
        /// <param name="character"></param>
        public static implicit operator SetElement ( Char character ) =>
            new SetElement ( character );

        /// <summary>
        /// Creates a new element from a range.
        /// </summary>
        /// <param name="range"></param>
        public static implicit operator SetElement ( Range<Char> range ) =>
            new SetElement ( range );

        /// <summary>
        /// Creates a new element from an unicode category.
        /// </summary>
        /// <param name="unicodeCategory"></param>
        public static implicit operator SetElement ( UnicodeCategory unicodeCategory ) =>
            new SetElement ( unicodeCategory );

        /// <inheritdoc cref="Character"/>
        public static explicit operator Char ( SetElement setElement ) =>
            setElement.Character;

        /// <inheritdoc cref="Range"/>
        public static explicit operator Range<Char> ( SetElement setElement ) =>
            setElement.Range;

        /// <inheritdoc cref="UnicodeCategory"/>
        public static explicit operator UnicodeCategory ( SetElement setElement ) =>
            setElement.UnicodeCategory;
    }
}
