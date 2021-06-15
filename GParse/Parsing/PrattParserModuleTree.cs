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
        private readonly Dictionary<TTokenType, Dictionary<string, List<TModule>>> _modulesWithTargetId = new();

        private readonly Dictionary<TTokenType, List<TModule>> _modulesWithoutTargetId = new();

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="module"></param>
        public void AddModule(TTokenType tokenType, TModule module)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (module is null)
                throw new ArgumentNullException(nameof(module));

            if (!_modulesWithoutTargetId.TryGetValue(tokenType, out var list))
            {
                list = new List<TModule>();
                _modulesWithoutTargetId[tokenType] = list;
            }

            list.Add(module);
        }

        /// <summary>
        /// Adds a module to the tree
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="module"></param>
        public void AddModule(TTokenType tokenType, string id, TModule module)
        {
            if (tokenType is null)
                throw new ArgumentNullException(nameof(tokenType));
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            if (module is null)
                throw new ArgumentNullException(nameof(module));

            if (!_modulesWithTargetId.TryGetValue(tokenType, out var dict))
            {
                dict = new Dictionary<string, List<TModule>>(StringComparer.Ordinal);
                _modulesWithTargetId[tokenType] = dict;
            }

            if (!dict.TryGetValue(id, out var list))
            {
                list = new List<TModule>();
                dict[id] = list;
            }

            list.Add(module);
        }

        /// <summary>
        /// Returns the sorted candidates for the next token in line
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<TModule> GetSortedCandidates(ITokenReader<TTokenType> reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var peeked = reader.Lookahead();

            if (_modulesWithTargetId.TryGetValue(peeked.Type, out var dict)
                 && dict.TryGetValue(peeked.Id, out var candidates))
            {
                foreach (var candidate in candidates)
                    yield return candidate;
            }

            if (_modulesWithoutTargetId.TryGetValue(peeked.Type, out candidates))
            {
                foreach (var candidate in candidates)
                    yield return candidate;
            }
        }
    }
}