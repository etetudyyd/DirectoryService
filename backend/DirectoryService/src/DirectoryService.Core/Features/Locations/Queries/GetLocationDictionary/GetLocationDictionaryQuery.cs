using Core.Abstractions;
using DirectoryService.Locations.Requests;

namespace DirectoryService.Features.Locations.Queries.GetLocationDictionary;

public record GetLocationDictionaryQuery(GetLocationDictionaryRequest Request) : IQuery;