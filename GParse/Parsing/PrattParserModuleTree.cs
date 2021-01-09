using System;
using System.Collections.Generic;
using GParse.Lexing;

namespace GParse.Parsing
{
    /// <summary>
    /// A tree used to store all modules of a kind
    /// </summary>
    /// <typeparam name="TTokenType">The <see cref="Token{TTokenType}.Type"/> type.</typeparam>
    /// <typeparam name="TModule"></typeparam>
    public class PrattParserModuleTree<TTokenType, TModule>
        where TTokenType : notnull
    {
        private readonly Dictionary<TTokenType, Dictionary<String, List<TModule>>> modulesWithTargetId = new ( );

        private readonly Dictionary<TTokenType, List<TModule>> modulesWithoutTargetId = new ( );

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="module"></param>
        public void AddModule ( TTokenType tokenType, TModule module )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( module is null )
                throw new ArgumentNullException ( nameof ( module ) );

            if ( !this.modulesWithoutTargetId.TryGetValue ( tokenType, out List<TModule>? list ) )
            {
                list = new List<TModule> ( );
                this.modulesWithoutTargetId[tokenType] = list;
            }

            list.Add ( module );
        }

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="module"></param>
        public void AddModule ( TTokenType tokenType, String id, TModule module )
        {
            if ( tokenType is null )
                throw new ArgumentNullException ( nameof ( tokenType ) );
            if ( id is null )
                throw new ArgumentNullException ( nameof ( id ) );
            if ( module is null )
                throw new ArgumentNullException ( nameof ( module ) );

            if ( !this.modulesWithTargetId.TryGetValue ( tokenType, out Dictionary<String, List<TModule>>? dict ) )
            {
                dict = new Dictionary<String, List<TModule>> ( StringComparer.Ordinal );
                this.modulesWithTargetId[tokenType] = dict;
            }

            if ( !dict.TryGetValue ( id, out List<TModule>? list ) )
            {
                list = new List<TModule> ( );
                dict[id] = list;
            }

            list.Add ( module );
        }

        /// <summary>
        /// Returns the sorted candidates for the next token in line
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<TModule> GetSortedCandidates ( ITokenReader<TTokenType> reader )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

            Token<TTokenType> peeked = reader.Lookahead ( );

            if ( this.modulesWithTargetId.TryGetValue ( peeked.Type, out Dictionary<String, List<TModule>>? dict )
                 && dict.TryGetValue ( peeked.Id, out List<TModule>? candidates ) )
            {
                foreach ( TModule candidate in candidates )
                    yield return candidate;
            }

            if ( this.modulesWithoutTargetId.TryGetValue ( peeked.Type, out candidates ) )
            {
                foreach ( TModule candidate in candidates )
                    yield return candidate;
            }
        }
    }
}
