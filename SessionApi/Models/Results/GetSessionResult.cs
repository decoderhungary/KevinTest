using System;
using System.Text.Json.Serialization;
using Facet;
using Repository.Entities;

namespace SessionApi.Models.Results;

[Facet(typeof(Session), exclude:"PlayerId")]
public partial class GetSessionResult:ResultBase
{
    [JsonIgnore]
    public bool HitCache { get; set; }
}