using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.Entities
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
