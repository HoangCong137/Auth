using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthServer.Domain.Entities
{
    public class User : IdentityUser<Guid>, IEntity<Guid>
    {
        [MaxLength(50)]
        public string FullName { get; set; }

        public string Source { get; set; }
        public string FacebookId { get; set; }
        public string AppleId { get; set; }
    }
}
