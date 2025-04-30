using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Collections;

namespace Better.Identifier.Runtime.Extensions
{
    public static class IdentifierExtensions
    {
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