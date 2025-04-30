using System;
using Better.Attributes.Runtime.Manipulation;
using Better.Commons.Runtime.Interfaces;
using UnityEngine;

namespace Better.Identifier.Runtime
{
    [Serializable]
    public sealed class Identifier : INameable, IEquatable<Identifier>, IComparable<Identifier>
    {
        private const int GuidByteSize = 16;
        private const int HashSeed = 17;
        private const int HashMultiplier = 31;

        [SerializeField] private string _name;
        
        [ReadOnly]
        [SerializeField] private string _id;
        
        private Guid _guid;

        public string Name => _name;
        public string Id => _id;

        public Identifier(string name) : this(name, Guid.NewGuid())
        {
        }

        private Identifier(string name, Guid id)
        {
            _name = name;
            _guid = id;
            _id = _guid.ToString();
        }

        public Identifier Join(Identifier other)
        {
            if (ReferenceEquals(other, null))
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            if (ReferenceEquals(this, other))
            {
                throw new InvalidOperationException("Cannot join Identifier with itself.");
            }

            var guidLeft = AsGuid();
            var guidRight = other.AsGuid();

            var bytesLeft = guidLeft.ToByteArray();
            var bytesRight = guidRight.ToByteArray();

            Span<byte> merged = stackalloc byte[GuidByteSize];
            for (var i = 0; i < GuidByteSize; i++)
            {
                merged[i] = (byte)(bytesLeft[i] ^ bytesRight[i]);
            }

            var newGuid = new Guid(merged);
            var newName = _name + other._name;
            return new Identifier(newName, newGuid);
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
            return obj is Identifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = HashSeed;
                hash = (hash * HashMultiplier) + Id.GetHashCode();
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
            return $"{Name} [{Id}]";
        }
    }
}