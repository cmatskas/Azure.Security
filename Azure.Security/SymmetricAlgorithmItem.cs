using System;

namespace Azure.Security
{
    using System.Security.Cryptography;

    public class SymmetricAlgorithmItem
    {
        public SymmetricAlgorithm Algorithm { get; set; }
        public Guid UserId { get; set; }
    }
}
