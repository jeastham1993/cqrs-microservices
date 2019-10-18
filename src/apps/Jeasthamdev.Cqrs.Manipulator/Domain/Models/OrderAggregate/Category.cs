using System;
using Newtonsoft.Json;

namespace Jeasthamdev.Cqrs.Manipulator.Domain.Models
{
    public sealed class Category
    {
        [JsonProperty]
        public int Id { get; private set; }

        [JsonProperty]
        public string Code { get; private set; }

        [JsonProperty]
        public string Description { get; set; }
    }
}