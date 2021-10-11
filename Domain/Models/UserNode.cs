﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserNode
    {
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public Guid NodeId { get; set; }
        [JsonIgnore]
        public Node Node { get; set; }
        public AccessTypeId AccessTypeId { get; set; }
        [JsonIgnore]
        public AccessType AccessType { get; set; }
        public long Timestamp { get; set; }
    }
}
