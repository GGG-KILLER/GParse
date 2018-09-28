using System;
using System.Reflection;
using System.Linq.Expressions;

namespace GParse.Common.Utilities

{
    public static class ExprUtils
    {
        #region MethodCall (Generated Code)

        public static MethodCallExpression MethodCall<Class> ( Expression inst, String name )
        {
            return Expression.Call ( inst, typeof ( Class ).GetMethod ( name ) );
        }

        public static MethodCallExpression MethodCall ( Type type, Expression inst, String name )
        {
            return Expression.Call ( inst, type.GetMethod ( name ) );
        }

        public static MethodCallExpression MethodCall<T1> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4, T5> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4, T5> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4, T5, T6> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4, T5, T6> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4, T5, T6, T7> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4, T5, T6, T7> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4, T5, T6, T7, T8> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) ),
                args.Length > 7 ? args[7] : ( @params[7].HasDefaultValue ? Expression.Constant ( @params[7].DefaultValue ) : throw new InvalidOperationException ( "Argument #7 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4, T5, T6, T7, T8> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) ),
                args.Length > 7 ? args[7] : ( @params[7].HasDefaultValue ? Expression.Constant ( @params[7].DefaultValue ) : throw new InvalidOperationException ( "Argument #7 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<T1, T2, T3, T4, T5, T6, T7, T8, T9> ( Type type, Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = type.GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
                typeof ( T9 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) ),
                args.Length > 7 ? args[7] : ( @params[7].HasDefaultValue ? Expression.Constant ( @params[7].DefaultValue ) : throw new InvalidOperationException ( "Argument #7 does not have a default value." ) ),
                args.Length > 8 ? args[8] : ( @params[8].HasDefaultValue ? Expression.Constant ( @params[8].DefaultValue ) : throw new InvalidOperationException ( "Argument #8 does not have a default value." ) )
            );
        }

        public static MethodCallExpression MethodCall<Class, T1, T2, T3, T4, T5, T6, T7, T8, T9> ( Expression inst, String name, params Expression[] args )
        {
            MethodInfo method = typeof ( Class ).GetMethod ( name, new[] {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
                typeof ( T9 ),
            } );
            ParameterInfo[] @params = method.GetParameters ( );

            return Expression.Call ( inst, method,
                args.Length > 0 ? args[0] : ( @params[0].HasDefaultValue ? Expression.Constant ( @params[0].DefaultValue ) : throw new InvalidOperationException ( "Argument #0 does not have a default value." ) ),
                args.Length > 1 ? args[1] : ( @params[1].HasDefaultValue ? Expression.Constant ( @params[1].DefaultValue ) : throw new InvalidOperationException ( "Argument #1 does not have a default value." ) ),
                args.Length > 2 ? args[2] : ( @params[2].HasDefaultValue ? Expression.Constant ( @params[2].DefaultValue ) : throw new InvalidOperationException ( "Argument #2 does not have a default value." ) ),
                args.Length > 3 ? args[3] : ( @params[3].HasDefaultValue ? Expression.Constant ( @params[3].DefaultValue ) : throw new InvalidOperationException ( "Argument #3 does not have a default value." ) ),
                args.Length > 4 ? args[4] : ( @params[4].HasDefaultValue ? Expression.Constant ( @params[4].DefaultValue ) : throw new InvalidOperationException ( "Argument #4 does not have a default value." ) ),
                args.Length > 5 ? args[5] : ( @params[5].HasDefaultValue ? Expression.Constant ( @params[5].DefaultValue ) : throw new InvalidOperationException ( "Argument #5 does not have a default value." ) ),
                args.Length > 6 ? args[6] : ( @params[6].HasDefaultValue ? Expression.Constant ( @params[6].DefaultValue ) : throw new InvalidOperationException ( "Argument #6 does not have a default value." ) ),
                args.Length > 7 ? args[7] : ( @params[7].HasDefaultValue ? Expression.Constant ( @params[7].DefaultValue ) : throw new InvalidOperationException ( "Argument #7 does not have a default value." ) ),
                args.Length > 8 ? args[8] : ( @params[8].HasDefaultValue ? Expression.Constant ( @params[8].DefaultValue ) : throw new InvalidOperationException ( "Argument #8 does not have a default value." ) )
            );
        }

        #endregion MethodCall (Generated Code)

        #region New (Generated Code)

        public static NewExpression New<Class> ( )
        {
            return Expression.New ( typeof ( Class ) );
        }

        public static NewExpression New<Class, T1> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4, T5> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4, T5, T6> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4, T5, T6, T7> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4, T5, T6, T7, T8> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
            } );

            return Expression.New ( constructor, args );
        }
        public static NewExpression New<Class, T1, T2, T3, T4, T5, T6, T7, T8, T9> ( params Expression[] args )
        {
            ConstructorInfo constructor = typeof ( Class ).GetConstructor ( new[]
            {
                typeof ( T1 ),
                typeof ( T2 ),
                typeof ( T3 ),
                typeof ( T4 ),
                typeof ( T5 ),
                typeof ( T6 ),
                typeof ( T7 ),
                typeof ( T8 ),
                typeof ( T9 ),
            } );

            return Expression.New ( constructor, args );
        }

        #endregion New (Generated Code)

        #region Throw (Generated Code)

        public static UnaryExpression Throw<Class> ( ) where Class : Exception
        {
            return Expression.Throw ( New<Class> ( ), typeof ( Class ) );
        }

        public static UnaryExpression Throw<Class, T1> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4, T5> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4, T5> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4, T5, T6> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4, T5, T6> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4, T5, T6, T7> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4, T5, T6, T7> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4, T5, T6, T7, T8> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4, T5, T6, T7, T8> ( args ), typeof ( Class ) );
        }
        public static UnaryExpression Throw<Class, T1, T2, T3, T4, T5, T6, T7, T8, T9> ( params Expression[] args ) where Class : Exception
        {
            return Expression.Throw ( New<Class, T1, T2, T3, T4, T5, T6, T7, T8, T9> ( args ), typeof ( Class ) );
        }

        #endregion Throw (Generated Code)

        public static IndexExpression IndexGet ( Expression inst, Expression key )
        {
            return Expression.Property ( inst, "Item", key );
        }

        public static BinaryExpression IndexSet ( Expression inst, Expression key, Expression val )
        {
            Expression indexExpr = IndexGet ( inst, key );
            return Expression.Assign ( indexExpr, val );
        }

        public static MemberExpression FieldGet<T> ( Expression inst, String name )
        {
            return Expression.Field ( inst, typeof ( T ), name );
        }

        public static BinaryExpression FieldSet<T> ( Expression inst, String name, Expression val )
        {
            return Expression.Assign ( FieldGet<T> ( inst, name ), val );
        }

        public static MemberExpression PropertyGet<T> ( Expression inst, String name )
        {
            return Expression.Property ( inst, typeof ( T ), name );
        }

        public static BinaryExpression PropertySet<T> ( Expression inst, String name, Expression val )
        {
            return Expression.Assign ( PropertyGet<T> ( inst, name ), val );
        }

        public static MethodCallExpression Call ( Delegate @delegate, params Expression[] args )
        {
            return Expression.Call ( @delegate.Target != null ? Expression.Constant ( @delegate.Target ) : null,
                @delegate.Method, args );
        }

        public static MethodCallExpression Call ( LambdaExpression lambda, params Expression[] args )
        {
            return Expression.Call ( lambda.Compile ( ).Method, args );
        }
    }
}
