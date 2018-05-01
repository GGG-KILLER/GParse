using System;
using System.Linq.Expressions;
using GParse.Common.IO;

namespace GParse.Verbose.Abstractions
{
    public interface IPatternMatcher
    {
        Boolean IsMatch ( SourceCodeReader reader, Int32 offset = 0 );
        String Match ( SourceCodeReader reader );
        void ResetInternalState ( );

        #region Expression Trees Compilation
        Expression IsMatchExpression ( ParameterExpression reader, Expression offset );
        Expression MatchExpression ( ParameterExpression reader );
        #endregion
    }
}
