using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Verbose.Utilities
{
    public class NoDuplicatesList<T> : List<T>
    {
        public NoDuplicatesList ( ) : base ( )
        {
        }

        public NoDuplicatesList ( Int32 capacity ) : base ( capacity )
        {
        }

        public NoDuplicatesList ( IEnumerable<T> collection ) : base ( )
        {
            foreach ( T matcher in collection )
                this.Add ( matcher );
        }

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

        public new void Add ( T item )
        {
            if ( this.Find ( i => i.GetHashCode ( ) == item.GetHashCode ( ) ) != default )
                base.Add ( item );
        }

        public new void AddRange ( IEnumerable<T> items )
        {
            foreach ( T item in items )
                this.Add ( item );
        }

        public new void Insert ( Int32 index, T item )
        {
            if ( !this.Contains ( item ) )
                base.Insert ( index, item );
        }

        public new void InsertRange ( Int32 index, IEnumerable<T> items )
        {
            base.InsertRange ( index, items.Where ( item => !this.Contains ( item ) ) );
        }
    }
}
