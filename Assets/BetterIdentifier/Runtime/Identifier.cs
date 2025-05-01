using System;
using System.Diagnostics;
using Better.Attributes.Runtime.Manipulation;
using Better.Commons.Runtime.Interfaces;
using Better.Identifier.Runtime.Extensions;
using UnityEngine;

namespace Better.Identifier.Runtime
{
    [DebuggerDisplay("{ToString(),nq}")]
    [Serializable]
    public sealed class Identifier : INameable, IEquatable<Identifier>, IComparable<Identifier>
    {
        public readonly static Identifier Empty = new Identifier(nameof(Empty), Guid.Empty);
        private const int HashSeed = 17;
        private const int HashMultiplier = 31;

        [SerializeField] private string _name;

        [ReadOnly]
        [SerializeField] private string _id;

        private Guid _guid;

        public string Name => _name;
        public string Id => _id;

        public Identifier() : this(string.Empty)
        {
        }

        public Identifier(string name) : this(name, Guid.NewGuid())
        {
        }

        private Identifier(string name, Guid guid)
        {
            _name = name;
            _guid = guid;
            _id = _guid.ToString();
        }

        public Identifier Join(Identifier other)
        {
            if (other.IsEmptyOrNull())
            {
                var message = nameof(other);
                throw new ArgumentNullException(message);
            }
            
            if (this.IsEmptyOrNull())
            {
                var message = "this";
                throw new ArgumentNullException(message);
            }

            if (ReferenceEquals(this, other))
            {
                var message = "Cannot join Identifier with itself";
                throw new InvalidOperationException(message);
            }

            var guid = AsGuid();
            var otherGuid = other.AsGuid();

            var guidBytes = guid.ToByteArray();
            var otherGuidBytes = otherGuid.ToByteArray();
            var minByteCount = Math.Min(guidBytes.Length, otherGuidBytes.Length);

            Span<byte> merged = stackalloc byte[minByteCount];
            for (var i = 0; i < minByteCount; i++)
            {
                merged[i] = (byte)(guidBytes[i] ^ otherGuidBytes[i]);
            }

            var joinedGuid = new Guid(merged);
            var joinedName = Name + other.Name;
            var joinedIdentifier = new Identifier(joinedName, joinedGuid);
            return joinedIdentifier;
        }

        public Guid AsGuid()
        {
            if (_guid.Equals(Guid.Empty))
            {
                _guid = Guid.Parse(_id);
            }

            return _guid;
        }

        public bool Equals(Identifier other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is Identifier otherIdentifier)
            {
                return Equals(otherIdentifier);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = HashSeed;
                hash *= HashMultiplier;
                hash += Id.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Identifier left, Identifier right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null))
            {
                return false;
            }

            if (ReferenceEquals(right, null))
            {
                return false;
            }

            var equality = left.Equals(right);
            return equality;
        }

        public static bool operator !=(Identifier left, Identifier right)
        {
            var equals = left == right;
            return !equals;
        }

        public int CompareTo(Identifier other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            var compare = string.Compare(Id, other.Id, StringComparison.Ordinal);
            return compare;
        }

        public override string ToString()
        {
            return $"{Name}: [{Id}]";
        }
    }
}