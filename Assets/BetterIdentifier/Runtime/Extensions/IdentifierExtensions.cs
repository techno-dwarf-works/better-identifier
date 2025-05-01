using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Collections;
using Better.Commons.Runtime.Utility;
using UnityEngine;

namespace Better.Identifier.Runtime.Extensions
{
    public static class IdentifierExtensions
    {
        public static bool IsEmpty(this Identifier self)
        {
            if (self == null)
            {
                var message = $"{nameof(self)} can not be null";
                DebugUtility.LogException<ArgumentNullException>(message);
                return false;
            }
            
            var isEmpty = self.Equals(Identifier.Empty);
            return isEmpty;
        }
        
        public static bool IsEmptyOrNull(this Identifier self)
        {
            if (self == null)
            {
                return true;
            }
            
            var isEmpty = self.IsEmpty();
            return isEmpty;
        }
        
        public static Identifier Join(this IEnumerable<Identifier> self)
        {
            if (self == null)
            {
                var message = $"{nameof(self)} can not be null";
                throw new ArgumentNullException(message);
            }

            var identifiers = self.ToArray();
            if (identifiers.Length <= 1)
            {
                var message = $"{nameof(identifiers.Length)} can not be less than 1";
                throw new InvalidOperationException(message);
            }

            var joinedIdentifier = identifiers[0];
            for (var index = 1; index < identifiers.Length; index++)
            {
                var identifier = identifiers[index];
                joinedIdentifier = joinedIdentifier.Join(identifier);
            }

            return joinedIdentifier;
        }
    }
}