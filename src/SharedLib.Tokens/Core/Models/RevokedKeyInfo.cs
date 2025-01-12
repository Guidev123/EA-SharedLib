namespace SharedLib.Tokens.Core.Models;

record RevokedKeyInfo(string Id, string? RevokedReason = default)
{
    public string Id { get; } = Id;
    public string? RevokedReason { get; } = RevokedReason;
}