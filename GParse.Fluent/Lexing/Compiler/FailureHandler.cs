using System.Linq.Expressions;

namespace GParse.Fluent.Lexing.Compiler
{
    /// <summary>
    /// Indicates the kind of location where failures can be handled
    /// </summary>
    internal enum FailSafeLocation
    {
        /// <summary>
        /// A loop
        /// </summary>
        Loop,

        /// <summary>
        /// Other locations
        /// </summary>
        Other
    }

    /// <summary>
    /// The label that should be jumped to in case of failure
    /// </summary>
    internal enum LabelTargetPreference
    {
        /// <summary>
        /// </summary>
        Primary,

        /// <summary>
        /// </summary>
        Secondary
    }

    /// <summary>
    /// Defines the structure of the information that a failure handler will use
    /// </summary>
    internal readonly struct FailureHandleInfo
    {
        /// <summary>
        /// The kind of location we're in
        /// </summary>
        public readonly FailSafeLocation Location;

        /// <summary>
        /// The preferential label to jump to
        /// </summary>
        public readonly LabelTargetPreference Preference;

        /// <summary>
        /// The primary label
        /// </summary>
        public readonly LabelTarget Primary;

        /// <summary>
        /// The secondary label
        /// </summary>
        public readonly LabelTarget Secondary;

        /// <summary>
        /// Initializes this struct
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="pref"></param>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public FailureHandleInfo ( FailSafeLocation loc, LabelTargetPreference pref, LabelTarget primary, LabelTarget secondary = null )
        {
            this.Location = loc;
            this.Preference = pref;
            this.Primary = primary;
            this.Secondary = secondary;
        }
    }
}
