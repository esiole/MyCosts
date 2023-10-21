namespace MyCosts.Api.Models.Config;

public record JwtOptions(string Audience, string Issuer, string Key, int ValidForDays);