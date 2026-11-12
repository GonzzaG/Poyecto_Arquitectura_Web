using System.Data.Entity.Migrations.Infrastructure;
using System.Resources;

namespace DAL.Migrations
{
    public sealed partial class IntegridadUsuariosObjetosSha256 : IMigrationMetadata
    {
        string IMigrationMetadata.Id { get { return "202701021200000_IntegridadUsuariosObjetosSha256"; } }
        string IMigrationMetadata.Source { get { return null; } }
        string IMigrationMetadata.Target
        {
            get { return new ResourceManager(typeof(IntegridadUsuariosObjetosSha256)).GetString("Target"); }
        }
    }
}
