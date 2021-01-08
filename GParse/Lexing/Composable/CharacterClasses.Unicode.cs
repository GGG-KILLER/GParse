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
    public static partial class CharacterClasses
    {
        /// <summary>
        /// The class containing all regex unicode character categories and code blocks.
        /// </summary>
        [SuppressMessage ( "Design", "CA1034:Nested types should not be visible", Justification = "It'd be rather annoying if these were all in the root class." )]
        [SuppressMessage ( "Naming", "CA1724:Type names should not match namespaces", Justification = "It's a nested class so there's no problem." )]
        public static class Unicode
        {
            #region Fields

            /// <summary>
            /// The Lu character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lu = new ( UnicodeCategory.UppercaseLetter ); // CategoryFlagSet(Letter, Uppercase)

            /// <summary>
            /// The Ll character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Ll = new ( UnicodeCategory.LowercaseLetter ); // CategoryFlagSet(Letter, Lowercase)

            /// <summary>
            /// The Lt character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lt = new ( UnicodeCategory.TitlecaseLetter ); // CategoryFlagSet(Letter, Titlecase)

            /// <summary>
            /// The Lm character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lm = new ( UnicodeCategory.ModifierLetter ); // CategoryFlagSet(Letter, Modifier)

            /// <summary>
            /// The Lo character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Lo = new ( UnicodeCategory.OtherLetter ); // CategoryFlagSet(Letter, Other)

            /// <summary>
            /// The L character category/block node.
            /// </summary>
            public static readonly Set L = new ( UnicodeCategory.UppercaseLetter, UnicodeCategory.LowercaseLetter, UnicodeCategory.TitlecaseLetter, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherLetter ); // CategoryFlagSet(Lu, Ll, Lt, Lm, Lo)

            /// <summary>
            /// The Mn character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Mn = new ( UnicodeCategory.NonSpacingMark ); // CategoryFlagSet(Mark, Nonspacing)

            /// <summary>
            /// The Mc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Mc = new ( UnicodeCategory.SpacingCombiningMark ); // CategoryFlagSet(Mark, SpacingCombining)

            /// <summary>
            /// The Me character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Me = new ( UnicodeCategory.EnclosingMark ); // CategoryFlagSet(Mark, Enclosing)

            /// <summary>
            /// The M character category/block node.
            /// </summary>
            public static readonly Set M = new ( UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark, UnicodeCategory.EnclosingMark ); // CategoryFlagSet(Mn, Mc, Me)

            /// <summary>
            /// The Nd character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Nd = new ( UnicodeCategory.DecimalDigitNumber ); // CategoryFlagSet(Number, DecimalDigit)

            /// <summary>
            /// The Nl character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Nl = new ( UnicodeCategory.LetterNumber ); // CategoryFlagSet(Number, Letter)

            /// <summary>
            /// The No character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal No = new ( UnicodeCategory.OtherNumber ); // CategoryFlagSet(Number, Other)

            /// <summary>
            /// The N character category/block node.
            /// </summary>
            public static readonly Set N = new ( UnicodeCategory.DecimalDigitNumber, UnicodeCategory.LetterNumber, UnicodeCategory.OtherNumber ); // CategoryFlagSet(Nd, Nl, No)

            /// <summary>
            /// The Pc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pc = new ( UnicodeCategory.ConnectorPunctuation ); // CategoryFlagSet(Punctuation, Connector)

            /// <summary>
            /// The Pd character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pd = new ( UnicodeCategory.DashPunctuation ); // CategoryFlagSet(Punctuation, Dash)

            /// <summary>
            /// The Ps character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Ps = new ( UnicodeCategory.OpenPunctuation ); // CategoryFlagSet(Punctuation, Open)

            /// <summary>
            /// The Pe character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pe = new ( UnicodeCategory.ClosePunctuation ); // CategoryFlagSet(Punctuation, Close)

            /// <summary>
            /// The Pi character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pi = new ( UnicodeCategory.InitialQuotePunctuation ); // CategoryFlagSet(Punctuation, InitialQuote)

            /// <summary>
            /// The Pf character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Pf = new ( UnicodeCategory.FinalQuotePunctuation ); // CategoryFlagSet(Punctuation, FinalQuote)

            /// <summary>
            /// The Po character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Po = new ( UnicodeCategory.OtherPunctuation ); // CategoryFlagSet(Punctuation, Other)

            /// <summary>
            /// The P character category/block node.
            /// </summary>
            public static readonly Set P = new ( UnicodeCategory.ConnectorPunctuation, UnicodeCategory.DashPunctuation, UnicodeCategory.OpenPunctuation, UnicodeCategory.ClosePunctuation, UnicodeCategory.InitialQuotePunctuation, UnicodeCategory.FinalQuotePunctuation, UnicodeCategory.OtherPunctuation ); // CategoryFlagSet(Pc, Pd, Ps, Pe, Pi, Pf, Po)

            /// <summary>
            /// The Sm character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sm = new ( UnicodeCategory.MathSymbol ); // CategoryFlagSet(Symbol, Math)

            /// <summary>
            /// The Sc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sc = new ( UnicodeCategory.CurrencySymbol ); // CategoryFlagSet(Symbol, Currency)

            /// <summary>
            /// The Sk character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Sk = new ( UnicodeCategory.ModifierSymbol ); // CategoryFlagSet(Symbol, Modifier)

            /// <summary>
            /// The So character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal So = new ( UnicodeCategory.OtherSymbol ); // CategoryFlagSet(Symbol, Other)

            /// <summary>
            /// The S character category/block node.
            /// </summary>
            public static readonly Set S = new ( UnicodeCategory.MathSymbol, UnicodeCategory.CurrencySymbol, UnicodeCategory.ModifierSymbol, UnicodeCategory.OtherSymbol ); // CategoryFlagSet(Sm, Sc, Sk, So)

            /// <summary>
            /// The Zs character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zs = new ( UnicodeCategory.SpaceSeparator ); // CategoryFlagSet(Separator, Space)

            /// <summary>
            /// The Zl character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zl = new ( UnicodeCategory.LineSeparator ); // CategoryFlagSet(Separator, Line)

            /// <summary>
            /// The Zp character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Zp = new ( UnicodeCategory.ParagraphSeparator ); // CategoryFlagSet(Separator, Paragraph)

            /// <summary>
            /// The Z character category/block node.
            /// </summary>
            public static readonly Set Z = new ( UnicodeCategory.SpaceSeparator, UnicodeCategory.LineSeparator, UnicodeCategory.ParagraphSeparator ); // CategoryFlagSet(Zs, Zl, Zp)

            /// <summary>
            /// The Cc character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cc = new ( UnicodeCategory.Control ); // CategoryFlagSet(Control)

            /// <summary>
            /// The Cf character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cf = new ( UnicodeCategory.Format ); // CategoryFlagSet(Format)

            /// <summary>
            /// The Cs character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cs = new ( UnicodeCategory.Surrogate ); // CategoryFlagSet(Surrogate)

            /// <summary>
            /// The Co character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Co = new ( UnicodeCategory.PrivateUse ); // CategoryFlagSet(PrivateUse)

            /// <summary>
            /// The Cn character category/block node.
            /// </summary>
            public static readonly UnicodeCategoryTerminal Cn = new ( UnicodeCategory.OtherNotAssigned ); // CategoryFlagSet(Other, NotAssigned)

            /// <summary>
            /// The C character category/block node.
            /// </summary>
            public static readonly Set C = new ( UnicodeCategory.Control, UnicodeCategory.Format, UnicodeCategory.Surrogate, UnicodeCategory.PrivateUse, UnicodeCategory.OtherNotAssigned ); // CategoryFlagSet(Cc, Cf, Cs, Co, Cn)

            /// <summary>
            /// The IsBasicLatin character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBasicLatin = new ( '\u0000', '\u007F' ); // Range(0000-007F)

            /// <summary>
            /// The IsLatin-1Supplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatin1Supplement = new ( '\u0080', '\u00FF' ); // Range(0080-00FF)

            /// <summary>
            /// The IsLatinExtended-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedA = new ( '\u0100', '\u017F' ); // Range(0100-017F)

            /// <summary>
            /// The IsLatinExtended-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedB = new ( '\u0180', '\u024F' ); // Range(0180-024F)

            /// <summary>
            /// The IsIPAExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsIPAExtensions = new ( '\u0250', '\u02AF' ); // Range(0250-02AF)

            /// <summary>
            /// The IsSpacingModifierLetters character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSpacingModifierLetters = new ( '\u02B0', '\u02FF' ); // Range(02B0-02FF)

            /// <summary>
            /// The IsCombiningDiacriticalMarks character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningDiacriticalMarks = new ( '\u0300', '\u036F' ); // Range(0300-036F)

            /// <summary>
            /// The IsGreek character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreek = new ( '\u0370', '\u03FF' ); // Range(0370-03FF)

            /// <summary>
            /// The IsGreekandCoptic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreekandCoptic = new ( '\u0370', '\u03FF' ); // Range(0370-03FF)

            /// <summary>
            /// The IsCyrillic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCyrillic = new ( '\u0400', '\u04FF' ); // Range(0400-04FF)

            /// <summary>
            /// The IsCyrillicSupplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCyrillicSupplement = new ( '\u0500', '\u052F' ); // Range(0500-052F)

            /// <summary>
            /// The IsArmenian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArmenian = new ( '\u0530', '\u058F' ); // Range(0530-058F)

            /// <summary>
            /// The IsHebrew character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHebrew = new ( '\u0590', '\u05FF' ); // Range(0590-05FF)

            /// <summary>
            /// The IsArabic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabic = new ( '\u0600', '\u06FF' ); // Range(0600-06FF)

            /// <summary>
            /// The IsSyriac character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSyriac = new ( '\u0700', '\u074F' ); // Range(0700-074F)

            /// <summary>
            /// The IsThaana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsThaana = new ( '\u0780', '\u07BF' ); // Range(0780-07BF)

            /// <summary>
            /// The IsDevanagari character category/block node.
            /// </summary>
            public static readonly CharacterRange IsDevanagari = new ( '\u0900', '\u097F' ); // Range(0900-097F)

            /// <summary>
            /// The IsBengali character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBengali = new ( '\u0980', '\u09FF' ); // Range(0980-09FF)

            /// <summary>
            /// The IsGurmukhi character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGurmukhi = new ( '\u0A00', '\u0A7F' ); // Range(0A00-0A7F)

            /// <summary>
            /// The IsGujarati character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGujarati = new ( '\u0A80', '\u0AFF' ); // Range(0A80-0AFF)

            /// <summary>
            /// The IsOriya character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOriya = new ( '\u0B00', '\u0B7F' ); // Range(0B00-0B7F)

            /// <summary>
            /// The IsTamil character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTamil = new ( '\u0B80', '\u0BFF' ); // Range(0B80-0BFF)

            /// <summary>
            /// The IsTelugu character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTelugu = new ( '\u0C00', '\u0C7F' ); // Range(0C00-0C7F)

            /// <summary>
            /// The IsKannada character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKannada = new ( '\u0C80', '\u0CFF' ); // Range(0C80-0CFF)

            /// <summary>
            /// The IsMalayalam character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMalayalam = new ( '\u0D00', '\u0D7F' ); // Range(0D00-0D7F)

            /// <summary>
            /// The IsSinhala character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSinhala = new ( '\u0D80', '\u0DFF' ); // Range(0D80-0DFF)

            /// <summary>
            /// The IsThai character category/block node.
            /// </summary>
            public static readonly CharacterRange IsThai = new ( '\u0E00', '\u0E7F' ); // Range(0E00-0E7F)

            /// <summary>
            /// The IsLao character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLao = new ( '\u0E80', '\u0EFF' ); // Range(0E80-0EFF)

            /// <summary>
            /// The IsTibetan character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTibetan = new ( '\u0F00', '\u0FFF' ); // Range(0F00-0FFF)

            /// <summary>
            /// The IsMyanmar character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMyanmar = new ( '\u1000', '\u109F' ); // Range(1000-109F)

            /// <summary>
            /// The IsGeorgian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeorgian = new ( '\u10A0', '\u10FF' ); // Range(10A0-10FF)

            /// <summary>
            /// The IsHangulJamo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulJamo = new ( '\u1100', '\u11FF' ); // Range(1100-11FF)

            /// <summary>
            /// The IsEthiopic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEthiopic = new ( '\u1200', '\u137F' ); // Range(1200-137F)

            /// <summary>
            /// The IsCherokee character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCherokee = new ( '\u13A0', '\u13FF' ); // Range(13A0-13FF)

            /// <summary>
            /// The IsUnifiedCanadianAboriginalSyllabics character category/block node.
            /// </summary>
            public static readonly CharacterRange IsUnifiedCanadianAboriginalSyllabics = new ( '\u1400', '\u167F' ); // Range(1400-167F)

            /// <summary>
            /// The IsOgham character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOgham = new ( '\u1680', '\u169F' ); // Range(1680-169F)

            /// <summary>
            /// The IsRunic character category/block node.
            /// </summary>
            public static readonly CharacterRange IsRunic = new ( '\u16A0', '\u16FF' ); // Range(16A0-16FF)

            /// <summary>
            /// The IsTagalog character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTagalog = new ( '\u1700', '\u171F' ); // Range(1700-171F)

            /// <summary>
            /// The IsHanunoo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHanunoo = new ( '\u1720', '\u173F' ); // Range(1720-173F)

            /// <summary>
            /// The IsBuhid character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBuhid = new ( '\u1740', '\u175F' ); // Range(1740-175F)

            /// <summary>
            /// The IsTagbanwa character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTagbanwa = new ( '\u1760', '\u177F' ); // Range(1760-177F)

            /// <summary>
            /// The IsKhmer character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKhmer = new ( '\u1780', '\u17FF' ); // Range(1780-17FF)

            /// <summary>
            /// The IsMongolian character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMongolian = new ( '\u1800', '\u18AF' ); // Range(1800-18AF)

            /// <summary>
            /// The IsLimbu character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLimbu = new ( '\u1900', '\u194F' ); // Range(1900-194F)

            /// <summary>
            /// The IsTaiLe character category/block node.
            /// </summary>
            public static readonly CharacterRange IsTaiLe = new ( '\u1950', '\u197F' ); // Range(1950-197F)

            /// <summary>
            /// The IsKhmerSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKhmerSymbols = new ( '\u19E0', '\u19FF' ); // Range(19E0-19FF)

            /// <summary>
            /// The IsPhoneticExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsPhoneticExtensions = new ( '\u1D00', '\u1D7F' ); // Range(1D00-1D7F)

            /// <summary>
            /// The IsLatinExtendedAdditional character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLatinExtendedAdditional = new ( '\u1E00', '\u1EFF' ); // Range(1E00-1EFF)

            /// <summary>
            /// The IsGreekExtended character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGreekExtended = new ( '\u1F00', '\u1FFF' ); // Range(1F00-1FFF)

            /// <summary>
            /// The IsGeneralPunctuation character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeneralPunctuation = new ( '\u2000', '\u206F' ); // Range(2000-206F)

            /// <summary>
            /// The IsSuperscriptsandSubscripts character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSuperscriptsandSubscripts = new ( '\u2070', '\u209F' ); // Range(2070-209F)

            /// <summary>
            /// The IsCurrencySymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCurrencySymbols = new ( '\u20A0', '\u20CF' ); // Range(20A0-20CF)

            /// <summary>
            /// The IsCombiningDiacriticalMarksforSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningDiacriticalMarksforSymbols = new ( '\u20D0', '\u20FF' ); // Range(20D0-20FF)

            /// <summary>
            /// The IsCombiningMarksforSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningMarksforSymbols = new ( '\u20D0', '\u20FF' ); // Range(20D0-20FF)

            /// <summary>
            /// The IsLetterlikeSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLetterlikeSymbols = new ( '\u2100', '\u214F' ); // Range(2100-214F)

            /// <summary>
            /// The IsNumberForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsNumberForms = new ( '\u2150', '\u218F' ); // Range(2150-218F)

            /// <summary>
            /// The IsArrows character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArrows = new ( '\u2190', '\u21FF' ); // Range(2190-21FF)

            /// <summary>
            /// The IsMathematicalOperators character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMathematicalOperators = new ( '\u2200', '\u22FF' ); // Range(2200-22FF)

            /// <summary>
            /// The IsMiscellaneousTechnical character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousTechnical = new ( '\u2300', '\u23FF' ); // Range(2300-23FF)

            /// <summary>
            /// The IsControlPictures character category/block node.
            /// </summary>
            public static readonly CharacterRange IsControlPictures = new ( '\u2400', '\u243F' ); // Range(2400-243F)

            /// <summary>
            /// The IsOpticalCharacterRecognition character category/block node.
            /// </summary>
            public static readonly CharacterRange IsOpticalCharacterRecognition = new ( '\u2440', '\u245F' ); // Range(2440-245F)

            /// <summary>
            /// The IsEnclosedAlphanumerics character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEnclosedAlphanumerics = new ( '\u2460', '\u24FF' ); // Range(2460-24FF)

            /// <summary>
            /// The IsBoxDrawing character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBoxDrawing = new ( '\u2500', '\u257F' ); // Range(2500-257F)

            /// <summary>
            /// The IsBlockElements character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBlockElements = new ( '\u2580', '\u259F' ); // Range(2580-259F)

            /// <summary>
            /// The IsGeometricShapes character category/block node.
            /// </summary>
            public static readonly CharacterRange IsGeometricShapes = new ( '\u25A0', '\u25FF' ); // Range(25A0-25FF)

            /// <summary>
            /// The IsMiscellaneousSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousSymbols = new ( '\u2600', '\u26FF' ); // Range(2600-26FF)

            /// <summary>
            /// The IsDingbats character category/block node.
            /// </summary>
            public static readonly CharacterRange IsDingbats = new ( '\u2700', '\u27BF' ); // Range(2700-27BF)

            /// <summary>
            /// The IsMiscellaneousMathematicalSymbols-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousMathematicalSymbolsA = new ( '\u27C0', '\u27EF' ); // Range(27C0-27EF)

            /// <summary>
            /// The IsSupplementalArrows-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalArrowsA = new ( '\u27F0', '\u27FF' ); // Range(27F0-27FF)

            /// <summary>
            /// The IsBraillePatterns character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBraillePatterns = new ( '\u2800', '\u28FF' ); // Range(2800-28FF)

            /// <summary>
            /// The IsSupplementalArrows-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalArrowsB = new ( '\u2900', '\u297F' ); // Range(2900-297F)

            /// <summary>
            /// The IsMiscellaneousMathematicalSymbols-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousMathematicalSymbolsB = new ( '\u2980', '\u29FF' ); // Range(2980-29FF)

            /// <summary>
            /// The IsSupplementalMathematicalOperators character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSupplementalMathematicalOperators = new ( '\u2A00', '\u2AFF' ); // Range(2A00-2AFF)

            /// <summary>
            /// The IsMiscellaneousSymbolsandArrows character category/block node.
            /// </summary>
            public static readonly CharacterRange IsMiscellaneousSymbolsandArrows = new ( '\u2B00', '\u2BFF' ); // Range(2B00-2BFF)

            /// <summary>
            /// The IsCJKRadicalsSupplement character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKRadicalsSupplement = new ( '\u2E80', '\u2EFF' ); // Range(2E80-2EFF)

            /// <summary>
            /// The IsKangxiRadicals character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKangxiRadicals = new ( '\u2F00', '\u2FDF' ); // Range(2F00-2FDF)

            /// <summary>
            /// The IsIdeographicDescriptionCharacters character category/block node.
            /// </summary>
            public static readonly CharacterRange IsIdeographicDescriptionCharacters = new ( '\u2FF0', '\u2FFF' ); // Range(2FF0-2FFF)

            /// <summary>
            /// The IsCJKSymbolsandPunctuation character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKSymbolsandPunctuation = new ( '\u3000', '\u303F' ); // Range(3000-303F)

            /// <summary>
            /// The IsHiragana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHiragana = new ( '\u3040', '\u309F' ); // Range(3040-309F)

            /// <summary>
            /// The IsKatakana character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKatakana = new ( '\u30A0', '\u30FF' ); // Range(30A0-30FF)

            /// <summary>
            /// The IsBopomofo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBopomofo = new ( '\u3100', '\u312F' ); // Range(3100-312F)

            /// <summary>
            /// The IsHangulCompatibilityJamo character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulCompatibilityJamo = new ( '\u3130', '\u318F' ); // Range(3130-318F)

            /// <summary>
            /// The IsKanbun character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKanbun = new ( '\u3190', '\u319F' ); // Range(3190-319F)

            /// <summary>
            /// The IsBopomofoExtended character category/block node.
            /// </summary>
            public static readonly CharacterRange IsBopomofoExtended = new ( '\u31A0', '\u31BF' ); // Range(31A0-31BF)

            /// <summary>
            /// The IsKatakanaPhoneticExtensions character category/block node.
            /// </summary>
            public static readonly CharacterRange IsKatakanaPhoneticExtensions = new ( '\u31F0', '\u31FF' ); // Range(31F0-31FF)

            /// <summary>
            /// The IsEnclosedCJKLettersandMonths character category/block node.
            /// </summary>
            public static readonly CharacterRange IsEnclosedCJKLettersandMonths = new ( '\u3200', '\u32FF' ); // Range(3200-32FF)

            /// <summary>
            /// The IsCJKCompatibility character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibility = new ( '\u3300', '\u33FF' ); // Range(3300-33FF)

            /// <summary>
            /// The IsCJKUnifiedIdeographsExtensionA character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKUnifiedIdeographsExtensionA = new ( '\u3400', '\u4DBF' ); // Range(3400-4DBF)

            /// <summary>
            /// The IsYijingHexagramSymbols character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYijingHexagramSymbols = new ( '\u4DC0', '\u4DFF' ); // Range(4DC0-4DFF)

            /// <summary>
            /// The IsCJKUnifiedIdeographs character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKUnifiedIdeographs = new ( '\u4E00', '\u9FFF' ); // Range(4E00-9FFF)

            /// <summary>
            /// The IsYiSyllables character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYiSyllables = new ( '\uA000', '\uA48F' ); // Range(A000-A48F)

            /// <summary>
            /// The IsYiRadicals character category/block node.
            /// </summary>
            public static readonly CharacterRange IsYiRadicals = new ( '\uA490', '\uA4CF' ); // Range(A490-A4CF)

            /// <summary>
            /// The IsHangulSyllables character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHangulSyllables = new ( '\uAC00', '\uD7AF' ); // Range(AC00-D7AF)

            /// <summary>
            /// The IsHighSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHighSurrogates = new ( '\uD800', '\uDB7F' ); // Range(D800-DB7F)

            /// <summary>
            /// The IsHighPrivateUseSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHighPrivateUseSurrogates = new ( '\uDB80', '\uDBFF' ); // Range(DB80-DBFF)

            /// <summary>
            /// The IsLowSurrogates character category/block node.
            /// </summary>
            public static readonly CharacterRange IsLowSurrogates = new ( '\uDC00', '\uDFFF' ); // Range(DC00-DFFF)

            /// <summary>
            /// The IsPrivateUseorIsPrivateUseArea character category/block node.
            /// </summary>
            public static readonly CharacterRange IsPrivateUseorIsPrivateUseArea = new ( '\uE000', '\uF8FF' ); // Range(E000-F8FF)

            /// <summary>
            /// The IsCJKCompatibilityIdeographs character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibilityIdeographs = new ( '\uF900', '\uFAFF' ); // Range(F900-FAFF)

            /// <summary>
            /// The IsAlphabeticPresentationForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsAlphabeticPresentationForms = new ( '\uFB00', '\uFB4F' ); // Range(FB00-FB4F)

            /// <summary>
            /// The IsArabicPresentationForms-A character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabicPresentationFormsA = new ( '\uFB50', '\uFDFF' ); // Range(FB50-FDFF)

            /// <summary>
            /// The IsVariationSelectors character category/block node.
            /// </summary>
            public static readonly CharacterRange IsVariationSelectors = new ( '\uFE00', '\uFE0F' ); // Range(FE00-FE0F)

            /// <summary>
            /// The IsCombiningHalfMarks character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCombiningHalfMarks = new ( '\uFE20', '\uFE2F' ); // Range(FE20-FE2F)

            /// <summary>
            /// The IsCJKCompatibilityForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsCJKCompatibilityForms = new ( '\uFE30', '\uFE4F' ); // Range(FE30-FE4F)

            /// <summary>
            /// The IsSmallFormVariants character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSmallFormVariants = new ( '\uFE50', '\uFE6F' ); // Range(FE50-FE6F)

            /// <summary>
            /// The IsArabicPresentationForms-B character category/block node.
            /// </summary>
            public static readonly CharacterRange IsArabicPresentationFormsB = new ( '\uFE70', '\uFEFF' ); // Range(FE70-FEFF)

            /// <summary>
            /// The IsHalfwidthandFullwidthForms character category/block node.
            /// </summary>
            public static readonly CharacterRange IsHalfwidthandFullwidthForms = new ( '\uFF00', '\uFFEF' ); // Range(FF00-FFEF)

            /// <summary>
            /// The IsSpecials character category/block node.
            /// </summary>
            public static readonly CharacterRange IsSpecials = new ( '\uFFF0', '\uFFFF' ); // Range(FFF0-FFFF)


            #endregion Fields

            #region AllCategories

            /// <summary>
            /// A dictionary containing all category names and their respective nodes.
            /// </summary>
            public static readonly IImmutableDictionary<String, GrammarNode<Char>> AllCategories = ImmutableDictionary.CreateRange ( new KeyValuePair<String, GrammarNode<Char>>[]
            {
                new ( "Lu", Lu ),
                new ( "Ll", Ll ),
                new ( "Lt", Lt ),
                new ( "Lm", Lm ),
                new ( "Lo", Lo ),
                new ( "L", L ),
                new ( "Mn", Mn ),
                new ( "Mc", Mc ),
                new ( "Me", Me ),
                new ( "M", M ),
                new ( "Nd", Nd ),
                new ( "Nl", Nl ),
                new ( "No", No ),
                new ( "N", N ),
                new ( "Pc", Pc ),
                new ( "Pd", Pd ),
                new ( "Ps", Ps ),
                new ( "Pe", Pe ),
                new ( "Pi", Pi ),
                new ( "Pf", Pf ),
                new ( "Po", Po ),
                new ( "P", P ),
                new ( "Sm", Sm ),
                new ( "Sc", Sc ),
                new ( "Sk", Sk ),
                new ( "So", So ),
                new ( "S", S ),
                new ( "Zs", Zs ),
                new ( "Zl", Zl ),
                new ( "Zp", Zp ),
                new ( "Z", Z ),
                new ( "Cc", Cc ),
                new ( "Cf", Cf ),
                new ( "Cs", Cs ),
                new ( "Co", Co ),
                new ( "Cn", Cn ),
                new ( "C", C ),
                new ( "IsBasicLatin", IsBasicLatin ),
                new ( "IsLatin-1Supplement", IsLatin1Supplement ),
                new ( "IsLatinExtended-A", IsLatinExtendedA ),
                new ( "IsLatinExtended-B", IsLatinExtendedB ),
                new ( "IsIPAExtensions", IsIPAExtensions ),
                new ( "IsSpacingModifierLetters", IsSpacingModifierLetters ),
                new ( "IsCombiningDiacriticalMarks", IsCombiningDiacriticalMarks ),
                new ( "IsGreek", IsGreek ),
                new ( "IsGreekandCoptic", IsGreekandCoptic ),
                new ( "IsCyrillic", IsCyrillic ),
                new ( "IsCyrillicSupplement", IsCyrillicSupplement ),
                new ( "IsArmenian", IsArmenian ),
                new ( "IsHebrew", IsHebrew ),
                new ( "IsArabic", IsArabic ),
                new ( "IsSyriac", IsSyriac ),
                new ( "IsThaana", IsThaana ),
                new ( "IsDevanagari", IsDevanagari ),
                new ( "IsBengali", IsBengali ),
                new ( "IsGurmukhi", IsGurmukhi ),
                new ( "IsGujarati", IsGujarati ),
                new ( "IsOriya", IsOriya ),
                new ( "IsTamil", IsTamil ),
                new ( "IsTelugu", IsTelugu ),
                new ( "IsKannada", IsKannada ),
                new ( "IsMalayalam", IsMalayalam ),
                new ( "IsSinhala", IsSinhala ),
                new ( "IsThai", IsThai ),
                new ( "IsLao", IsLao ),
                new ( "IsTibetan", IsTibetan ),
                new ( "IsMyanmar", IsMyanmar ),
                new ( "IsGeorgian", IsGeorgian ),
                new ( "IsHangulJamo", IsHangulJamo ),
                new ( "IsEthiopic", IsEthiopic ),
                new ( "IsCherokee", IsCherokee ),
                new ( "IsUnifiedCanadianAboriginalSyllabics", IsUnifiedCanadianAboriginalSyllabics ),
                new ( "IsOgham", IsOgham ),
                new ( "IsRunic", IsRunic ),
                new ( "IsTagalog", IsTagalog ),
                new ( "IsHanunoo", IsHanunoo ),
                new ( "IsBuhid", IsBuhid ),
                new ( "IsTagbanwa", IsTagbanwa ),
                new ( "IsKhmer", IsKhmer ),
                new ( "IsMongolian", IsMongolian ),
                new ( "IsLimbu", IsLimbu ),
                new ( "IsTaiLe", IsTaiLe ),
                new ( "IsKhmerSymbols", IsKhmerSymbols ),
                new ( "IsPhoneticExtensions", IsPhoneticExtensions ),
                new ( "IsLatinExtendedAdditional", IsLatinExtendedAdditional ),
                new ( "IsGreekExtended", IsGreekExtended ),
                new ( "IsGeneralPunctuation", IsGeneralPunctuation ),
                new ( "IsSuperscriptsandSubscripts", IsSuperscriptsandSubscripts ),
                new ( "IsCurrencySymbols", IsCurrencySymbols ),
                new ( "IsCombiningDiacriticalMarksforSymbols", IsCombiningDiacriticalMarksforSymbols ),
                new ( "IsCombiningMarksforSymbols", IsCombiningMarksforSymbols ),
                new ( "IsLetterlikeSymbols", IsLetterlikeSymbols ),
                new ( "IsNumberForms", IsNumberForms ),
                new ( "IsArrows", IsArrows ),
                new ( "IsMathematicalOperators", IsMathematicalOperators ),
                new ( "IsMiscellaneousTechnical", IsMiscellaneousTechnical ),
                new ( "IsControlPictures", IsControlPictures ),
                new ( "IsOpticalCharacterRecognition", IsOpticalCharacterRecognition ),
                new ( "IsEnclosedAlphanumerics", IsEnclosedAlphanumerics ),
                new ( "IsBoxDrawing", IsBoxDrawing ),
                new ( "IsBlockElements", IsBlockElements ),
                new ( "IsGeometricShapes", IsGeometricShapes ),
                new ( "IsMiscellaneousSymbols", IsMiscellaneousSymbols ),
                new ( "IsDingbats", IsDingbats ),
                new ( "IsMiscellaneousMathematicalSymbols-A", IsMiscellaneousMathematicalSymbolsA ),
                new ( "IsSupplementalArrows-A", IsSupplementalArrowsA ),
                new ( "IsBraillePatterns", IsBraillePatterns ),
                new ( "IsSupplementalArrows-B", IsSupplementalArrowsB ),
                new ( "IsMiscellaneousMathematicalSymbols-B", IsMiscellaneousMathematicalSymbolsB ),
                new ( "IsSupplementalMathematicalOperators", IsSupplementalMathematicalOperators ),
                new ( "IsMiscellaneousSymbolsandArrows", IsMiscellaneousSymbolsandArrows ),
                new ( "IsCJKRadicalsSupplement", IsCJKRadicalsSupplement ),
                new ( "IsKangxiRadicals", IsKangxiRadicals ),
                new ( "IsIdeographicDescriptionCharacters", IsIdeographicDescriptionCharacters ),
                new ( "IsCJKSymbolsandPunctuation", IsCJKSymbolsandPunctuation ),
                new ( "IsHiragana", IsHiragana ),
                new ( "IsKatakana", IsKatakana ),
                new ( "IsBopomofo", IsBopomofo ),
                new ( "IsHangulCompatibilityJamo", IsHangulCompatibilityJamo ),
                new ( "IsKanbun", IsKanbun ),
                new ( "IsBopomofoExtended", IsBopomofoExtended ),
                new ( "IsKatakanaPhoneticExtensions", IsKatakanaPhoneticExtensions ),
                new ( "IsEnclosedCJKLettersandMonths", IsEnclosedCJKLettersandMonths ),
                new ( "IsCJKCompatibility", IsCJKCompatibility ),
                new ( "IsCJKUnifiedIdeographsExtensionA", IsCJKUnifiedIdeographsExtensionA ),
                new ( "IsYijingHexagramSymbols", IsYijingHexagramSymbols ),
                new ( "IsCJKUnifiedIdeographs", IsCJKUnifiedIdeographs ),
                new ( "IsYiSyllables", IsYiSyllables ),
                new ( "IsYiRadicals", IsYiRadicals ),
                new ( "IsHangulSyllables", IsHangulSyllables ),
                new ( "IsHighSurrogates", IsHighSurrogates ),
                new ( "IsHighPrivateUseSurrogates", IsHighPrivateUseSurrogates ),
                new ( "IsLowSurrogates", IsLowSurrogates ),
                new ( "IsPrivateUseorIsPrivateUseArea", IsPrivateUseorIsPrivateUseArea ),
                new ( "IsCJKCompatibilityIdeographs", IsCJKCompatibilityIdeographs ),
                new ( "IsAlphabeticPresentationForms", IsAlphabeticPresentationForms ),
                new ( "IsArabicPresentationForms-A", IsArabicPresentationFormsA ),
                new ( "IsVariationSelectors", IsVariationSelectors ),
                new ( "IsCombiningHalfMarks", IsCombiningHalfMarks ),
                new ( "IsCJKCompatibilityForms", IsCJKCompatibilityForms ),
                new ( "IsSmallFormVariants", IsSmallFormVariants ),
                new ( "IsArabicPresentationForms-B", IsArabicPresentationFormsB ),
                new ( "IsHalfwidthandFullwidthForms", IsHalfwidthandFullwidthForms ),
                new ( "IsSpecials", IsSpecials ),
            } );

            #endregion AllCategories

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

