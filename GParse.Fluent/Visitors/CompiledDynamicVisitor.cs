using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common.Utilities;

namespace GParse.Fluent.Visitors
{
    /// <summary>
    /// A dynamic visitor that is compiled through expression trees
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class CompiledDynamicVisitor<T, TResult> where T : class
    {
        private static readonly Dictionary<Type, Func<T, TResult>> Cache = new Dictionary<Type, Func<T, TResult>> ( );
        private readonly LabelTarget Return;
        private readonly ParameterExpression Node;
        private readonly List<ParameterExpression> Locals;
        private readonly List<Expression> Body;

        /// <summary>
        /// Initializes this class
        /// </summary>
        protected CompiledDynamicVisitor ( )
        {
            if ( Cache.ContainsKey ( this.GetType ( ) ) )
                return;
            // Initialize the expression block stuff
            this.Locals = new List<ParameterExpression> ( );
            this.Body = new List<Expression> ( );
            this.Node = Expression.Parameter ( typeof ( T ), "node" );
            this.Return = Expression.Label ( typeof ( TResult ), "return" );
            this.Setup ( );
            this.Body.Add ( Expression.Throw ( Expression.Constant ( new NotSupportedException ( "Input type unsuported" ) ) ) );
            this.Body.Add ( Expression.Label ( this.Return, Expression.Default ( typeof ( TResult ) ) ) );
            Cache[this.GetType ( )] = Expression.Lambda<Func<T, TResult>> (
                    Expression.Block ( typeof ( TResult ), this.Locals, this.Body ), this.Node )
                .Compile ( );
        }

        #region Setup

        /// <summary>
        /// The setup method of the AST visitor
        /// </summary>
        protected abstract void Setup ( );

        /// <summary>
        /// Registers a visitor for a given type
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="action"></param>
        protected void RegisterVisitor<NodeType> ( Func<NodeType, TResult> action ) where NodeType : T
        {
            ParameterExpression local = Expression.Variable ( typeof ( NodeType ), typeof ( NodeType ).Name.ToLower ( ) );
            this.Locals.Add ( local );
            this.Body.Add ( Expression.IfThen (
                Expression.NotEqual (
                    Expression.Assign ( local, Expression.TypeAs ( this.Node, typeof ( NodeType ) ) ),
                    Expression.Constant ( null )
                ),
                Expression.Return ( this.Return, ExprUtils.Call ( action, local ) )
            ) );
        }

        #endregion Setup

        /// <summary>
        /// Visits a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public TResult Visit ( T node ) => Cache[this.GetType ( )] ( node );
    }
}
