using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The type of the <see cref="SetElement"/>.
    /// </summary>
    [SuppressMessage ( "Design", "CA1028:Enum Storage should be Int32", Justification = "Not going to increase memory usage for no reason." )]
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
        UnicodeCategory,
        /// <summary>
        /// A grammar node.
        /// </summary>
        Node,
    }


    /// <summary>
    /// A union type of all the possible types of a regex set.
    /// </summary>
    [SuppressMessage ( "Performance", "CA1815:Override equals and operator equals on value types", Justification = "Unnecessary since equality comparisons are never done." )]
    public readonly struct SetElement
    {
        private readonly Char _character;
        private readonly Range<Char> _range;
        private readonly UnicodeCategory _unicodeCategory;
        private readonly GrammarNode<Char>? _node;

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
        /// Obtains the node value of this element. Throws if this is not a node element.
        /// </summary>
        public GrammarNode<Char> Node
        {
            get
            {
                if ( this.Type != SetElementType.Node )
                    throw new InvalidOperationException ( "This is not a node element." );
                return this._node!;
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
            this._node = default;
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
            this._node = default;
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
            this._node = default;
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="node"></param>
        internal SetElement ( GrammarNode<Char> node )
        {
            this.Type = SetElementType.Node;
            this._character = default;
            this._range = default;
            this._unicodeCategory = default;
            this._node = node ?? throw new ArgumentNullException ( nameof ( node ) );
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="set"></param>
        public SetElement ( Set set ) : this ( ( GrammarNode<Char> ) set )
        {
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        public SetElement ( NegatedCharacterRange negatedCharacterRange ) : this ( ( GrammarNode<Char> ) negatedCharacterRange )
        {
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        public SetElement ( NegatedCharacterTerminal negatedCharacterTerminal ) : this ( ( GrammarNode<Char> ) negatedCharacterTerminal )
        {
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="negatedUnicodeCategoryTerminal"></param>
        public SetElement ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal ) : this ( ( GrammarNode<Char> ) negatedUnicodeCategoryTerminal )
        {
        }

        /// <summary>
        /// Initializes a new set element.
        /// </summary>
        /// <param name="negatedSet"></param>
        public SetElement ( NegatedSet negatedSet ) : this ( ( GrammarNode<Char> ) negatedSet )
        {
        }

        /// <summary>
        /// Creates a new element from a character.
        /// </summary>
        /// <param name="character"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( Char character ) =>
            new SetElement ( character );

        /// <summary>
        /// Creates a new element from a range.
        /// </summary>
        /// <param name="range"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( Range<Char> range ) =>
            new SetElement ( range );

        /// <summary>
        /// Creates a new element from an unicode category.
        /// </summary>
        /// <param name="unicodeCategory"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( UnicodeCategory unicodeCategory ) =>
            new SetElement ( unicodeCategory );

        /// <summary>
        /// Creates a new element from a character terminal.
        /// </summary>
        /// <param name="characterTerminal"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( CharacterTerminal characterTerminal ) =>
            characterTerminal is not null ? new SetElement ( characterTerminal.Value ) : default;

        /// <summary>
        /// Creates a new element from a character range.
        /// </summary>
        /// <param name="characterRange"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( CharacterRange characterRange ) =>
            characterRange is not null ? new SetElement ( characterRange.Range ) : default;

        /// <summary>
        /// Creates a new element from a unicode category terminal.
        /// </summary>
        /// <param name="unicodeCategoryTerminal"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( UnicodeCategoryTerminal unicodeCategoryTerminal ) =>
            unicodeCategoryTerminal is not null ? new SetElement ( unicodeCategoryTerminal.Category ) : default;

        /// <summary>
        /// Creates a new element from a set.
        /// </summary>
        /// <param name="set"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( Set set ) =>
            set is not null ? new SetElement ( set ) : default;

        /// <summary>
        /// Creates a new element from a negated character terminal.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( NegatedCharacterTerminal negatedCharacterTerminal ) =>
            negatedCharacterTerminal is not null ? new SetElement ( negatedCharacterTerminal ) : default;

        /// <summary>
        /// Creates a new element from a negated character range.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( NegatedCharacterRange negatedCharacterRange ) =>
            negatedCharacterRange is not null ? new SetElement ( negatedCharacterRange ) : default;

        /// <summary>
        /// Creates a new element from a negated unicode category terminal.
        /// </summary>
        /// <param name="negatedUnicodeCategoryTerminal"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal ) =>
            negatedUnicodeCategoryTerminal is not null ? new SetElement ( negatedUnicodeCategoryTerminal ) : default;

        /// <summary>
        /// Creates a new element from a negated set.
        /// </summary>
        /// <param name="negatedSet"></param>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Has a constructor for it." )]
        public static implicit operator SetElement ( NegatedSet negatedSet ) =>
            negatedSet is not null ? new SetElement ( negatedSet ) : default;

        /// <inheritdoc cref="Character"/>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's a property for it." )]
        public static explicit operator Char ( SetElement setElement ) =>
            setElement.Character;

        /// <inheritdoc cref="Range"/>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's a property for it." )]
        public static explicit operator Range<Char> ( SetElement setElement ) =>
            setElement.Range;

        /// <inheritdoc cref="UnicodeCategory"/>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's a property for it." )]
        public static explicit operator UnicodeCategory ( SetElement setElement ) =>
            setElement.UnicodeCategory;

        /// <inheritdoc cref="Node"/>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There's a property for it." )]
        public static explicit operator GrammarNode<Char> ( SetElement setElement ) =>
            setElement.Node;
    }
}
