using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using GParse.Math;

namespace GParse.IO
{
    /// <summary>
    /// A source code reader
    /// </summary>
    public sealed class StringCodeReader : ICodeReader
    {
        /// <summary>
        /// A cache of compiled regex expressions
        /// </summary>
        private static readonly ConcurrentDictionary<String, Regex> _regexCache = new();

        /// <summary>
        /// The string containing the code being read
        /// </summary>
        private readonly String _code;

        private SourceLocation _cachedLocation;

        /// <inheritdoc />
        public Int32 Length => this._code.Length;

        #region Location Management

        /// <inheritdoc />
        public Int32 Position { get; private set; }

        #endregion Location Management

        /// <inheritdoc />
        public StringCodeReader(String str)
        {
            this._code = str ?? throw new ArgumentNullException(nameof(str));
            this._cachedLocation = SourceLocation.StartOfFile;
            this.Position = 0;
        }

        #region Location Management

        /// <inheritdoc />
        public SourceLocation GetLocation()
        {
            // In case the location we have cached is not the current location,
            // we recalculate it with the cached location as the last known
            // location.
            if (this._cachedLocation.Byte != this.Position)
            {
                this._cachedLocation = SourceLocation.Calculate(
                    this._code,
                    this.Position,
                    this._cachedLocation);
            }

            return this._cachedLocation;
        }

        /// <inheritdoc/>
        public SourceLocation GetLocation(Int32 position)
        {
            if (this._cachedLocation.Byte == position)
                return this._cachedLocation;
            else if (this._cachedLocation.Byte < position)
                return SourceLocation.Calculate(this._code, position, this._cachedLocation);
            else
                return SourceLocation.Calculate(this._code, position);
        }

        /// <inheritdoc/>
        public SourceRange GetLocation(Range<Int32> range)
        {
            if (range.Start <= this._cachedLocation.Byte)
                return SourceRange.Calculate(this._code, range, this._cachedLocation);
            else
                return SourceRange.Calculate(this._code, range);
        }

        #endregion Location Management

        /// <inheritdoc />
        public void Advance(Int32 offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (offset > this.Length - this.Position)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset is too big.");

            this.Position += offset;
        }

        #region Non-mutable Operations

        #region FindOffset

        /// <inheritdoc />
        public Int32 FindOffset(Char character)
        {
            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position. Even though this can be 2x slower than string.IndexOf(char)
            // on .NET Framework if the value is near the starting position, it can, on the other
            // hand, be 2x faster than string.IndexOf(char) on .NET Framework if the value is far from
            // the starting position.
            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position);
            return span.IndexOf(character);
        }

