using System;
using System.Linq.Expressions;

namespace GParse.Verbose.Compilation

{
    public static class ExpressionUtils
    {
        #region GetMethodCallExpression

        public static Expression GetMethodCallExpression<Class> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 )
            } ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1, A2> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 ),
                typeof ( A2 )
            } ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1, A2, A3> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 ),
                typeof ( A2 ),
                typeof ( A3 )
            } ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1, A2, A3, A4> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 ),
                typeof ( A2 ),
                typeof ( A3 ),
                typeof ( A4 )
            } ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1, A2, A3, A4, A5> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 ),
                typeof ( A2 ),
                typeof ( A3 ),
                typeof ( A4 ),
                typeof ( A5 )
            } ), args );
        }

        public static Expression GetMethodCallExpression<Class, A1, A2, A3, A4, A5, A6> ( Expression inst, String name, params Expression[] args )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name, new[] {
                typeof ( A1 ),
                typeof ( A2 ),
                typeof ( A3 ),
                typeof ( A4 ),
                typeof ( A5 ),
                typeof ( A6 )
            } ), args );
        }

        #endregion GetMethodCallExpression

        public static Expression GetDelegateCallExpression ( Delegate @delegate, params Expression[] args )
        {
            return Expression.Call ( @delegate.Target != null ? Expression.Constant ( @delegate.Target ) : null,
                @delegate.Method, args );
        }

        public static Expression GetLambdaExpressionCallExpression ( LambdaExpression lambda, params Expression[] args )
        {
            return Expression.Call ( lambda.Compile ( ).Method, args );
        }
    }
}
