using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Common.Utilities
{
    /// <summary>
    /// A list that stores no duplicated values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoDuplicatesList<T> : List<T>
    {
        /// <summary>
        /// Initializes this list
        /// </summary>
        public NoDuplicatesList ( ) : base ( )
        {
        }

        /// <summary>
        /// Initializes this list
        /// </summary>
        /// <param name="capacity"></param>
        public NoDuplicatesList ( Int32 capacity ) : base ( capacity )
        {
        }

        /// <summary>
        /// Initializes this list
        /// </summary>
        /// <param name="collection"></param>
        public NoDuplicatesList ( IEnumerable<T> collection ) : base ( )
        {
            foreach ( T matcher in collection )
                this.Add ( matcher );
        }

        /// <summary>
        /// Retrieves an element from this list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new T this[Int32 index]
        {
            get => base[index];

            set
            {
                if ( this.Contains ( value ) )
                    this.RemoveAt ( index );
                else
                    base[index] = value;
            }
        }

        /// <summary>
        /// Whether this list contains a given item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new Boolean Contains ( T item )
            => this.Find ( i => i.GetHashCode ( ) == item.GetHashCode ( ) ) != default;

        /// <summary>
        /// Adds an item to this list
        /// </summary>
        /// <param name="item"></param>
        public new void Add ( T item )
        {
            if ( !this.Contains ( item ) )
                base.Add ( item );
        }

        /// <summary>
        /// Adds a range of items to this list
        /// </summary>
        /// <param name="items"></param>
        public new void AddRange ( IEnumerable<T> items )
        {
            foreach ( T item in items )
                this.Add ( item );
        }

        /// <summary>
        /// Inserts an item into this list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert ( Int32 index, T item )
        {
            if ( !this.Contains ( item ) )
                base.Insert ( index, item );
        }

        /// <summary>
        /// Inserts a range of items to this list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="items"></param>
        public new void InsertRange ( Int32 index, IEnumerable<T> items ) =>
            base.InsertRange ( index, items.Where ( item => !this.Contains ( item ) ) );
    }
}