        /// <inheritdoc />
        public Int32 FindOffset(Char character, Int32 offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length || this.Position + offset >= this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position + offset);
            var result = span.IndexOf(character);
            return result == -1 ? result : result + offset;
        }

        /// <inheritdoc />
        public Int32 FindOffset(String str)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("The string must not be null or empty.", nameof(str));

            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position);
            return span.IndexOf(str.AsSpan(), StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public Int32 FindOffset(String str, Int32 offset)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("The string must not be null or empty.", nameof(str));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");

            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length || this.Position + offset >= this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position + offset);
            var result = span.IndexOf(str.AsSpan(), StringComparison.Ordinal);
            return result == -1 ? result : result + offset;
        }

        /// <inheritdoc />
        public Int32 FindOffset(Predicate<Char> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (this.Position == this.Length)
                return -1;

            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position);
            for (var i = 0; i < span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        public Int32 FindOffset(Predicate<Char> predicate, Int32 offset)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (this.Position == this.Length || this.Position + offset >= this.Length)
                return -1;

            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position + offset);
            for (var i = 0; i < span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    return i + offset;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        public Int32 FindOffset(ReadOnlySpan<Char> span)
        {
            if (span.IsEmpty)
                throw new ArgumentException("The span must not be empty.", nameof(span));

            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> subSpan = this._code.AsSpan(this.Position);
            return subSpan.IndexOf(span, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public Int32 FindOffset(ReadOnlySpan<Char> span, Int32 offset)
        {
            if (span.IsEmpty)
                throw new ArgumentException("The span must not be empty.", nameof(span));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");

            // Skip the IndexOf call if we're already at the end of the string
            if (this.Position == this.Length || this.Position + offset >= this.Length)
                return -1;

            // We get a slice (span) of the string from the current position until the end of it and
            // then return the result of IndexOf because the result is supposed to be relative to
            // our current position
            ReadOnlySpan<Char> subSpan = this._code.AsSpan(this.Position + offset);
            var result = subSpan.IndexOf(span, StringComparison.Ordinal);
            return result == -1 ? result : result + offset;
        }

        #endregion FindOffset

        #region IsNext

        /// <inheritdoc />
        public Boolean IsNext(Char ch) =>
            this.Position != this.Length && this._code[this.Position] == ch;

        /// <inheritdoc />
        public Boolean IsNext(String str)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("String cannot be null or empty", nameof(str));

            var len = str.Length;
            if (len > this.Length - this.Position)
                return false;

            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position);
            return span.StartsWith(str.AsSpan(), StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public Boolean IsNext(ReadOnlySpan<Char> span)
        {
            if (span.IsEmpty)
                throw new ArgumentException("The span must not be empty.", nameof(span));
            var len = span.Length;
            if (len > this.Length - this.Position)
                return false;

            ReadOnlySpan<Char> code = this._code.AsSpan(this.Position);
            return code.StartsWith(span, StringComparison.Ordinal);
        }

        #endregion IsNext

        #region IsAt

        /// <inheritdoc />
        public Boolean IsAt(Char character, Int32 offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            return this.Position + offset < this.Length && this._code[this.Position + offset] == character;
        }

        /// <inheritdoc />
        public Boolean IsAt(String str, Int32 offset)
        {
            if (String.IsNullOrEmpty(str))
                throw new ArgumentException("String cannot be null or empty.", nameof(str));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (str.Length > this.Length - (this.Position + offset))
                return false;

            ReadOnlySpan<Char> span = this._code.AsSpan(this.Position + offset);
            return span.StartsWith(str.AsSpan(), StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public Boolean IsAt(ReadOnlySpan<Char> span, Int32 offset)
        {
            if (span.IsEmpty)
                throw new ArgumentException("The span must not be empty.", nameof(span));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (span.Length > this.Length - (this.Position + offset))
                return false;

            ReadOnlySpan<Char> code = this._code.AsSpan(this.Position + offset);
            return code.StartsWith(span, StringComparison.Ordinal);
        }

        #endregion IsAt

        #region Peek

        /// <inheritdoc />
        public Char? Peek()
        {
            if (this.Position == this.Length)
                return null;

            return this._code[this.Position];
        }

        /// <inheritdoc />
        public Char? Peek(Int32 offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (offset >= this.Length - this.Position)
                return null;

            return this._code[this.Position + offset];
        }

        #endregion Peek

        #region PeekRegex

        /// <inheritdoc />
        public Match PeekRegex(String expression)
        {
            if (String.IsNullOrEmpty(expression))
                throw new ArgumentException($"'{nameof(expression)}' cannot be null or empty.", nameof(expression));
            if (!_regexCache.TryGetValue(expression, out Regex? regex))
            {
                regex = new Regex("\\G" + expression, RegexOptions.Compiled | RegexOptions.CultureInvariant);
                _regexCache[expression] = regex;
            }

            return regex.Match(this._code, this.Position);
        }

        /// <inheritdoc />
        public Match PeekRegex(Regex regex)
        {
            if (regex is null)
                throw new ArgumentNullException(nameof(regex));

            Match match = regex.Match(this._code, this.Position);
            if (match.Success && match.Index != this.Position)
                throw new ArgumentException("The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location.", nameof(regex));
            return match;
        }

        #endregion PeekRegex

        #region PeekString

        /// <inheritdoc />
        public String? PeekString(Int32 length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (length > this.Length - this.Position)
                length = this.Length - this.Position;

            return this._code.Substring(this.Position, length);
        }

        /// <inheritdoc />
        public String? PeekString(Int32 length, Int32 offset)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be positive.");
            if (this.Position + offset >= this.Length)
                return null;
            if (this.Position + offset + length >= this.Length)
                length = this.Length - this.Position - offset;

            return this._code.Substring(this.Position + offset, length);
        }

        #endregion PeekString

        #region PeekSpan

        /// <inheritdoc />
        public ReadOnlySpan<Char> PeekSpan(Int32 length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (this.Position + length >= this.Length)
                length = this.Length - this.Position;

            return this._code.AsSpan(this.Position, length);
        }

        /// <inheritdoc />
        public ReadOnlySpan<Char> PeekSpan(Int32 length, Int32 offset)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be positive.");
            if (this.Position + offset >= this.Length)
                return ReadOnlySpan<Char>.Empty;
            if (this.Position + offset + length >= this.Length)
                length = this.Length - this.Position - offset;

            return this._code.AsSpan(this.Position + offset, length);
        }

        #endregion PeekSpan

        #endregion Non-mutable Operations

        #region Mutable Operations

        #region Read

        /// <inheritdoc />
        public Char? Read()
        {
            if (this.Position == this.Length)
                return null;

            // Maybe use try-finally here?
            var @return = this._code[this.Position];
            this.Advance(1);
            return @return;
        }

        /// <inheritdoc />
        public Char? Read(Int32 offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset must be positive.");
            if (offset == 0)
                return this.Read();
            if (offset >= this.Length - this.Position)
                return null;

            // Maybe use try-finally here?
            var @return = this._code[this.Position + offset];
            this.Advance(offset + 1);
            return @return;
        }

        #endregion Read

        #region ReadLine

        /// <inheritdoc />
        public String ReadLine()
        {
            // Read until CR or LF
            var offset = this._code.AsSpan(this.Position)
                                   .IndexOfAny('\r', '\n');
            if (offset < 0) offset = this.Length - this.Position;
            var line = this.ReadString(offset)!;
            // Read the CR or LF
            var read = this.Read();
            // Then check for CR+LF and skip LF if any
            if (read == '\r' && this.Peek() == '\n')
                this.Advance(1);
            return line;
        }

        #endregion ReadLine

        #region ReadString

        /// <inheritdoc />
        public String? ReadString(Int32 length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (length > this.Length - this.Position)
                length = this.Length - this.Position;
            if (length == 0)
                return String.Empty;

            // Maybe use try-finally here?
            var @return = this._code.Substring(this.Position, length);
            this.Advance(length);
            return @return;
        }

        #endregion ReadString

        #region ReadStringUntil

        /// <inheritdoc />
        public String ReadStringUntil(Char delim)
        {
            var length = this.FindOffset(delim);
            if (length > -1)
                return this.ReadString(length)!;
            else
                return this.ReadToEnd();
        }

        /// <inheritdoc />
        public String ReadStringUntil(String delim)
        {
            var length = this.FindOffset(delim);
            if (length > -1)
                return this.ReadString(length)!;
            else
                return this.ReadToEnd();
        }

        /// <inheritdoc />
        public String ReadStringUntil(Predicate<Char> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var length = this.FindOffset(filter);
            if (length > -1)
                return this.ReadString(length)!;
            else
                return this.ReadToEnd();
        }

        #endregion ReadStringUntil

        #region ReadStringWhile

        /// <inheritdoc />
        public String ReadStringWhile(Predicate<Char> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var length = this.FindOffset(v => !filter(v));
            if (length > -1)
                return this.ReadString(length)!;
            else
                return this.ReadToEnd();
        }

        #endregion ReadStringWhile

        #region ReadToEnd

        /// <inheritdoc />
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppression valid for some target frameworks.")]
        [SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Not all target frameworks have it.")]
        public String ReadToEnd()
        {
            var ret = this._code.Substring(this.Position);
            this.Advance(this.Length - this.Position);
            return ret;
        }

        #endregion ReadToEnd

        #region ReadSpanLine

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanLine()
        {
            // Read until CR or LF
            var offset = this._code.AsSpan(this.Position)
                                   .IndexOfAny('\r', '\n');
            if (offset < 0) offset = this.Length - this.Position;
            ReadOnlySpan<Char> line = this.ReadSpan(offset)!;
            // Read the CR or LF
            var read = this.Read();
            // Then check for CR+LF and skip LF if any
            if (read == '\r' && this.Peek() == '\n')
                this.Advance(1);
            return line;
        }

        #endregion ReadSpanLine

        #region ReadSpan

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpan(Int32 length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            if (length > this.Length - this.Position)
                length = this.Length - this.Position;
            if (length == 0)
                return ReadOnlySpan<Char>.Empty;

            // Maybe use try-finally here?
            ReadOnlySpan<Char> @return = this._code.AsSpan(this.Position, length);
            this.Advance(length);
            return @return;
        }

        #endregion ReadSpan

        #region ReadSpanUntil

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanUntil(Char delim)
        {
            var length = this.FindOffset(delim);
            if (length > -1)
                return this.ReadSpan(length);
            else
                return this.ReadSpanToEnd();
        }

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanUntil(String delim)
        {
            var length = this.FindOffset(delim);
            if (length > -1)
                return this.ReadSpan(length);
            else
                return this.ReadSpanToEnd();
        }

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanUntil(Predicate<Char> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var length = this.FindOffset(filter);
            if (length > -1)
                return this.ReadSpan(length);
            else
                return this.ReadSpanToEnd();
        }

        #endregion ReadSpanUntil

        #region ReadSpanWhile

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanWhile(Predicate<Char> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var length = this.FindOffset(v => !filter(v));
            if (length > -1)
                return this.ReadSpan(length);
            else
                return this.ReadSpanToEnd();
        }

        #endregion ReadSpanWhile

        #region ReadSpanToEnd

        /// <inheritdoc />
        public ReadOnlySpan<Char> ReadSpanToEnd()
        {
            ReadOnlySpan<Char> ret = this._code.AsSpan(this.Position);
            this.Advance(this.Length - this.Position);
            return ret;
        }

        #endregion ReadSpanToEnd

        #region MatchRegex

        /// <inheritdoc />
        public Match MatchRegex(String expression)
        {
            if (String.IsNullOrEmpty(expression))
                throw new ArgumentException($"'{nameof(expression)}' cannot be null or empty.", nameof(expression));

            if (!_regexCache.TryGetValue(expression, out Regex? regex))
            {
                regex = new Regex("\\G" + expression, RegexOptions.Compiled | RegexOptions.CultureInvariant);
                _regexCache[expression] = regex;
            }

            Match match = regex.Match(this._code, this.Position);
            if (match.Success)
                this.Advance(match.Length);
            return match;
        }

        /// <inheritdoc />
        public Match MatchRegex(Regex regex)
        {
            if (regex is null)
                throw new ArgumentNullException(nameof(regex));

            Match match = regex.Match(this._code, this.Position);
            if (match.Success)
            {
                if (match.Index != this.Position)
                    throw new ArgumentException("The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location.", nameof(regex));
                this.Advance(match.Length);
            }
            return match;
        }

        #endregion MatchRegex

        #endregion Mutable Operations

        #region Position Manipulation

        /// <inheritdoc />
        public void Reset()
        {
            this.Position = 0;
            this._cachedLocation = SourceLocation.StartOfFile;
        }

        /// <inheritdoc />
        public void Restore(SourceLocation location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));
            if (location.Line < 1 || location.Column < 1 || location.Byte < 0)
                throw new Exception("Invalid rewind position.");
            else if (location.Byte == this.Position)
                return;

            this.Position = location.Byte;
            this._cachedLocation = location;
        }

        /// <inheritdoc/>
        public void Restore(Int32 position)
        {
            if (this.Position == position)
                return;

            if (this._cachedLocation.Byte < position)
                this._cachedLocation = SourceLocation.StartOfFile;
            this.Position = position;
        }

        #endregion Position Manipulation
    }
}