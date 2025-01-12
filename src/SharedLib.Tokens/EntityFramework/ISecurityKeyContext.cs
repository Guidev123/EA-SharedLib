using Microsoft.EntityFrameworkCore;
using SharedLib.Tokens.Core.Models;

namespace SharedLib.Tokens.EntityFramework;

public interface ISecurityKeyContext
{
    DbSet<KeyMaterial> SecurityKeys { get; set; }
}
