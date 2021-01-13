using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;
using GParse.Math;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A regex alternation set.
    /// </summary>
    /// <remarks>
    /// This node matches if a character fits <b>any</b> of this conditions:
    /// <list type="bullet">
    /// <item>Is in the set's <see cref="Characters"/>;</item>
    /// <item>Is in any of the the set's <see cref="Ranges"/>;</item>
    /// <item>Is in any of the set's <see cref="UnicodeCategories"/>;</item>
    /// <item>Or is matched by any of the set's <see cref="Nodes"/>.</item>
    /// </list>
    /// </remarks>
    [SuppressMessage ( "Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>" )]
    public sealed class Set : GrammarNode<Char>, IEquatable<Set?>
    {
        /// <summary>
        /// The characters matched by this set.
        /// </summary>
        public IImmutableSet<Char> Characters { get; }

        /// <summary>
        /// The ranges matched by this set.
        /// </summary>
        public IImmutableSet<Range<Char>> Ranges { get; }

        /// <summary>
        /// The unicode categories matched by this set.
        /// </summary>
        public IImmutableSet<UnicodeCategory> UnicodeCategories { get; }

        /// <summary>
        /// The nodes matched by this set.
        /// </summary>
        public IImmutableSet<GrammarNode<Char>> Nodes { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterSet;

        /// <summary>
        /// Initializes a new set from its components.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="ranges"></param>
        /// <param name="unicodeCategories"></param>
        /// <param name="nodes"></param>
        internal Set (
            IImmutableSet<Char> characters,
            IImmutableSet<Range<Char>> ranges,
            IImmutableSet<UnicodeCategory> unicodeCategories,
            IImmutableSet<GrammarNode<Char>> nodes )
        {
            this.Characters = characters ?? throw new ArgumentNullException ( nameof ( characters ) );
            this.Ranges = ranges;
            this.UnicodeCategories = unicodeCategories;
            this.Nodes = nodes;
        }

        /// <summary>
        /// Initializes a new alternation set.
        /// </summary>
        /// <param name="setElements">The elements of this set.</param>
        public Set ( params SetElement[] setElements )
        {
            if ( setElements is null )
                throw new ArgumentNullException ( nameof ( setElements ) );
            if ( setElements.Length < 1 )
                throw new ArgumentException ( "At least 1 set element is required.", nameof ( setElements ) );

            ImmutableHashSet<Char>.Builder characters = ImmutableHashSet.CreateBuilder<Char> ( );
            ImmutableHashSet<Range<Char>>.Builder ranges = ImmutableHashSet.CreateBuilder<Range<Char>> ( );
            ImmutableHashSet<UnicodeCategory>.Builder categories = ImmutableHashSet.CreateBuilder<UnicodeCategory> ( );
            ImmutableHashSet<GrammarNode<Char>>.Builder nodes = ImmutableHashSet.CreateBuilder<GrammarNode<Char>> ( );
            for ( var elementIdx = 0; elementIdx < setElements.Length; elementIdx++ )
            {
                SetElement setElement = setElements[elementIdx];
                switch ( setElement.Type )
                {
                    case SetElementType.Character:
                        characters.Add ( setElement.Character );
                        break;
                    case SetElementType.Range:
                        ranges.Add ( setElement.Range );
                        break;
                    case SetElementType.UnicodeCategory:
                        categories.Add ( setElement.UnicodeCategory );
                        break;
                    case SetElementType.Node:
                        nodes.Add ( setElement.Node );
                        break;
                    case SetElementType.Invalid:
                    default:
                        throw new InvalidOperationException ( $"Invalid set element provided at index {elementIdx}." );
                }
            }

            this.Characters = characters.ToImmutable ( );
            this.Ranges = ranges.ToImmutable ( );
            this.UnicodeCategories = categories.ToImmutable ( );
            this.Nodes = nodes.ToImmutable ( );
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) =>
            this.Equals ( obj as Set );

        /// <inheritdoc/>
        public Boolean Equals ( Set? other ) =>
            other != null
            && this.Characters.SetEquals ( other.Characters )
            && this.Ranges.SetEquals ( other.Ranges )
            && this.UnicodeCategories.SetEquals ( other.UnicodeCategories )
            && this.Nodes.SetEquals ( other.Nodes );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( )
        {
            var hash = new HashCode ( );
            foreach ( var character in this.Characters ) hash.Add ( character );
            foreach ( Range<Char> range in this.Ranges ) hash.Add ( range );
            foreach ( UnicodeCategory category in this.UnicodeCategories ) hash.Add ( category );
            foreach ( GrammarNode<Char> node in this.Nodes ) hash.Add ( node );
            return hash.ToHashCode ( );
        }

        /// <summary>
        /// Negates this set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        [SuppressMessage ( "Usage", "CA2225:Operator overloads have named alternates", Justification = "There is the Negate extension method." )]
        public static NegatedSet operator ! ( Set set )
        {
            if ( set is null )
                throw new ArgumentNullException ( nameof ( set ) );
            return new NegatedSet (
                set.Characters,
                set.Ranges,
                set.UnicodeCategories,
                set.Nodes );
        }

        /// <summary>
        /// Checks whether two sets are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == (Set? left, Set? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two sets are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( Set? left, Set? right ) =>
            !( left == right );
    }
}
