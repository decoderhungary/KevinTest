using System;
using Facet;
using Repository.Entities;

namespace SessionApi.Models.Results.SessionStart;

[Facet(typeof(Session), exclude: ["PlayerId"])]
public partial class StartSessionResult : ResultBase;