using GParse.Common;
using GParse.Common.IO;
using GParse.Lexing.Settings;
using System;

namespace GParse.Lexing.Tests
{
	/// <summary>
	/// Partial C lexer
	/// </summary>
	internal class TestLexer : LexerBase
	{
		private readonly CharLexSettings charSettingsUTF8 = new CharLexSettings
		{
			BinaryEscapePrefix = null,
			DecimalEscapePrefix = null,
			// Max = \177777
			OctalEscapePrefix = "\\",
			OctalEscapeMaxLengh = 6,
			// Max = \xFFFF
			HexadecimalEscapePrefix = "\\x",
			HexadecimalEscapeMaxLengh = 4
		};

		public TestLexer ( String input ) : base ( input )
		{
			this.consumeNewlinesAutomatically = true;

			this.charSettings = new CharLexSettings
			{
				BinaryEscapePrefix = null,
				DecimalEscapePrefix = null,
				// Max = \377
				OctalEscapePrefix = "\\",
				OctalEscapeMaxLengh = 3,
				// Max = \xFF
				HexadecimalEscapePrefix = "\\x",
				HexadecimalEscapeMaxLengh = 2
			};

			this.stringSettings = new StringLexSettings
			{
				CharSettings = this.charSettings,
				NewlineEscape = "\\"
			};

			this.numberSettings = new IntegerLexSettings
			{
				BinaryPrefix = null,
				DecimalPrefix = null,
				DefaultType = IntegerLexSettings.NumberType.Decimal,
				HexadecimalPrefix = "0x",
				OctalPrefix = "0"
			};

			#region Character Escapes

			this.charSettings
				.RegisterEscapeConstant ( @"\a", '\a' )
				.RegisterEscapeConstant ( @"\b", '\b' )
				.RegisterEscapeConstant ( @"\f", '\f' )
				.RegisterEscapeConstant ( @"\n", '\n' )
				.RegisterEscapeConstant ( @"\r", '\r' )
				.RegisterEscapeConstant ( @"\t", '\t' )
				.RegisterEscapeConstant ( @"\v", '\v' )
				.RegisterEscapeConstant ( @"\'", '\'' )
				.RegisterEscapeConstant ( @"\""", '"' )
				.RegisterEscapeConstant ( @"\\", '\\' )
				.RegisterEscapeConstant ( @"\?", '?' );

			this.charSettingsUTF8
				.RegisterEscapeConstant ( @"\a", '\a' )
				.RegisterEscapeConstant ( @"\b", '\b' )
				.RegisterEscapeConstant ( @"\f", '\f' )
				.RegisterEscapeConstant ( @"\n", '\n' )
				.RegisterEscapeConstant ( @"\r", '\r' )
				.RegisterEscapeConstant ( @"\t", '\t' )
				.RegisterEscapeConstant ( @"\v", '\v' )
				.RegisterEscapeConstant ( @"\'", '\'' )
				.RegisterEscapeConstant ( @"\""", '"' )
				.RegisterEscapeConstant ( @"\\", '\\' )
				.RegisterEscapeConstant ( @"\?", '?' );

			#endregion Character Escapes

			this.tokenManager
				.AddToken ( "If", "if", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
				.AddToken ( "Else", "else", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
				.AddToken ( "Return", "return", TokenType.Keyword, ch => ch == '(' || ch == ' ' || ch == '\t' || ch == ';' )
				.AddToken ( "True", "true", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
				.AddToken ( "False", "false", TokenType.Keyword, ch => !IsIdentifierChar ( ch ) )
				.AddToken ( "LCurly", "{", TokenType.LCurly )
				.AddToken ( "RCurly", "}", TokenType.RCurly )
				.AddToken ( "LParen", "(", TokenType.LParen )
				.AddToken ( "RParen", ")", TokenType.RParen )
				.AddToken ( "&", "&", TokenType.Operator )
				.AddToken ( "And", "&&", TokenType.Operator )
				.AddToken ( "BOr", "|", TokenType.Operator )
				.AddToken ( "Or", "||", TokenType.Operator )
				.AddToken ( "Not", "!", TokenType.Operator )
				.AddToken ( "NotEq", "!=", TokenType.Operator )
				.AddToken ( "Eq", "==", TokenType.Operator )
				.AddToken ( "Semicolon", ";", TokenType.Punctuation );
		}

		private static Boolean IsIdentifierFirstChar ( Char ch ) => Char.IsLetter ( ch ) || ch == '_';

		private static Boolean IsIdentifierChar ( Char ch ) => Char.IsLetterOrDigit ( ch ) || ch == '_';

		protected override Boolean TryReadToken ( out Token tok )
		{
			tok = null;
			SourceLocation start = this.Location;
			this.reader.Save ( );

			if ( this.reader.Peek ( ) == '"' )
			{
				this.reader.Advance ( 1 );

				if ( this.TryReadString ( "\"", out String value, out String raw, false, this.stringSettings ) )
				{
					tok = new Token ( "String Literal", $"\"{raw}\"", value, TokenType.String, start.To ( this.Location ) );
				}
			}
			else if ( this.reader.Peek ( ) == '\'' )
			{
				this.reader.Advance ( 1 );

				if ( this.TryReadChar ( "'", out Char value, out String raw, this.charSettings ) )
				{
					tok = new Token ( "Char Literal", $"'{raw}'", value, TokenType.Char, start.To ( this.Location ) );
				}
			}
			else if ( this.TryReadInteger ( out String raw, out Int64 value, this.numberSettings ) )
			{
				tok = new Token ( "Number Literal", raw, value, TokenType.Number, start.To ( this.Location ) );
			}
			else if ( IsIdentifierFirstChar ( ( Char ) this.reader.Peek ( ) ) )
			{
				var id = this.reader.ReadStringWhile ( IsIdentifierChar );
				tok = new Token ( "Identifier", id, id, TokenType.Identifier, start.To ( this.Location ) );
			}
			else
			{
				this.reader.LoadSave ( );
				return false;
			}

			this.reader.DiscardSave ( );
			return true;
		}
	}
}
