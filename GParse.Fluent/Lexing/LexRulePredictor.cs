using System;
using System.Collections.Generic;
using System.Linq;

namespace GParse.Fluent.Lexing
{
    /// <summary>
    /// Really basic predictor (probably doesn't improves
    /// absolutely nothing)
    /// </summary>
    public class LexRulePredictor
    {
        /// <summary>
        /// Stores feedback based on first char of matched rule
        /// </summary>
        private readonly Dictionary<Char, Dictionary<String, Int64>> CharBasedResults = new Dictionary<Char, Dictionary<String, Int64>> ( );

        /// <summary>
        /// Stores feedback on general occurrences of the rule
        /// </summary>
        private readonly Dictionary<String, Int64> GeneralResults = new Dictionary<String, Int64> ( );

        /// <summary>
        /// The rule names for initialization
        /// </summary>
        private readonly String[] Rules;

        public LexRulePredictor ( String[] rules )
        {
            this.Rules = rules;
        }

        /// <summary>
        /// Initializes a character in the
        /// <see cref="CharBasedResults" /> set
        /// </summary>
        /// <param name="ch"></param>
        private void InitializeChar ( Char ch )
        {
            this.CharBasedResults[ch] = new Dictionary<String, Int64> ( );
            foreach ( var rule in this.Rules )
                this.CharBasedResults[ch][rule] = 0;
        }

        /// <summary>
        /// Registers feedback
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="matchedRuleName"></param>
        public void StoreResult ( Char ch, String matchedRuleName )
        {
            if ( !this.CharBasedResults.ContainsKey ( ch ) )
                this.InitializeChar ( ch );

            foreach ( KeyValuePair<String, Int64> kv in this.CharBasedResults[ch] )
            {
                if ( kv.Key == matchedRuleName )
                    this.CharBasedResults[ch][matchedRuleName]++;
                else
                    this.CharBasedResults[ch][matchedRuleName]--;
            }

            foreach ( var ruleName in this.Rules )
            {
                if ( ruleName == matchedRuleName )
                    this.GeneralResults[ruleName]++;
                else
                    this.GeneralResults[ruleName]--;
            }
        }

        /// <summary>
        /// Suggests a rule name based on previous results (based
        /// on current char if possible otherwise uses general results)
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public IEnumerable<String> Suggest ( Char ch )
        {
            return ( this.CharBasedResults.ContainsKey ( ch )
                    ? this.CharBasedResults[ch]
                    : this.GeneralResults )
                .OrderBy ( kv => kv.Value )
                .Select ( kv => kv.Key );
        }
    }
}
