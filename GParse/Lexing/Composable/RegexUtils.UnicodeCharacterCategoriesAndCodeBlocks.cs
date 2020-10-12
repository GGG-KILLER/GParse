using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Contains common character classes.
    /// </summary>
    public static partial class RegexUtils
    {
        /// <summary>
        /// The class containing all regex unicode character categories and code blocks.
        /// </summary>
        public static class CharacterCategories
        {
            #region Fields

            /// <summary>
            /// The Lu character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lu = new UnicodeCategoryTerminal ( UnicodeCategory.UppercaseLetter ); // CategoryFlagSet(Letter, Uppercase)

            /// <summary>
            /// The Ll character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Ll = new UnicodeCategoryTerminal ( UnicodeCategory.LowercaseLetter ); // CategoryFlagSet(Letter, Lowercase)

            /// <summary>
            /// The Lt character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lt = new UnicodeCategoryTerminal ( UnicodeCategory.TitlecaseLetter ); // CategoryFlagSet(Letter, Titlecase)

            /// <summary>
            /// The Lm character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lm = new UnicodeCategoryTerminal ( UnicodeCategory.ModifierLetter ); // CategoryFlagSet(Letter, Modifier)

            /// <summary>
            /// The Lo character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lo = new UnicodeCategoryTerminal ( UnicodeCategory.OtherLetter ); // CategoryFlagSet(Letter, Other)

            /// <summary>
            /// The L character category/block node.
            /// </summary>
            public static readonly Alternation<Char> L = new Alternation<Char> ( Lu, Ll, Lt, Lm, Lo ); // CategoryFlagSet(Lu, Ll, Lt, Lm, Lo)

            /// <summary>
            /// The Mn character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Mn = new UnicodeCategoryTerminal ( UnicodeCategory.NonSpacingMark ); // CategoryFlagSet(Mark, Nonspacing)

            /// <summary>
            /// The Mc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Mc = new UnicodeCategoryTerminal ( UnicodeCategory.SpacingCombiningMark ); // CategoryFlagSet(Mark, SpacingCombining)

            /// <summary>
            /// The Me character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Me = new UnicodeCategoryTerminal ( UnicodeCategory.EnclosingMark ); // CategoryFlagSet(Mark, Enclosing)

            /// <summary>
            /// The M character category/block node.
            /// </summary>
            public static readonly Alternation<Char> M = new Alternation<Char> ( Mn, Mc, Me ); // CategoryFlagSet(Mn, Mc, Me)

            /// <summary>
            /// The Nd character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Nd = new UnicodeCategoryTerminal ( UnicodeCategory.DecimalDigitNumber ); // CategoryFlagSet(Number, DecimalDigit)

            /// <summary>
            /// The Nl character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Nl = new UnicodeCategoryTerminal ( UnicodeCategory.LetterNumber ); // CategoryFlagSet(Number, Letter)

            /// <summary>
            /// The No character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal No = new UnicodeCategoryTerminal ( UnicodeCategory.OtherNumber ); // CategoryFlagSet(Number, Other)

            /// <summary>
            /// The N character category/block node.
            /// </summary>
            public static readonly Alternation<Char> N = new Alternation<Char> ( Nd, Nl, No ); // CategoryFlagSet(Nd, Nl, No)

            /// <summary>
            /// The Pc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pc = new UnicodeCategoryTerminal ( UnicodeCategory.ConnectorPunctuation ); // CategoryFlagSet(Punctuation, Connector)

            /// <summary>
            /// The Pd character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pd = new UnicodeCategoryTerminal ( UnicodeCategory.DashPunctuation ); // CategoryFlagSet(Punctuation, Dash)

            /// <summary>
            /// The Ps character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Ps = new UnicodeCategoryTerminal ( UnicodeCategory.OpenPunctuation ); // CategoryFlagSet(Punctuation, Open)

            /// <summary>
            /// The Pe character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pe = new UnicodeCategoryTerminal ( UnicodeCategory.ClosePunctuation ); // CategoryFlagSet(Punctuation, Close)

            /// <summary>
            /// The Pi character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pi = new UnicodeCategoryTerminal ( UnicodeCategory.InitialQuotePunctuation ); // CategoryFlagSet(Punctuation, InitialQuote)

            /// <summary>
            /// The Pf character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pf = new UnicodeCategoryTerminal ( UnicodeCategory.FinalQuotePunctuation ); // CategoryFlagSet(Punctuation, FinalQuote)

            /// <summary>
            /// The Po character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Po = new UnicodeCategoryTerminal ( UnicodeCategory.OtherPunctuation ); // CategoryFlagSet(Punctuation, Other)

            /// <summary>
            /// The P character category/block node.
            /// </summary>
            public static readonly Alternation<Char> P = new Alternation<Char> ( Pc, Pd, Ps, Pe, Pi, Pf, Po ); // CategoryFlagSet(Pc, Pd, Ps, Pe, Pi, Pf, Po)

            /// <summary>
            /// The Sm character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sm = new UnicodeCategoryTerminal ( UnicodeCategory.MathSymbol ); // CategoryFlagSet(Symbol, Math)

            /// <summary>
            /// The Sc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sc = new UnicodeCategoryTerminal ( UnicodeCategory.CurrencySymbol ); // CategoryFlagSet(Symbol, Currency)

            /// <summary>
            /// The Sk character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sk = new UnicodeCategoryTerminal ( UnicodeCategory.ModifierSymbol ); // CategoryFlagSet(Symbol, Modifier)

            /// <summary>
            /// The So character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal So = new UnicodeCategoryTerminal ( UnicodeCategory.OtherSymbol ); // CategoryFlagSet(Symbol, Other)

            /// <summary>
            /// The S character category/block node.
            /// </summary>
            public static readonly Alternation<Char> S = new Alternation<Char> ( Sm, Sc, Sk, So ); // CategoryFlagSet(Sm, Sc, Sk, So)

            /// <summary>
            /// The Zs character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zs = new UnicodeCategoryTerminal ( UnicodeCategory.SpaceSeparator ); // CategoryFlagSet(Separator, Space)

            /// <summary>
            /// The Zl character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zl = new UnicodeCategoryTerminal ( UnicodeCategory.LineSeparator ); // CategoryFlagSet(Separator, Line)

            /// <summary>
            /// The Zp character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zp = new UnicodeCategoryTerminal ( UnicodeCategory.ParagraphSeparator ); // CategoryFlagSet(Separator, Paragraph)

            /// <summary>
            /// The Z character category/block node.
            /// </summary>
            public static readonly Alternation<Char> Z = new Alternation<Char> ( Zs, Zl, Zp ); // CategoryFlagSet(Zs, Zl, Zp)

            /// <summary>
            /// The Cc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cc = new UnicodeCategoryTerminal ( UnicodeCategory.Control ); // CategoryFlagSet(Control)

            /// <summary>
            /// The Cf character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cf = new UnicodeCategoryTerminal ( UnicodeCategory.Format ); // CategoryFlagSet(Format)

            /// <summary>
            /// The Cs character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cs = new UnicodeCategoryTerminal ( UnicodeCategory.Surrogate ); // CategoryFlagSet(Surrogate)

            /// <summary>
            /// The Co character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Co = new UnicodeCategoryTerminal ( UnicodeCategory.PrivateUse ); // CategoryFlagSet(PrivateUse)

            /// <summary>
            /// The Cn character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cn = new UnicodeCategoryTerminal ( UnicodeCategory.OtherNotAssigned ); // CategoryFlagSet(Other, NotAssigned)

            /// <summary>
            /// The C character category/block node.
            /// </summary>
            public static readonly Alternation<Char> C = new Alternation<Char> ( Cc, Cf, Cs, Co, Cn ); // CategoryFlagSet(Cc, Cf, Cs, Co, Cn)

            /// <summary>
            /// The IsBasicLatin character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBasicLatin = new CharacterRange ( '\u0000', '\u007F' ); // Range(0000-007F)

            /// <summary>
            /// The IsLatin-1Supplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatin1Supplement = new CharacterRange ( '\u0080', '\u00FF' ); // Range(0080-00FF)

            /// <summary>
            /// The IsLatinExtended-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedA = new CharacterRange ( '\u0100', '\u017F' ); // Range(0100-017F)

            /// <summary>
            /// The IsLatinExtended-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedB = new CharacterRange ( '\u0180', '\u024F' ); // Range(0180-024F)

            /// <summary>
            /// The IsIPAExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsIPAExtensions = new CharacterRange ( '\u0250', '\u02AF' ); // Range(0250-02AF)

            /// <summary>
            /// The IsSpacingModifierLetters character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSpacingModifierLetters = new CharacterRange ( '\u02B0', '\u02FF' ); // Range(02B0-02FF)

            /// <summary>
            /// The IsCombiningDiacriticalMarks character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningDiacriticalMarks = new CharacterRange ( '\u0300', '\u036F' ); // Range(0300-036F)

            /// <summary>
            /// The IsGreek character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreek = new CharacterRange ( '\u0370', '\u03FF' ); // Range(0370-03FF)

            /// <summary>
            /// The IsGreekandCoptic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreekandCoptic = new CharacterRange ( '\u0370', '\u03FF' ); // Range(0370-03FF)

            /// <summary>
            /// The IsCyrillic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCyrillic = new CharacterRange ( '\u0400', '\u04FF' ); // Range(0400-04FF)

            /// <summary>
            /// The IsCyrillicSupplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCyrillicSupplement = new CharacterRange ( '\u0500', '\u052F' ); // Range(0500-052F)

            /// <summary>
            /// The IsArmenian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArmenian = new CharacterRange ( '\u0530', '\u058F' ); // Range(0530-058F)

            /// <summary>
            /// The IsHebrew character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHebrew = new CharacterRange ( '\u0590', '\u05FF' ); // Range(0590-05FF)

            /// <summary>
            /// The IsArabic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabic = new CharacterRange ( '\u0600', '\u06FF' ); // Range(0600-06FF)

            /// <summary>
            /// The IsSyriac character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSyriac = new CharacterRange ( '\u0700', '\u074F' ); // Range(0700-074F)

            /// <summary>
            /// The IsThaana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsThaana = new CharacterRange ( '\u0780', '\u07BF' ); // Range(0780-07BF)

            /// <summary>
            /// The IsDevanagari character category/block node.
            /// </summary>
            public static readonly CharacterRange IsDevanagari = new CharacterRange ( '\u0900', '\u097F' ); // Range(0900-097F)

            /// <summary>
            /// The IsBengali character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBengali = new CharacterRange ( '\u0980', '\u09FF' ); // Range(0980-09FF)

            /// <summary>
            /// The IsGurmukhi character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGurmukhi = new CharacterRange ( '\u0A00', '\u0A7F' ); // Range(0A00-0A7F)

            /// <summary>
            /// The IsGujarati character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGujarati = new CharacterRange ( '\u0A80', '\u0AFF' ); // Range(0A80-0AFF)

            /// <summary>
            /// The IsOriya character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOriya = new CharacterRange ( '\u0B00', '\u0B7F' ); // Range(0B00-0B7F)

            /// <summary>
            /// The IsTamil character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTamil = new CharacterRange ( '\u0B80', '\u0BFF' ); // Range(0B80-0BFF)

            /// <summary>
            /// The IsTelugu character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTelugu = new CharacterRange ( '\u0C00', '\u0C7F' ); // Range(0C00-0C7F)

            /// <summary>
            /// The IsKannada character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKannada = new CharacterRange ( '\u0C80', '\u0CFF' ); // Range(0C80-0CFF)

            /// <summary>
            /// The IsMalayalam character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMalayalam = new CharacterRange ( '\u0D00', '\u0D7F' ); // Range(0D00-0D7F)

            /// <summary>
            /// The IsSinhala character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSinhala = new CharacterRange ( '\u0D80', '\u0DFF' ); // Range(0D80-0DFF)

            /// <summary>
            /// The IsThai character category/block node.
            /// </summary>
            public static readonly CharacterRange IsThai = new CharacterRange ( '\u0E00', '\u0E7F' ); // Range(0E00-0E7F)

            /// <summary>
            /// The IsLao character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLao = new CharacterRange ( '\u0E80', '\u0EFF' ); // Range(0E80-0EFF)

            /// <summary>
            /// The IsTibetan character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTibetan = new CharacterRange ( '\u0F00', '\u0FFF' ); // Range(0F00-0FFF)

            /// <summary>
            /// The IsMyanmar character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMyanmar = new CharacterRange ( '\u1000', '\u109F' ); // Range(1000-109F)

            /// <summary>
            /// The IsGeorgian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeorgian = new CharacterRange ( '\u10A0', '\u10FF' ); // Range(10A0-10FF)

            /// <summary>
            /// The IsHangulJamo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulJamo = new CharacterRange ( '\u1100', '\u11FF' ); // Range(1100-11FF)

            /// <summary>
            /// The IsEthiopic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEthiopic = new CharacterRange ( '\u1200', '\u137F' ); // Range(1200-137F)

            /// <summary>
            /// The IsCherokee character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCherokee = new CharacterRange ( '\u13A0', '\u13FF' ); // Range(13A0-13FF)

            /// <summary>
            /// The IsUnifiedCanadianAboriginalSyllabics character category/block node.
            /// </summary>
            public static readonly CharacterRange IsUnifiedCanadianAboriginalSyllabics = new CharacterRange ( '\u1400', '\u167F' ); // Range(1400-167F)

            /// <summary>
            /// The IsOgham character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOgham = new CharacterRange ( '\u1680', '\u169F' ); // Range(1680-169F)

            /// <summary>
            /// The IsRunic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsRunic = new CharacterRange ( '\u16A0', '\u16FF' ); // Range(16A0-16FF)

            /// <summary>
            /// The IsTagalog character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTagalog = new CharacterRange ( '\u1700', '\u171F' ); // Range(1700-171F)

            /// <summary>
            /// The IsHanunoo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHanunoo = new CharacterRange ( '\u1720', '\u173F' ); // Range(1720-173F)

            /// <summary>
            /// The IsBuhid character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBuhid = new CharacterRange ( '\u1740', '\u175F' ); // Range(1740-175F)

            /// <summary>
            /// The IsTagbanwa character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTagbanwa = new CharacterRange ( '\u1760', '\u177F' ); // Range(1760-177F)

            /// <summary>
            /// The IsKhmer character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKhmer = new CharacterRange ( '\u1780', '\u17FF' ); // Range(1780-17FF)

            /// <summary>
            /// The IsMongolian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMongolian = new CharacterRange ( '\u1800', '\u18AF' ); // Range(1800-18AF)

            /// <summary>
            /// The IsLimbu character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLimbu = new CharacterRange ( '\u1900', '\u194F' ); // Range(1900-194F)

            /// <summary>
            /// The IsTaiLe character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTaiLe = new CharacterRange ( '\u1950', '\u197F' ); // Range(1950-197F)

            /// <summary>
            /// The IsKhmerSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKhmerSymbols = new CharacterRange ( '\u19E0', '\u19FF' ); // Range(19E0-19FF)

            /// <summary>
            /// The IsPhoneticExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsPhoneticExtensions = new CharacterRange ( '\u1D00', '\u1D7F' ); // Range(1D00-1D7F)

            /// <summary>
            /// The IsLatinExtendedAdditional character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedAdditional = new CharacterRange ( '\u1E00', '\u1EFF' ); // Range(1E00-1EFF)

            /// <summary>
            /// The IsGreekExtended character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreekExtended = new CharacterRange ( '\u1F00', '\u1FFF' ); // Range(1F00-1FFF)

            /// <summary>
            /// The IsGeneralPunctuation character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeneralPunctuation = new CharacterRange ( '\u2000', '\u206F' ); // Range(2000-206F)

            /// <summary>
            /// The IsSuperscriptsandSubscripts character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSuperscriptsandSubscripts = new CharacterRange ( '\u2070', '\u209F' ); // Range(2070-209F)

            /// <summary>
            /// The IsCurrencySymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCurrencySymbols = new CharacterRange ( '\u20A0', '\u20CF' ); // Range(20A0-20CF)

            /// <summary>
            /// The IsCombiningDiacriticalMarksforSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningDiacriticalMarksforSymbols = new CharacterRange ( '\u20D0', '\u20FF' ); // Range(20D0-20FF)

            /// <summary>
            /// The IsCombiningMarksforSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningMarksforSymbols = new CharacterRange ( '\u20D0', '\u20FF' ); // Range(20D0-20FF)

            /// <summary>
            /// The IsLetterlikeSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLetterlikeSymbols = new CharacterRange ( '\u2100', '\u214F' ); // Range(2100-214F)

            /// <summary>
            /// The IsNumberForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsNumberForms = new CharacterRange ( '\u2150', '\u218F' ); // Range(2150-218F)

            /// <summary>
            /// The IsArrows character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArrows = new CharacterRange ( '\u2190', '\u21FF' ); // Range(2190-21FF)

            /// <summary>
            /// The IsMathematicalOperators character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMathematicalOperators = new CharacterRange ( '\u2200', '\u22FF' ); // Range(2200-22FF)

            /// <summary>
            /// The IsMiscellaneousTechnical character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousTechnical = new CharacterRange ( '\u2300', '\u23FF' ); // Range(2300-23FF)

            /// <summary>
            /// The IsControlPictures character category/block node.
            /// </summary>
            public static readonly CharacterRange IsControlPictures = new CharacterRange ( '\u2400', '\u243F' ); // Range(2400-243F)

            /// <summary>
            /// The IsOpticalCharacterRecognition character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOpticalCharacterRecognition = new CharacterRange ( '\u2440', '\u245F' ); // Range(2440-245F)

            /// <summary>
            /// The IsEnclosedAlphanumerics character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEnclosedAlphanumerics = new CharacterRange ( '\u2460', '\u24FF' ); // Range(2460-24FF)

            /// <summary>
            /// The IsBoxDrawing character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBoxDrawing = new CharacterRange ( '\u2500', '\u257F' ); // Range(2500-257F)

            /// <summary>
            /// The IsBlockElements character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBlockElements = new CharacterRange ( '\u2580', '\u259F' ); // Range(2580-259F)

            /// <summary>
            /// The IsGeometricShapes character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeometricShapes = new CharacterRange ( '\u25A0', '\u25FF' ); // Range(25A0-25FF)

            /// <summary>
            /// The IsMiscellaneousSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousSymbols = new CharacterRange ( '\u2600', '\u26FF' ); // Range(2600-26FF)

            /// <summary>
            /// The IsDingbats character category/block node.
            /// </summary>
            public static readonly CharacterRange IsDingbats = new CharacterRange ( '\u2700', '\u27BF' ); // Range(2700-27BF)

            /// <summary>
            /// The IsMiscellaneousMathematicalSymbols-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousMathematicalSymbolsA = new CharacterRange ( '\u27C0', '\u27EF' ); // Range(27C0-27EF)

            /// <summary>
            /// The IsSupplementalArrows-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalArrowsA = new CharacterRange ( '\u27F0', '\u27FF' ); // Range(27F0-27FF)

            /// <summary>
            /// The IsBraillePatterns character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBraillePatterns = new CharacterRange ( '\u2800', '\u28FF' ); // Range(2800-28FF)

            /// <summary>
            /// The IsSupplementalArrows-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalArrowsB = new CharacterRange ( '\u2900', '\u297F' ); // Range(2900-297F)

            /// <summary>
            /// The IsMiscellaneousMathematicalSymbols-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousMathematicalSymbolsB = new CharacterRange ( '\u2980', '\u29FF' ); // Range(2980-29FF)

            /// <summary>
            /// The IsSupplementalMathematicalOperators character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalMathematicalOperators = new CharacterRange ( '\u2A00', '\u2AFF' ); // Range(2A00-2AFF)

            /// <summary>
            /// The IsMiscellaneousSymbolsandArrows character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousSymbolsandArrows = new CharacterRange ( '\u2B00', '\u2BFF' ); // Range(2B00-2BFF)

            /// <summary>
            /// The IsCJKRadicalsSupplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKRadicalsSupplement = new CharacterRange ( '\u2E80', '\u2EFF' ); // Range(2E80-2EFF)

            /// <summary>
            /// The IsKangxiRadicals character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKangxiRadicals = new CharacterRange ( '\u2F00', '\u2FDF' ); // Range(2F00-2FDF)

            /// <summary>
            /// The IsIdeographicDescriptionCharacters character category/block node.
            /// </summary>
            public static readonly CharacterRange IsIdeographicDescriptionCharacters = new CharacterRange ( '\u2FF0', '\u2FFF' ); // Range(2FF0-2FFF)

            /// <summary>
            /// The IsCJKSymbolsandPunctuation character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKSymbolsandPunctuation = new CharacterRange ( '\u3000', '\u303F' ); // Range(3000-303F)

            /// <summary>
            /// The IsHiragana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHiragana = new CharacterRange ( '\u3040', '\u309F' ); // Range(3040-309F)

            /// <summary>
            /// The IsKatakana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKatakana = new CharacterRange ( '\u30A0', '\u30FF' ); // Range(30A0-30FF)

            /// <summary>
            /// The IsBopomofo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBopomofo = new CharacterRange ( '\u3100', '\u312F' ); // Range(3100-312F)

            /// <summary>
            /// The IsHangulCompatibilityJamo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulCompatibilityJamo = new CharacterRange ( '\u3130', '\u318F' ); // Range(3130-318F)

            /// <summary>
            /// The IsKanbun character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKanbun = new CharacterRange ( '\u3190', '\u319F' ); // Range(3190-319F)

            /// <summary>
            /// The IsBopomofoExtended character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBopomofoExtended = new CharacterRange ( '\u31A0', '\u31BF' ); // Range(31A0-31BF)

            /// <summary>
            /// The IsKatakanaPhoneticExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKatakanaPhoneticExtensions = new CharacterRange ( '\u31F0', '\u31FF' ); // Range(31F0-31FF)

            /// <summary>
            /// The IsEnclosedCJKLettersandMonths character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEnclosedCJKLettersandMonths = new CharacterRange ( '\u3200', '\u32FF' ); // Range(3200-32FF)

            /// <summary>
            /// The IsCJKCompatibility character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibility = new CharacterRange ( '\u3300', '\u33FF' ); // Range(3300-33FF)

            /// <summary>
            /// The IsCJKUnifiedIdeographsExtensionA character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKUnifiedIdeographsExtensionA = new CharacterRange ( '\u3400', '\u4DBF' ); // Range(3400-4DBF)

            /// <summary>
            /// The IsYijingHexagramSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYijingHexagramSymbols = new CharacterRange ( '\u4DC0', '\u4DFF' ); // Range(4DC0-4DFF)

            /// <summary>
            /// The IsCJKUnifiedIdeographs character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKUnifiedIdeographs = new CharacterRange ( '\u4E00', '\u9FFF' ); // Range(4E00-9FFF)

            /// <summary>
            /// The IsYiSyllables character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYiSyllables = new CharacterRange ( '\uA000', '\uA48F' ); // Range(A000-A48F)

            /// <summary>
            /// The IsYiRadicals character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYiRadicals = new CharacterRange ( '\uA490', '\uA4CF' ); // Range(A490-A4CF)

            /// <summary>
            /// The IsHangulSyllables character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulSyllables = new CharacterRange ( '\uAC00', '\uD7AF' ); // Range(AC00-D7AF)

            /// <summary>
            /// The IsHighSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHighSurrogates = new CharacterRange ( '\uD800', '\uDB7F' ); // Range(D800-DB7F)

            /// <summary>
            /// The IsHighPrivateUseSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHighPrivateUseSurrogates = new CharacterRange ( '\uDB80', '\uDBFF' ); // Range(DB80-DBFF)

            /// <summary>
            /// The IsLowSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLowSurrogates = new CharacterRange ( '\uDC00', '\uDFFF' ); // Range(DC00-DFFF)

            /// <summary>
            /// The IsPrivateUseorIsPrivateUseArea character category/block node.
            /// </summary>
            public static readonly CharacterRange IsPrivateUseorIsPrivateUseArea = new CharacterRange ( '\uE000', '\uF8FF' ); // Range(E000-F8FF)

            /// <summary>
            /// The IsCJKCompatibilityIdeographs character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibilityIdeographs = new CharacterRange ( '\uF900', '\uFAFF' ); // Range(F900-FAFF)

            /// <summary>
            /// The IsAlphabeticPresentationForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsAlphabeticPresentationForms = new CharacterRange ( '\uFB00', '\uFB4F' ); // Range(FB00-FB4F)

            /// <summary>
            /// The IsArabicPresentationForms-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabicPresentationFormsA = new CharacterRange ( '\uFB50', '\uFDFF' ); // Range(FB50-FDFF)

            /// <summary>
            /// The IsVariationSelectors character category/block node.
            /// </summary>
            public static readonly CharacterRange IsVariationSelectors = new CharacterRange ( '\uFE00', '\uFE0F' ); // Range(FE00-FE0F)

            /// <summary>
            /// The IsCombiningHalfMarks character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningHalfMarks = new CharacterRange ( '\uFE20', '\uFE2F' ); // Range(FE20-FE2F)

            /// <summary>
            /// The IsCJKCompatibilityForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibilityForms = new CharacterRange ( '\uFE30', '\uFE4F' ); // Range(FE30-FE4F)

            /// <summary>
            /// The IsSmallFormVariants character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSmallFormVariants = new CharacterRange ( '\uFE50', '\uFE6F' ); // Range(FE50-FE6F)

            /// <summary>
            /// The IsArabicPresentationForms-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabicPresentationFormsB = new CharacterRange ( '\uFE70', '\uFEFF' ); // Range(FE70-FEFF)

            /// <summary>
            /// The IsHalfwidthandFullwidthForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHalfwidthandFullwidthForms = new CharacterRange ( '\uFF00', '\uFFEF' ); // Range(FF00-FFEF)

            /// <summary>
            /// The IsSpecials character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSpecials = new CharacterRange ( '\uFFF0', '\uFFFF' ); // Range(FFF0-FFFF)


            #endregion Fields

            #region AllCategories

            /// <summary>
            /// An array containing all category names and their respective nodes. To transform a name into
            /// a node use <see cref="TryParse(String, out GrammarNode{Char}?)" /> or
            /// <see cref="TryParse(ReadOnlySpan{Char}, out GrammarNode{Char}?)" />.
            /// </summary>
            public static readonly ImmutableArray<KeyValuePair<String, GrammarNode<Char>>> AllCategories = ImmutableArray.CreateRange ( new[]
            {
                new KeyValuePair<String, GrammarNode<Char>> ( "Lu", Lu ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Ll", Ll ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Lt", Lt ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Lm", Lm ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Lo", Lo ),
                new KeyValuePair<String, GrammarNode<Char>> ( "L", L ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Mn", Mn ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Mc", Mc ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Me", Me ),
                new KeyValuePair<String, GrammarNode<Char>> ( "M", M ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Nd", Nd ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Nl", Nl ),
                new KeyValuePair<String, GrammarNode<Char>> ( "No", No ),
                new KeyValuePair<String, GrammarNode<Char>> ( "N", N ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Pc", Pc ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Pd", Pd ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Ps", Ps ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Pe", Pe ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Pi", Pi ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Pf", Pf ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Po", Po ),
                new KeyValuePair<String, GrammarNode<Char>> ( "P", P ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Sm", Sm ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Sc", Sc ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Sk", Sk ),
                new KeyValuePair<String, GrammarNode<Char>> ( "So", So ),
                new KeyValuePair<String, GrammarNode<Char>> ( "S", S ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Zs", Zs ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Zl", Zl ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Zp", Zp ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Z", Z ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Cc", Cc ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Cf", Cf ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Cs", Cs ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Co", Co ),
                new KeyValuePair<String, GrammarNode<Char>> ( "Cn", Cn ),
                new KeyValuePair<String, GrammarNode<Char>> ( "C", C ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBasicLatin", IsBasicLatin ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLatin-1Supplement", IsLatin1Supplement ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLatinExtended-A", IsLatinExtendedA ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLatinExtended-B", IsLatinExtendedB ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsIPAExtensions", IsIPAExtensions ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSpacingModifierLetters", IsSpacingModifierLetters ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCombiningDiacriticalMarks", IsCombiningDiacriticalMarks ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGreek", IsGreek ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGreekandCoptic", IsGreekandCoptic ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCyrillic", IsCyrillic ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCyrillicSupplement", IsCyrillicSupplement ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsArmenian", IsArmenian ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHebrew", IsHebrew ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsArabic", IsArabic ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSyriac", IsSyriac ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsThaana", IsThaana ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsDevanagari", IsDevanagari ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBengali", IsBengali ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGurmukhi", IsGurmukhi ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGujarati", IsGujarati ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsOriya", IsOriya ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTamil", IsTamil ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTelugu", IsTelugu ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKannada", IsKannada ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMalayalam", IsMalayalam ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSinhala", IsSinhala ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsThai", IsThai ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLao", IsLao ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTibetan", IsTibetan ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMyanmar", IsMyanmar ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGeorgian", IsGeorgian ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHangulJamo", IsHangulJamo ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsEthiopic", IsEthiopic ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCherokee", IsCherokee ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsUnifiedCanadianAboriginalSyllabics", IsUnifiedCanadianAboriginalSyllabics ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsOgham", IsOgham ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsRunic", IsRunic ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTagalog", IsTagalog ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHanunoo", IsHanunoo ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBuhid", IsBuhid ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTagbanwa", IsTagbanwa ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKhmer", IsKhmer ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMongolian", IsMongolian ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLimbu", IsLimbu ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsTaiLe", IsTaiLe ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKhmerSymbols", IsKhmerSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsPhoneticExtensions", IsPhoneticExtensions ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLatinExtendedAdditional", IsLatinExtendedAdditional ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGreekExtended", IsGreekExtended ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGeneralPunctuation", IsGeneralPunctuation ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSuperscriptsandSubscripts", IsSuperscriptsandSubscripts ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCurrencySymbols", IsCurrencySymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCombiningDiacriticalMarksforSymbols", IsCombiningDiacriticalMarksforSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCombiningMarksforSymbols", IsCombiningMarksforSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLetterlikeSymbols", IsLetterlikeSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsNumberForms", IsNumberForms ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsArrows", IsArrows ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMathematicalOperators", IsMathematicalOperators ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMiscellaneousTechnical", IsMiscellaneousTechnical ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsControlPictures", IsControlPictures ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsOpticalCharacterRecognition", IsOpticalCharacterRecognition ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsEnclosedAlphanumerics", IsEnclosedAlphanumerics ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBoxDrawing", IsBoxDrawing ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBlockElements", IsBlockElements ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsGeometricShapes", IsGeometricShapes ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMiscellaneousSymbols", IsMiscellaneousSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsDingbats", IsDingbats ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMiscellaneousMathematicalSymbols-A", IsMiscellaneousMathematicalSymbolsA ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSupplementalArrows-A", IsSupplementalArrowsA ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBraillePatterns", IsBraillePatterns ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSupplementalArrows-B", IsSupplementalArrowsB ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMiscellaneousMathematicalSymbols-B", IsMiscellaneousMathematicalSymbolsB ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSupplementalMathematicalOperators", IsSupplementalMathematicalOperators ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsMiscellaneousSymbolsandArrows", IsMiscellaneousSymbolsandArrows ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKRadicalsSupplement", IsCJKRadicalsSupplement ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKangxiRadicals", IsKangxiRadicals ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsIdeographicDescriptionCharacters", IsIdeographicDescriptionCharacters ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKSymbolsandPunctuation", IsCJKSymbolsandPunctuation ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHiragana", IsHiragana ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKatakana", IsKatakana ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBopomofo", IsBopomofo ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHangulCompatibilityJamo", IsHangulCompatibilityJamo ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKanbun", IsKanbun ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsBopomofoExtended", IsBopomofoExtended ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsKatakanaPhoneticExtensions", IsKatakanaPhoneticExtensions ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsEnclosedCJKLettersandMonths", IsEnclosedCJKLettersandMonths ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKCompatibility", IsCJKCompatibility ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKUnifiedIdeographsExtensionA", IsCJKUnifiedIdeographsExtensionA ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsYijingHexagramSymbols", IsYijingHexagramSymbols ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKUnifiedIdeographs", IsCJKUnifiedIdeographs ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsYiSyllables", IsYiSyllables ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsYiRadicals", IsYiRadicals ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHangulSyllables", IsHangulSyllables ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHighSurrogates", IsHighSurrogates ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHighPrivateUseSurrogates", IsHighPrivateUseSurrogates ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsLowSurrogates", IsLowSurrogates ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsPrivateUseorIsPrivateUseArea", IsPrivateUseorIsPrivateUseArea ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKCompatibilityIdeographs", IsCJKCompatibilityIdeographs ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsAlphabeticPresentationForms", IsAlphabeticPresentationForms ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsArabicPresentationForms-A", IsArabicPresentationFormsA ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsVariationSelectors", IsVariationSelectors ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCombiningHalfMarks", IsCombiningHalfMarks ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsCJKCompatibilityForms", IsCJKCompatibilityForms ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSmallFormVariants", IsSmallFormVariants ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsArabicPresentationForms-B", IsArabicPresentationFormsB ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsHalfwidthandFullwidthForms", IsHalfwidthandFullwidthForms ),
                new KeyValuePair<String, GrammarNode<Char>> ( "IsSpecials", IsSpecials ),
            } );

            #endregion AllCategories

            /// <summary>
            /// Attempts to parse the regex name of a unicode character category or code block.
            /// </summary>
            /// <paramref name="name">The regex name to be parsed.</paramref>
            /// <paramref name="node">The node that matches the provided regex name.</paramref>
            /// <returns>Whether the name was recognized or not.</returns>
            public static Boolean TryParse ( String name, [NotNullWhen ( true )] out GrammarNode<Char>? node ) => TryParse ( name.AsSpan ( ), out node );

            /// <summary>
            /// Attempts to parse the regex name of a unicode character category or code block.
            /// </summary>
            /// <paramref name="name">The regex name to be parsed.</paramref>
            /// <paramref name="node">The node that matches the provided regex name.</paramref>
            /// <returns>Whether the name was recognized or not.</returns>
            public static Boolean TryParse ( ReadOnlySpan<Char> name, [NotNullWhen ( true )] out GrammarNode<Char>? node )
            {
                if ( name.Length > 0 )
                {
                    switch ( name[0] )
                    {
                        case 'L':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'u':
                                    {
                                        node = Lu;
                                        return true;
                                    }
                                    case 'l':
                                    {
                                        node = Ll;
                                        return true;
                                    }
                                    case 't':
                                    {
                                        node = Lt;
                                        return true;
                                    }
                                    case 'm':
                                    {
                                        node = Lm;
                                        return true;
                                    }
                                    case 'o':
                                    {
                                        node = Lo;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = L;
                                return true;
                            }
                            break;
                        }
                        case 'M':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'n':
                                    {
                                        node = Mn;
                                        return true;
                                    }
                                    case 'c':
                                    {
                                        node = Mc;
                                        return true;
                                    }
                                    case 'e':
                                    {
                                        node = Me;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = M;
                                return true;
                            }
                            break;
                        }
                        case 'N':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'd':
                                    {
                                        node = Nd;
                                        return true;
                                    }
                                    case 'l':
                                    {
                                        node = Nl;
                                        return true;
                                    }
                                    case 'o':
                                    {
                                        node = No;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = N;
                                return true;
                            }
                            break;
                        }
                        case 'P':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'c':
                                    {
                                        node = Pc;
                                        return true;
                                    }
                                    case 'd':
                                    {
                                        node = Pd;
                                        return true;
                                    }
                                    case 's':
                                    {
                                        node = Ps;
                                        return true;
                                    }
                                    case 'e':
                                    {
                                        node = Pe;
                                        return true;
                                    }
                                    case 'i':
                                    {
                                        node = Pi;
                                        return true;
                                    }
                                    case 'f':
                                    {
                                        node = Pf;
                                        return true;
                                    }
                                    case 'o':
                                    {
                                        node = Po;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = P;
                                return true;
                            }
                            break;
                        }
                        case 'S':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'm':
                                    {
                                        node = Sm;
                                        return true;
                                    }
                                    case 'c':
                                    {
                                        node = Sc;
                                        return true;
                                    }
                                    case 'k':
                                    {
                                        node = Sk;
                                        return true;
                                    }
                                    case 'o':
                                    {
                                        node = So;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = S;
                                return true;
                            }
                            break;
                        }
                        case 'Z':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 's':
                                    {
                                        node = Zs;
                                        return true;
                                    }
                                    case 'l':
                                    {
                                        node = Zl;
                                        return true;
                                    }
                                    case 'p':
                                    {
                                        node = Zp;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = Z;
                                return true;
                            }
                            break;
                        }
                        case 'C':
                        {
                            if ( name.Length > 1 )
                            {
                                switch ( name[1] )
                                {
                                    case 'c':
                                    {
                                        node = Cc;
                                        return true;
                                    }
                                    case 'f':
                                    {
                                        node = Cf;
                                        return true;
                                    }
                                    case 's':
                                    {
                                        node = Cs;
                                        return true;
                                    }
                                    case 'o':
                                    {
                                        node = Co;
                                        return true;
                                    }
                                    case 'n':
                                    {
                                        node = Cn;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                node = C;
                                return true;
                            }
                            break;
                        }
                        case 'I':
                        {
                            if ( name.Length > 1 && name[1] == 's' )
                            {
                                if ( name.Length > 2 )
                                {
                                    switch ( name[2] )
                                    {
                                        case 'B':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 11 && name[4] == 's' && name[5] == 'i' && name[6] == 'c' && name[7] == 'L' && name[8] == 'a' && name[9] == 't' && name[10] == 'i' && name[11] == 'n' )
                                                        {
                                                            node = IsBasicLatin;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 8 && name[4] == 'n' && name[5] == 'g' && name[6] == 'a' && name[7] == 'l' && name[8] == 'i' )
                                                        {
                                                            node = IsBengali;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'u':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'h' && name[5] == 'i' && name[6] == 'd' )
                                                        {
                                                            node = IsBuhid;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'o':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'x':
                                                                {
                                                                    if ( name.Length > 11 && name[5] == 'D' && name[6] == 'r' && name[7] == 'a' && name[8] == 'w' && name[9] == 'i' && name[10] == 'n' && name[11] == 'g' )
                                                                    {
                                                                        node = IsBoxDrawing;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'p':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'o' && name[6] == 'm' && name[7] == 'o' && name[8] == 'f' && name[9] == 'o' )
                                                                    {
                                                                        if ( name.Length > 17 && name[10] == 'E' && name[11] == 'x' && name[12] == 't' && name[13] == 'e' && name[14] == 'n' && name[15] == 'd' && name[16] == 'e' && name[17] == 'd' )
                                                                        {
                                                                            node = IsBopomofoExtended;
                                                                            return true;
                                                                        }
                                                                        else
                                                                        {
                                                                            node = IsBopomofo;
                                                                            return true;
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'l':
                                                    {
                                                        if ( name.Length > 14 && name[4] == 'o' && name[5] == 'c' && name[6] == 'k' && name[7] == 'E' && name[8] == 'l' && name[9] == 'e' && name[10] == 'm' && name[11] == 'e' && name[12] == 'n' && name[13] == 't' && name[14] == 's' )
                                                        {
                                                            node = IsBlockElements;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'r':
                                                    {
                                                        if ( name.Length > 16 && name[4] == 'a' && name[5] == 'i' && name[6] == 'l' && name[7] == 'l' && name[8] == 'e' && name[9] == 'P' && name[10] == 'a' && name[11] == 't' && name[12] == 't' && name[13] == 'e' && name[14] == 'r' && name[15] == 'n' && name[16] == 's' )
                                                        {
                                                            node = IsBraillePatterns;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'L':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 't':
                                                                {
                                                                    if ( name.Length > 6 && name[5] == 'i' && name[6] == 'n' )
                                                                    {
                                                                        if ( name.Length > 7 )
                                                                        {
                                                                            switch ( name[7] )
                                                                            {
                                                                                case '-':
                                                                                {
                                                                                    if ( name.Length > 18 && name[8] == '1' && name[9] == 'S' && name[10] == 'u' && name[11] == 'p' && name[12] == 'p' && name[13] == 'l' && name[14] == 'e' && name[15] == 'm' && name[16] == 'e' && name[17] == 'n' && name[18] == 't' )
                                                                                    {
                                                                                        node = IsLatin1Supplement;
                                                                                        return true;
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                case 'E':
                                                                                {
                                                                                    if ( name.Length > 14 && name[8] == 'x' && name[9] == 't' && name[10] == 'e' && name[11] == 'n' && name[12] == 'd' && name[13] == 'e' && name[14] == 'd' )
                                                                                    {
                                                                                        if ( name.Length > 15 )
                                                                                        {
                                                                                            switch ( name[15] )
                                                                                            {
                                                                                                case '-':
                                                                                                {
                                                                                                    if ( name.Length > 16 )
                                                                                                    {
                                                                                                        switch ( name[16] )
                                                                                                        {
                                                                                                            case 'A':
                                                                                                            {
                                                                                                                node = IsLatinExtendedA;
                                                                                                                return true;
                                                                                                            }
                                                                                                            case 'B':
                                                                                                            {
                                                                                                                node = IsLatinExtendedB;
                                                                                                                return true;
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    break;
                                                                                                }
                                                                                                case 'A':
                                                                                                {
                                                                                                    if ( name.Length > 24 && name[16] == 'd' && name[17] == 'd' && name[18] == 'i' && name[19] == 't' && name[20] == 'i' && name[21] == 'o' && name[22] == 'n' && name[23] == 'a' && name[24] == 'l' )
                                                                                                    {
                                                                                                        node = IsLatinExtendedAdditional;
                                                                                                        return true;
                                                                                                    }
                                                                                                    break;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'o':
                                                                {
                                                                    node = IsLao;
                                                                    return true;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'm' && name[5] == 'b' && name[6] == 'u' )
                                                        {
                                                            node = IsLimbu;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 18 && name[4] == 't' && name[5] == 't' && name[6] == 'e' && name[7] == 'r' && name[8] == 'l' && name[9] == 'i' && name[10] == 'k' && name[11] == 'e' && name[12] == 'S' && name[13] == 'y' && name[14] == 'm' && name[15] == 'b' && name[16] == 'o' && name[17] == 'l' && name[18] == 's' )
                                                        {
                                                            node = IsLetterlikeSymbols;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'o':
                                                    {
                                                        if ( name.Length > 14 && name[4] == 'w' && name[5] == 'S' && name[6] == 'u' && name[7] == 'r' && name[8] == 'r' && name[9] == 'o' && name[10] == 'g' && name[11] == 'a' && name[12] == 't' && name[13] == 'e' && name[14] == 's' )
                                                        {
                                                            node = IsLowSurrogates;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'I':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'P':
                                                    {
                                                        if ( name.Length > 14 && name[4] == 'A' && name[5] == 'E' && name[6] == 'x' && name[7] == 't' && name[8] == 'e' && name[9] == 'n' && name[10] == 's' && name[11] == 'i' && name[12] == 'o' && name[13] == 'n' && name[14] == 's' )
                                                        {
                                                            node = IsIPAExtensions;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'd':
                                                    {
                                                        if ( name.Length > 33 && name[4] == 'e' && name[5] == 'o' && name[6] == 'g' && name[7] == 'r' && name[8] == 'a' && name[9] == 'p' && name[10] == 'h' && name[11] == 'i' && name[12] == 'c' && name[13] == 'D' && name[14] == 'e' && name[15] == 's' && name[16] == 'c' && name[17] == 'r' && name[18] == 'i' && name[19] == 'p' && name[20] == 't' && name[21] == 'i' && name[22] == 'o' && name[23] == 'n' && name[24] == 'C' && name[25] == 'h' && name[26] == 'a' && name[27] == 'r' && name[28] == 'a' && name[29] == 'c' && name[30] == 't' && name[31] == 'e' && name[32] == 'r' && name[33] == 's' )
                                                        {
                                                            node = IsIdeographicDescriptionCharacters;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'S':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'p':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'a':
                                                                {
                                                                    if ( name.Length > 23 && name[5] == 'c' && name[6] == 'i' && name[7] == 'n' && name[8] == 'g' && name[9] == 'M' && name[10] == 'o' && name[11] == 'd' && name[12] == 'i' && name[13] == 'f' && name[14] == 'i' && name[15] == 'e' && name[16] == 'r' && name[17] == 'L' && name[18] == 'e' && name[19] == 't' && name[20] == 't' && name[21] == 'e' && name[22] == 'r' && name[23] == 's' )
                                                                    {
                                                                        node = IsSpacingModifierLetters;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'e':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'c' && name[6] == 'i' && name[7] == 'a' && name[8] == 'l' && name[9] == 's' )
                                                                    {
                                                                        node = IsSpecials;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'y':
                                                    {
                                                        if ( name.Length > 7 && name[4] == 'r' && name[5] == 'i' && name[6] == 'a' && name[7] == 'c' )
                                                        {
                                                            node = IsSyriac;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 8 && name[4] == 'n' && name[5] == 'h' && name[6] == 'a' && name[7] == 'l' && name[8] == 'a' )
                                                        {
                                                            node = IsSinhala;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'u':
                                                    {
                                                        if ( name.Length > 4 && name[4] == 'p' )
                                                        {
                                                            if ( name.Length > 5 )
                                                            {
                                                                switch ( name[5] )
                                                                {
                                                                    case 'e':
                                                                    {
                                                                        if ( name.Length > 26 && name[6] == 'r' && name[7] == 's' && name[8] == 'c' && name[9] == 'r' && name[10] == 'i' && name[11] == 'p' && name[12] == 't' && name[13] == 's' && name[14] == 'a' && name[15] == 'n' && name[16] == 'd' && name[17] == 'S' && name[18] == 'u' && name[19] == 'b' && name[20] == 's' && name[21] == 'c' && name[22] == 'r' && name[23] == 'i' && name[24] == 'p' && name[25] == 't' && name[26] == 's' )
                                                                        {
                                                                            node = IsSuperscriptsandSubscripts;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'p':
                                                                    {
                                                                        if ( name.Length > 13 && name[6] == 'l' && name[7] == 'e' && name[8] == 'm' && name[9] == 'e' && name[10] == 'n' && name[11] == 't' && name[12] == 'a' && name[13] == 'l' )
                                                                        {
                                                                            if ( name.Length > 14 )
                                                                            {
                                                                                switch ( name[14] )
                                                                                {
                                                                                    case 'A':
                                                                                    {
                                                                                        if ( name.Length > 20 && name[15] == 'r' && name[16] == 'r' && name[17] == 'o' && name[18] == 'w' && name[19] == 's' && name[20] == '-' )
                                                                                        {
                                                                                            if ( name.Length > 21 )
                                                                                            {
                                                                                                switch ( name[21] )
                                                                                                {
                                                                                                    case 'A':
                                                                                                    {
                                                                                                        node = IsSupplementalArrowsA;
                                                                                                        return true;
                                                                                                    }
                                                                                                    case 'B':
                                                                                                    {
                                                                                                        node = IsSupplementalArrowsB;
                                                                                                        return true;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    case 'M':
                                                                                    {
                                                                                        if ( name.Length > 34 && name[15] == 'a' && name[16] == 't' && name[17] == 'h' && name[18] == 'e' && name[19] == 'm' && name[20] == 'a' && name[21] == 't' && name[22] == 'i' && name[23] == 'c' && name[24] == 'a' && name[25] == 'l' && name[26] == 'O' && name[27] == 'p' && name[28] == 'e' && name[29] == 'r' && name[30] == 'a' && name[31] == 't' && name[32] == 'o' && name[33] == 'r' && name[34] == 's' )
                                                                                        {
                                                                                            node = IsSupplementalMathematicalOperators;
                                                                                            return true;
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'm':
                                                    {
                                                        if ( name.Length > 18 && name[4] == 'a' && name[5] == 'l' && name[6] == 'l' && name[7] == 'F' && name[8] == 'o' && name[9] == 'r' && name[10] == 'm' && name[11] == 'V' && name[12] == 'a' && name[13] == 'r' && name[14] == 'i' && name[15] == 'a' && name[16] == 'n' && name[17] == 't' && name[18] == 's' )
                                                        {
                                                            node = IsSmallFormVariants;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'C':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'o':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'm':
                                                                {
                                                                    if ( name.Length > 10 && name[5] == 'b' && name[6] == 'i' && name[7] == 'n' && name[8] == 'i' && name[9] == 'n' && name[10] == 'g' )
                                                                    {
                                                                        if ( name.Length > 11 )
                                                                        {
                                                                            switch ( name[11] )
                                                                            {
                                                                                case 'D':
                                                                                {
                                                                                    if ( name.Length > 26 && name[12] == 'i' && name[13] == 'a' && name[14] == 'c' && name[15] == 'r' && name[16] == 'i' && name[17] == 't' && name[18] == 'i' && name[19] == 'c' && name[20] == 'a' && name[21] == 'l' && name[22] == 'M' && name[23] == 'a' && name[24] == 'r' && name[25] == 'k' && name[26] == 's' )
                                                                                    {
                                                                                        if ( name.Length > 36 && name[27] == 'f' && name[28] == 'o' && name[29] == 'r' && name[30] == 'S' && name[31] == 'y' && name[32] == 'm' && name[33] == 'b' && name[34] == 'o' && name[35] == 'l' && name[36] == 's' )
                                                                                        {
                                                                                            node = IsCombiningDiacriticalMarksforSymbols;
                                                                                            return true;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            node = IsCombiningDiacriticalMarks;
                                                                                            return true;
                                                                                        }
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                case 'M':
                                                                                {
                                                                                    if ( name.Length > 25 && name[12] == 'a' && name[13] == 'r' && name[14] == 'k' && name[15] == 's' && name[16] == 'f' && name[17] == 'o' && name[18] == 'r' && name[19] == 'S' && name[20] == 'y' && name[21] == 'm' && name[22] == 'b' && name[23] == 'o' && name[24] == 'l' && name[25] == 's' )
                                                                                    {
                                                                                        node = IsCombiningMarksforSymbols;
                                                                                        return true;
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                case 'H':
                                                                                {
                                                                                    if ( name.Length > 19 && name[12] == 'a' && name[13] == 'l' && name[14] == 'f' && name[15] == 'M' && name[16] == 'a' && name[17] == 'r' && name[18] == 'k' && name[19] == 's' )
                                                                                    {
                                                                                        node = IsCombiningHalfMarks;
                                                                                        return true;
                                                                                    }
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'n':
                                                                {
                                                                    if ( name.Length > 16 && name[5] == 't' && name[6] == 'r' && name[7] == 'o' && name[8] == 'l' && name[9] == 'P' && name[10] == 'i' && name[11] == 'c' && name[12] == 't' && name[13] == 'u' && name[14] == 'r' && name[15] == 'e' && name[16] == 's' )
                                                                    {
                                                                        node = IsControlPictures;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'y':
                                                    {
                                                        if ( name.Length > 9 && name[4] == 'r' && name[5] == 'i' && name[6] == 'l' && name[7] == 'l' && name[8] == 'i' && name[9] == 'c' )
                                                        {
                                                            if ( name.Length > 19 && name[10] == 'S' && name[11] == 'u' && name[12] == 'p' && name[13] == 'p' && name[14] == 'l' && name[15] == 'e' && name[16] == 'm' && name[17] == 'e' && name[18] == 'n' && name[19] == 't' )
                                                            {
                                                                node = IsCyrillicSupplement;
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                node = IsCyrillic;
                                                                return true;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'h':
                                                    {
                                                        if ( name.Length > 9 && name[4] == 'e' && name[5] == 'r' && name[6] == 'o' && name[7] == 'k' && name[8] == 'e' && name[9] == 'e' )
                                                        {
                                                            node = IsCherokee;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'u':
                                                    {
                                                        if ( name.Length > 16 && name[4] == 'r' && name[5] == 'r' && name[6] == 'e' && name[7] == 'n' && name[8] == 'c' && name[9] == 'y' && name[10] == 'S' && name[11] == 'y' && name[12] == 'm' && name[13] == 'b' && name[14] == 'o' && name[15] == 'l' && name[16] == 's' )
                                                        {
                                                            node = IsCurrencySymbols;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'J':
                                                    {
                                                        if ( name.Length > 4 && name[4] == 'K' )
                                                        {
                                                            if ( name.Length > 5 )
                                                            {
                                                                switch ( name[5] )
                                                                {
                                                                    case 'R':
                                                                    {
                                                                        if ( name.Length > 22 && name[6] == 'a' && name[7] == 'd' && name[8] == 'i' && name[9] == 'c' && name[10] == 'a' && name[11] == 'l' && name[12] == 's' && name[13] == 'S' && name[14] == 'u' && name[15] == 'p' && name[16] == 'p' && name[17] == 'l' && name[18] == 'e' && name[19] == 'm' && name[20] == 'e' && name[21] == 'n' && name[22] == 't' )
                                                                        {
                                                                            node = IsCJKRadicalsSupplement;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'S':
                                                                    {
                                                                        if ( name.Length > 25 && name[6] == 'y' && name[7] == 'm' && name[8] == 'b' && name[9] == 'o' && name[10] == 'l' && name[11] == 's' && name[12] == 'a' && name[13] == 'n' && name[14] == 'd' && name[15] == 'P' && name[16] == 'u' && name[17] == 'n' && name[18] == 'c' && name[19] == 't' && name[20] == 'u' && name[21] == 'a' && name[22] == 't' && name[23] == 'i' && name[24] == 'o' && name[25] == 'n' )
                                                                        {
                                                                            node = IsCJKSymbolsandPunctuation;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'C':
                                                                    {
                                                                        if ( name.Length > 17 && name[6] == 'o' && name[7] == 'm' && name[8] == 'p' && name[9] == 'a' && name[10] == 't' && name[11] == 'i' && name[12] == 'b' && name[13] == 'i' && name[14] == 'l' && name[15] == 'i' && name[16] == 't' && name[17] == 'y' )
                                                                        {
                                                                            if ( name.Length > 18 )
                                                                            {
                                                                                switch ( name[18] )
                                                                                {
                                                                                    case 'I':
                                                                                    {
                                                                                        if ( name.Length > 27 && name[19] == 'd' && name[20] == 'e' && name[21] == 'o' && name[22] == 'g' && name[23] == 'r' && name[24] == 'a' && name[25] == 'p' && name[26] == 'h' && name[27] == 's' )
                                                                                        {
                                                                                            node = IsCJKCompatibilityIdeographs;
                                                                                            return true;
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    case 'F':
                                                                                    {
                                                                                        if ( name.Length > 22 && name[19] == 'o' && name[20] == 'r' && name[21] == 'm' && name[22] == 's' )
                                                                                        {
                                                                                            node = IsCJKCompatibilityForms;
                                                                                            return true;
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                node = IsCJKCompatibility;
                                                                                return true;
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'U':
                                                                    {
                                                                        if ( name.Length > 21 && name[6] == 'n' && name[7] == 'i' && name[8] == 'f' && name[9] == 'i' && name[10] == 'e' && name[11] == 'd' && name[12] == 'I' && name[13] == 'd' && name[14] == 'e' && name[15] == 'o' && name[16] == 'g' && name[17] == 'r' && name[18] == 'a' && name[19] == 'p' && name[20] == 'h' && name[21] == 's' )
                                                                        {
                                                                            if ( name.Length > 31 && name[22] == 'E' && name[23] == 'x' && name[24] == 't' && name[25] == 'e' && name[26] == 'n' && name[27] == 's' && name[28] == 'i' && name[29] == 'o' && name[30] == 'n' && name[31] == 'A' )
                                                                            {
                                                                                node = IsCJKUnifiedIdeographsExtensionA;
                                                                                return true;
                                                                            }
                                                                            else
                                                                            {
                                                                                node = IsCJKUnifiedIdeographs;
                                                                                return true;
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'G':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'r':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'e' && name[5] == 'e' && name[6] == 'k' )
                                                        {
                                                            if ( name.Length > 7 )
                                                            {
                                                                switch ( name[7] )
                                                                {
                                                                    case 'a':
                                                                    {
                                                                        if ( name.Length > 15 && name[8] == 'n' && name[9] == 'd' && name[10] == 'C' && name[11] == 'o' && name[12] == 'p' && name[13] == 't' && name[14] == 'i' && name[15] == 'c' )
                                                                        {
                                                                            node = IsGreekandCoptic;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'E':
                                                                    {
                                                                        if ( name.Length > 14 && name[8] == 'x' && name[9] == 't' && name[10] == 'e' && name[11] == 'n' && name[12] == 'd' && name[13] == 'e' && name[14] == 'd' )
                                                                        {
                                                                            node = IsGreekExtended;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                node = IsGreek;
                                                                return true;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'u':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'r':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'm' && name[6] == 'u' && name[7] == 'k' && name[8] == 'h' && name[9] == 'i' )
                                                                    {
                                                                        node = IsGurmukhi;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'j':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'a' && name[6] == 'r' && name[7] == 'a' && name[8] == 't' && name[9] == 'i' )
                                                                    {
                                                                        node = IsGujarati;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'o':
                                                                {
                                                                    if ( name.Length > 5 )
                                                                    {
                                                                        switch ( name[5] )
                                                                        {
                                                                            case 'r':
                                                                            {
                                                                                if ( name.Length > 9 && name[6] == 'g' && name[7] == 'i' && name[8] == 'a' && name[9] == 'n' )
                                                                                {
                                                                                    node = IsGeorgian;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                            case 'm':
                                                                            {
                                                                                if ( name.Length > 16 && name[6] == 'e' && name[7] == 't' && name[8] == 'r' && name[9] == 'i' && name[10] == 'c' && name[11] == 'S' && name[12] == 'h' && name[13] == 'a' && name[14] == 'p' && name[15] == 'e' && name[16] == 's' )
                                                                                {
                                                                                    node = IsGeometricShapes;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'n':
                                                                {
                                                                    if ( name.Length > 19 && name[5] == 'e' && name[6] == 'r' && name[7] == 'a' && name[8] == 'l' && name[9] == 'P' && name[10] == 'u' && name[11] == 'n' && name[12] == 'c' && name[13] == 't' && name[14] == 'u' && name[15] == 'a' && name[16] == 't' && name[17] == 'i' && name[18] == 'o' && name[19] == 'n' )
                                                                    {
                                                                        node = IsGeneralPunctuation;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'A':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'r':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'm':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'e' && name[6] == 'n' && name[7] == 'i' && name[8] == 'a' && name[9] == 'n' )
                                                                    {
                                                                        node = IsArmenian;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'a':
                                                                {
                                                                    if ( name.Length > 7 && name[5] == 'b' && name[6] == 'i' && name[7] == 'c' )
                                                                    {
                                                                        if ( name.Length > 25 && name[8] == 'P' && name[9] == 'r' && name[10] == 'e' && name[11] == 's' && name[12] == 'e' && name[13] == 'n' && name[14] == 't' && name[15] == 'a' && name[16] == 't' && name[17] == 'i' && name[18] == 'o' && name[19] == 'n' && name[20] == 'F' && name[21] == 'o' && name[22] == 'r' && name[23] == 'm' && name[24] == 's' && name[25] == '-' )
                                                                        {
                                                                            if ( name.Length > 26 )
                                                                            {
                                                                                switch ( name[26] )
                                                                                {
                                                                                    case 'A':
                                                                                    {
                                                                                        node = IsArabicPresentationFormsA;
                                                                                        return true;
                                                                                    }
                                                                                    case 'B':
                                                                                    {
                                                                                        node = IsArabicPresentationFormsB;
                                                                                        return true;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            node = IsArabic;
                                                                            return true;
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'r':
                                                                {
                                                                    if ( name.Length > 7 && name[5] == 'o' && name[6] == 'w' && name[7] == 's' )
                                                                    {
                                                                        node = IsArrows;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'l':
                                                    {
                                                        if ( name.Length > 28 && name[4] == 'p' && name[5] == 'h' && name[6] == 'a' && name[7] == 'b' && name[8] == 'e' && name[9] == 't' && name[10] == 'i' && name[11] == 'c' && name[12] == 'P' && name[13] == 'r' && name[14] == 'e' && name[15] == 's' && name[16] == 'e' && name[17] == 'n' && name[18] == 't' && name[19] == 'a' && name[20] == 't' && name[21] == 'i' && name[22] == 'o' && name[23] == 'n' && name[24] == 'F' && name[25] == 'o' && name[26] == 'r' && name[27] == 'm' && name[28] == 's' )
                                                        {
                                                            node = IsAlphabeticPresentationForms;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'H':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 7 && name[4] == 'b' && name[5] == 'r' && name[6] == 'e' && name[7] == 'w' )
                                                        {
                                                            node = IsHebrew;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'n':
                                                                {
                                                                    if ( name.Length > 5 )
                                                                    {
                                                                        switch ( name[5] )
                                                                        {
                                                                            case 'g':
                                                                            {
                                                                                if ( name.Length > 7 && name[6] == 'u' && name[7] == 'l' )
                                                                                {
                                                                                    if ( name.Length > 8 )
                                                                                    {
                                                                                        switch ( name[8] )
                                                                                        {
                                                                                            case 'J':
                                                                                            {
                                                                                                if ( name.Length > 11 && name[9] == 'a' && name[10] == 'm' && name[11] == 'o' )
                                                                                                {
                                                                                                    node = IsHangulJamo;
                                                                                                    return true;
                                                                                                }
                                                                                                break;
                                                                                            }
                                                                                            case 'C':
                                                                                            {
                                                                                                if ( name.Length > 24 && name[9] == 'o' && name[10] == 'm' && name[11] == 'p' && name[12] == 'a' && name[13] == 't' && name[14] == 'i' && name[15] == 'b' && name[16] == 'i' && name[17] == 'l' && name[18] == 'i' && name[19] == 't' && name[20] == 'y' && name[21] == 'J' && name[22] == 'a' && name[23] == 'm' && name[24] == 'o' )
                                                                                                {
                                                                                                    node = IsHangulCompatibilityJamo;
                                                                                                    return true;
                                                                                                }
                                                                                                break;
                                                                                            }
                                                                                            case 'S':
                                                                                            {
                                                                                                if ( name.Length > 16 && name[9] == 'y' && name[10] == 'l' && name[11] == 'l' && name[12] == 'a' && name[13] == 'b' && name[14] == 'l' && name[15] == 'e' && name[16] == 's' )
                                                                                                {
                                                                                                    node = IsHangulSyllables;
                                                                                                    return true;
                                                                                                }
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                break;
                                                                            }
                                                                            case 'u':
                                                                            {
                                                                                if ( name.Length > 8 && name[6] == 'n' && name[7] == 'o' && name[8] == 'o' )
                                                                                {
                                                                                    node = IsHanunoo;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'l':
                                                                {
                                                                    if ( name.Length > 27 && name[5] == 'f' && name[6] == 'w' && name[7] == 'i' && name[8] == 'd' && name[9] == 't' && name[10] == 'h' && name[11] == 'a' && name[12] == 'n' && name[13] == 'd' && name[14] == 'F' && name[15] == 'u' && name[16] == 'l' && name[17] == 'l' && name[18] == 'w' && name[19] == 'i' && name[20] == 'd' && name[21] == 't' && name[22] == 'h' && name[23] == 'F' && name[24] == 'o' && name[25] == 'r' && name[26] == 'm' && name[27] == 's' )
                                                                    {
                                                                        node = IsHalfwidthandFullwidthForms;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'r':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'a' && name[6] == 'g' && name[7] == 'a' && name[8] == 'n' && name[9] == 'a' )
                                                                    {
                                                                        node = IsHiragana;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'g':
                                                                {
                                                                    if ( name.Length > 5 && name[5] == 'h' )
                                                                    {
                                                                        if ( name.Length > 6 )
                                                                        {
                                                                            switch ( name[6] )
                                                                            {
                                                                                case 'S':
                                                                                {
                                                                                    if ( name.Length > 15 && name[7] == 'u' && name[8] == 'r' && name[9] == 'r' && name[10] == 'o' && name[11] == 'g' && name[12] == 'a' && name[13] == 't' && name[14] == 'e' && name[15] == 's' )
                                                                                    {
                                                                                        node = IsHighSurrogates;
                                                                                        return true;
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                case 'P':
                                                                                {
                                                                                    if ( name.Length > 25 && name[7] == 'r' && name[8] == 'i' && name[9] == 'v' && name[10] == 'a' && name[11] == 't' && name[12] == 'e' && name[13] == 'U' && name[14] == 's' && name[15] == 'e' && name[16] == 'S' && name[17] == 'u' && name[18] == 'r' && name[19] == 'r' && name[20] == 'o' && name[21] == 'g' && name[22] == 'a' && name[23] == 't' && name[24] == 'e' && name[25] == 's' )
                                                                                    {
                                                                                        node = IsHighPrivateUseSurrogates;
                                                                                        return true;
                                                                                    }
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'T':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'h':
                                                    {
                                                        if ( name.Length > 4 && name[4] == 'a' )
                                                        {
                                                            if ( name.Length > 5 )
                                                            {
                                                                switch ( name[5] )
                                                                {
                                                                    case 'a':
                                                                    {
                                                                        if ( name.Length > 7 && name[6] == 'n' && name[7] == 'a' )
                                                                        {
                                                                            node = IsThaana;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'i':
                                                                    {
                                                                        node = IsThai;
                                                                        return true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'm':
                                                                {
                                                                    if ( name.Length > 6 && name[5] == 'i' && name[6] == 'l' )
                                                                    {
                                                                        node = IsTamil;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 'g':
                                                                {
                                                                    if ( name.Length > 5 )
                                                                    {
                                                                        switch ( name[5] )
                                                                        {
                                                                            case 'a':
                                                                            {
                                                                                if ( name.Length > 8 && name[6] == 'l' && name[7] == 'o' && name[8] == 'g' )
                                                                                {
                                                                                    node = IsTagalog;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                            case 'b':
                                                                            {
                                                                                if ( name.Length > 9 && name[6] == 'a' && name[7] == 'n' && name[8] == 'w' && name[9] == 'a' )
                                                                                {
                                                                                    node = IsTagbanwa;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 'i':
                                                                {
                                                                    if ( name.Length > 6 && name[5] == 'L' && name[6] == 'e' )
                                                                    {
                                                                        node = IsTaiLe;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 7 && name[4] == 'l' && name[5] == 'u' && name[6] == 'g' && name[7] == 'u' )
                                                        {
                                                            node = IsTelugu;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 8 && name[4] == 'b' && name[5] == 'e' && name[6] == 't' && name[7] == 'a' && name[8] == 'n' )
                                                        {
                                                            node = IsTibetan;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'D':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'e':
                                                    {
                                                        if ( name.Length > 11 && name[4] == 'v' && name[5] == 'a' && name[6] == 'n' && name[7] == 'a' && name[8] == 'g' && name[9] == 'a' && name[10] == 'r' && name[11] == 'i' )
                                                        {
                                                            node = IsDevanagari;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 9 && name[4] == 'n' && name[5] == 'g' && name[6] == 'b' && name[7] == 'a' && name[8] == 't' && name[9] == 's' )
                                                        {
                                                            node = IsDingbats;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'O':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'r':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'i' && name[5] == 'y' && name[6] == 'a' )
                                                        {
                                                            node = IsOriya;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'g':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'h' && name[5] == 'a' && name[6] == 'm' )
                                                        {
                                                            node = IsOgham;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'p':
                                                    {
                                                        if ( name.Length > 28 && name[4] == 't' && name[5] == 'i' && name[6] == 'c' && name[7] == 'a' && name[8] == 'l' && name[9] == 'C' && name[10] == 'h' && name[11] == 'a' && name[12] == 'r' && name[13] == 'a' && name[14] == 'c' && name[15] == 't' && name[16] == 'e' && name[17] == 'r' && name[18] == 'R' && name[19] == 'e' && name[20] == 'c' && name[21] == 'o' && name[22] == 'g' && name[23] == 'n' && name[24] == 'i' && name[25] == 't' && name[26] == 'i' && name[27] == 'o' && name[28] == 'n' )
                                                        {
                                                            node = IsOpticalCharacterRecognition;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'K':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'n':
                                                                {
                                                                    if ( name.Length > 5 )
                                                                    {
                                                                        switch ( name[5] )
                                                                        {
                                                                            case 'n':
                                                                            {
                                                                                if ( name.Length > 8 && name[6] == 'a' && name[7] == 'd' && name[8] == 'a' )
                                                                                {
                                                                                    node = IsKannada;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                            case 'g':
                                                                            {
                                                                                if ( name.Length > 15 && name[6] == 'x' && name[7] == 'i' && name[8] == 'R' && name[9] == 'a' && name[10] == 'd' && name[11] == 'i' && name[12] == 'c' && name[13] == 'a' && name[14] == 'l' && name[15] == 's' )
                                                                                {
                                                                                    node = IsKangxiRadicals;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                            case 'b':
                                                                            {
                                                                                if ( name.Length > 7 && name[6] == 'u' && name[7] == 'n' )
                                                                                {
                                                                                    node = IsKanbun;
                                                                                    return true;
                                                                                }
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                                case 't':
                                                                {
                                                                    if ( name.Length > 9 && name[5] == 'a' && name[6] == 'k' && name[7] == 'a' && name[8] == 'n' && name[9] == 'a' )
                                                                    {
                                                                        if ( name.Length > 27 && name[10] == 'P' && name[11] == 'h' && name[12] == 'o' && name[13] == 'n' && name[14] == 'e' && name[15] == 't' && name[16] == 'i' && name[17] == 'c' && name[18] == 'E' && name[19] == 'x' && name[20] == 't' && name[21] == 'e' && name[22] == 'n' && name[23] == 's' && name[24] == 'i' && name[25] == 'o' && name[26] == 'n' && name[27] == 's' )
                                                                        {
                                                                            node = IsKatakanaPhoneticExtensions;
                                                                            return true;
                                                                        }
                                                                        else
                                                                        {
                                                                            node = IsKatakana;
                                                                            return true;
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'h':
                                                    {
                                                        if ( name.Length > 6 && name[4] == 'm' && name[5] == 'e' && name[6] == 'r' )
                                                        {
                                                            if ( name.Length > 13 && name[7] == 'S' && name[8] == 'y' && name[9] == 'm' && name[10] == 'b' && name[11] == 'o' && name[12] == 'l' && name[13] == 's' )
                                                            {
                                                                node = IsKhmerSymbols;
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                node = IsKhmer;
                                                                return true;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'M':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'a':
                                                    {
                                                        if ( name.Length > 4 )
                                                        {
                                                            switch ( name[4] )
                                                            {
                                                                case 'l':
                                                                {
                                                                    if ( name.Length > 10 && name[5] == 'a' && name[6] == 'y' && name[7] == 'a' && name[8] == 'l' && name[9] == 'a' && name[10] == 'm' )
                                                                    {
                                                                        node = IsMalayalam;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                                case 't':
                                                                {
                                                                    if ( name.Length > 22 && name[5] == 'h' && name[6] == 'e' && name[7] == 'm' && name[8] == 'a' && name[9] == 't' && name[10] == 'i' && name[11] == 'c' && name[12] == 'a' && name[13] == 'l' && name[14] == 'O' && name[15] == 'p' && name[16] == 'e' && name[17] == 'r' && name[18] == 'a' && name[19] == 't' && name[20] == 'o' && name[21] == 'r' && name[22] == 's' )
                                                                    {
                                                                        node = IsMathematicalOperators;
                                                                        return true;
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 'y':
                                                    {
                                                        if ( name.Length > 8 && name[4] == 'a' && name[5] == 'n' && name[6] == 'm' && name[7] == 'a' && name[8] == 'r' )
                                                        {
                                                            node = IsMyanmar;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'o':
                                                    {
                                                        if ( name.Length > 10 && name[4] == 'n' && name[5] == 'g' && name[6] == 'o' && name[7] == 'l' && name[8] == 'i' && name[9] == 'a' && name[10] == 'n' )
                                                        {
                                                            node = IsMongolian;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'i':
                                                    {
                                                        if ( name.Length > 14 && name[4] == 's' && name[5] == 'c' && name[6] == 'e' && name[7] == 'l' && name[8] == 'l' && name[9] == 'a' && name[10] == 'n' && name[11] == 'e' && name[12] == 'o' && name[13] == 'u' && name[14] == 's' )
                                                        {
                                                            if ( name.Length > 15 )
                                                            {
                                                                switch ( name[15] )
                                                                {
                                                                    case 'T':
                                                                    {
                                                                        if ( name.Length > 23 && name[16] == 'e' && name[17] == 'c' && name[18] == 'h' && name[19] == 'n' && name[20] == 'i' && name[21] == 'c' && name[22] == 'a' && name[23] == 'l' )
                                                                        {
                                                                            node = IsMiscellaneousTechnical;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'S':
                                                                    {
                                                                        if ( name.Length > 21 && name[16] == 'y' && name[17] == 'm' && name[18] == 'b' && name[19] == 'o' && name[20] == 'l' && name[21] == 's' )
                                                                        {
                                                                            if ( name.Length > 30 && name[22] == 'a' && name[23] == 'n' && name[24] == 'd' && name[25] == 'A' && name[26] == 'r' && name[27] == 'r' && name[28] == 'o' && name[29] == 'w' && name[30] == 's' )
                                                                            {
                                                                                node = IsMiscellaneousSymbolsandArrows;
                                                                                return true;
                                                                            }
                                                                            else
                                                                            {
                                                                                node = IsMiscellaneousSymbols;
                                                                                return true;
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'M':
                                                                    {
                                                                        if ( name.Length > 34 && name[16] == 'a' && name[17] == 't' && name[18] == 'h' && name[19] == 'e' && name[20] == 'm' && name[21] == 'a' && name[22] == 't' && name[23] == 'i' && name[24] == 'c' && name[25] == 'a' && name[26] == 'l' && name[27] == 'S' && name[28] == 'y' && name[29] == 'm' && name[30] == 'b' && name[31] == 'o' && name[32] == 'l' && name[33] == 's' && name[34] == '-' )
                                                                        {
                                                                            if ( name.Length > 35 )
                                                                            {
                                                                                switch ( name[35] )
                                                                                {
                                                                                    case 'A':
                                                                                    {
                                                                                        node = IsMiscellaneousMathematicalSymbolsA;
                                                                                        return true;
                                                                                    }
                                                                                    case 'B':
                                                                                    {
                                                                                        node = IsMiscellaneousMathematicalSymbolsB;
                                                                                        return true;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'E':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 't':
                                                    {
                                                        if ( name.Length > 9 && name[4] == 'h' && name[5] == 'i' && name[6] == 'o' && name[7] == 'p' && name[8] == 'i' && name[9] == 'c' )
                                                        {
                                                            node = IsEthiopic;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'n':
                                                    {
                                                        if ( name.Length > 9 && name[4] == 'c' && name[5] == 'l' && name[6] == 'o' && name[7] == 's' && name[8] == 'e' && name[9] == 'd' )
                                                        {
                                                            if ( name.Length > 10 )
                                                            {
                                                                switch ( name[10] )
                                                                {
                                                                    case 'A':
                                                                    {
                                                                        if ( name.Length > 22 && name[11] == 'l' && name[12] == 'p' && name[13] == 'h' && name[14] == 'a' && name[15] == 'n' && name[16] == 'u' && name[17] == 'm' && name[18] == 'e' && name[19] == 'r' && name[20] == 'i' && name[21] == 'c' && name[22] == 's' )
                                                                        {
                                                                            node = IsEnclosedAlphanumerics;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                    case 'C':
                                                                    {
                                                                        if ( name.Length > 28 && name[11] == 'J' && name[12] == 'K' && name[13] == 'L' && name[14] == 'e' && name[15] == 't' && name[16] == 't' && name[17] == 'e' && name[18] == 'r' && name[19] == 's' && name[20] == 'a' && name[21] == 'n' && name[22] == 'd' && name[23] == 'M' && name[24] == 'o' && name[25] == 'n' && name[26] == 't' && name[27] == 'h' && name[28] == 's' )
                                                                        {
                                                                            node = IsEnclosedCJKLettersandMonths;
                                                                            return true;
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'U':
                                        {
                                            if ( name.Length > 35 && name[3] == 'n' && name[4] == 'i' && name[5] == 'f' && name[6] == 'i' && name[7] == 'e' && name[8] == 'd' && name[9] == 'C' && name[10] == 'a' && name[11] == 'n' && name[12] == 'a' && name[13] == 'd' && name[14] == 'i' && name[15] == 'a' && name[16] == 'n' && name[17] == 'A' && name[18] == 'b' && name[19] == 'o' && name[20] == 'r' && name[21] == 'i' && name[22] == 'g' && name[23] == 'i' && name[24] == 'n' && name[25] == 'a' && name[26] == 'l' && name[27] == 'S' && name[28] == 'y' && name[29] == 'l' && name[30] == 'l' && name[31] == 'a' && name[32] == 'b' && name[33] == 'i' && name[34] == 'c' && name[35] == 's' )
                                            {
                                                node = IsUnifiedCanadianAboriginalSyllabics;
                                                return true;
                                            }
                                            break;
                                        }
                                        case 'R':
                                        {
                                            if ( name.Length > 6 && name[3] == 'u' && name[4] == 'n' && name[5] == 'i' && name[6] == 'c' )
                                            {
                                                node = IsRunic;
                                                return true;
                                            }
                                            break;
                                        }
                                        case 'P':
                                        {
                                            if ( name.Length > 3 )
                                            {
                                                switch ( name[3] )
                                                {
                                                    case 'h':
                                                    {
                                                        if ( name.Length > 19 && name[4] == 'o' && name[5] == 'n' && name[6] == 'e' && name[7] == 't' && name[8] == 'i' && name[9] == 'c' && name[10] == 'E' && name[11] == 'x' && name[12] == 't' && name[13] == 'e' && name[14] == 'n' && name[15] == 's' && name[16] == 'i' && name[17] == 'o' && name[18] == 'n' && name[19] == 's' )
                                                        {
                                                            node = IsPhoneticExtensions;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                    case 'r':
                                                    {
                                                        if ( name.Length > 29 && name[4] == 'i' && name[5] == 'v' && name[6] == 'a' && name[7] == 't' && name[8] == 'e' && name[9] == 'U' && name[10] == 's' && name[11] == 'e' && name[12] == 'o' && name[13] == 'r' && name[14] == 'I' && name[15] == 's' && name[16] == 'P' && name[17] == 'r' && name[18] == 'i' && name[19] == 'v' && name[20] == 'a' && name[21] == 't' && name[22] == 'e' && name[23] == 'U' && name[24] == 's' && name[25] == 'e' && name[26] == 'A' && name[27] == 'r' && name[28] == 'e' && name[29] == 'a' )
                                                        {
                                                            node = IsPrivateUseorIsPrivateUseArea;
                                                            return true;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'N':
                                        {
                                            if ( name.Length > 12 && name[3] == 'u' && name[4] == 'm' && name[5] == 'b' && name[6] == 'e' && name[7] == 'r' && name[8] == 'F' && name[9] == 'o' && name[10] == 'r' && name[11] == 'm' && name[12] == 's' )
                                            {
                                                node = IsNumberForms;
                                                return true;
                                            }
                                            break;
                                        }
                                        case 'Y':
                                        {
                                            if ( name.Length > 3 && name[3] == 'i' )
                                            {
                                                if ( name.Length > 4 )
                                                {
                                                    switch ( name[4] )
                                                    {
                                                        case 'j':
                                                        {
                                                            if ( name.Length > 22 && name[5] == 'i' && name[6] == 'n' && name[7] == 'g' && name[8] == 'H' && name[9] == 'e' && name[10] == 'x' && name[11] == 'a' && name[12] == 'g' && name[13] == 'r' && name[14] == 'a' && name[15] == 'm' && name[16] == 'S' && name[17] == 'y' && name[18] == 'm' && name[19] == 'b' && name[20] == 'o' && name[21] == 'l' && name[22] == 's' )
                                                            {
                                                                node = IsYijingHexagramSymbols;
                                                                return true;
                                                            }
                                                            break;
                                                        }
                                                        case 'S':
                                                        {
                                                            if ( name.Length > 12 && name[5] == 'y' && name[6] == 'l' && name[7] == 'l' && name[8] == 'a' && name[9] == 'b' && name[10] == 'l' && name[11] == 'e' && name[12] == 's' )
                                                            {
                                                                node = IsYiSyllables;
                                                                return true;
                                                            }
                                                            break;
                                                        }
                                                        case 'R':
                                                        {
                                                            if ( name.Length > 11 && name[5] == 'a' && name[6] == 'd' && name[7] == 'i' && name[8] == 'c' && name[9] == 'a' && name[10] == 'l' && name[11] == 's' )
                                                            {
                                                                node = IsYiRadicals;
                                                                return true;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                        case 'V':
                                        {
                                            if ( name.Length > 19 && name[3] == 'a' && name[4] == 'r' && name[5] == 'i' && name[6] == 'a' && name[7] == 't' && name[8] == 'i' && name[9] == 'o' && name[10] == 'n' && name[11] == 'S' && name[12] == 'e' && name[13] == 'l' && name[14] == 'e' && name[15] == 'c' && name[16] == 't' && name[17] == 'o' && name[18] == 'r' && name[19] == 's' )
                                            {
                                                node = IsVariationSelectors;
                                                return true;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                node = default;
                return false;
            }

            /// <summary>
            /// Converts a <see cref="UnicodeCategory"/> back into its regex name.
            /// </summary>
            /// <paramref name="category">The category to convert.</paramref>
            /// <returns>The regex name of the category.</returns>
            public static String? ToString ( UnicodeCategory category )
            {
                return category switch
                {
                    UnicodeCategory.UppercaseLetter => "Lu",
                    UnicodeCategory.LowercaseLetter => "Ll",
                    UnicodeCategory.TitlecaseLetter => "Lt",
                    UnicodeCategory.ModifierLetter => "Lm",
                    UnicodeCategory.OtherLetter => "Lo",
                    UnicodeCategory.NonSpacingMark => "Mn",
                    UnicodeCategory.SpacingCombiningMark => "Mc",
                    UnicodeCategory.EnclosingMark => "Me",
                    UnicodeCategory.DecimalDigitNumber => "Nd",
                    UnicodeCategory.LetterNumber => "Nl",
                    UnicodeCategory.OtherNumber => "No",
                    UnicodeCategory.SpaceSeparator => "Zs",
                    UnicodeCategory.LineSeparator => "Zl",
                    UnicodeCategory.ParagraphSeparator => "Zp",
                    UnicodeCategory.Control => "Cc",
                    UnicodeCategory.Format => "Cf",
                    UnicodeCategory.Surrogate => "Cs",
                    UnicodeCategory.PrivateUse => "Co",
                    UnicodeCategory.ConnectorPunctuation => "Pc",
                    UnicodeCategory.DashPunctuation => "Pd",
                    UnicodeCategory.OpenPunctuation => "Ps",
                    UnicodeCategory.ClosePunctuation => "Pe",
                    UnicodeCategory.InitialQuotePunctuation => "Pi",
                    UnicodeCategory.FinalQuotePunctuation => "Pf",
                    UnicodeCategory.OtherPunctuation => "Po",
                    UnicodeCategory.MathSymbol => "Sm",
                    UnicodeCategory.CurrencySymbol => "Sc",
                    UnicodeCategory.ModifierSymbol => "Sk",
                    UnicodeCategory.OtherSymbol => "So",
                    UnicodeCategory.OtherNotAssigned => "Cn",
                    _ => null
                };
            }
        }
    }
}

