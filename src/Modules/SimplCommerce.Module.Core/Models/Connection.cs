using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Core.Models
{
    public class Connection : EntityBase
    {
        public long Source { get; set; }

        public long Target { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public long CreatedById { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
