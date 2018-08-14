using System.Linq.Expressions;

namespace GParse.Verbose.Lexing.Compiler
{
    public enum FailSafeLocation
    {
        Loop,
        Other
    }

    public enum LabelTargetPreference
    {
        Primary,
        Secondary
    }

    public struct FailureHandleInfo
    {
        public readonly FailSafeLocation Location;
        public readonly LabelTargetPreference Preference;
        public readonly LabelTarget Primary;
        public readonly LabelTarget Secondary;

        public FailureHandleInfo ( FailSafeLocation loc, LabelTargetPreference pref, LabelTarget primary, LabelTarget secondary = null )
        {
            this.Location   = loc;
            this.Preference = pref;
            this.Primary    = primary;
            this.Secondary  = secondary;
        }
    }
}
