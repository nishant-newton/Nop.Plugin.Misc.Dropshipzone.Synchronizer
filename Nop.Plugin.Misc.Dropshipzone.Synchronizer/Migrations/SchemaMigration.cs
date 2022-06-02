using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/08/24 00:00:00", "Nop.Plugin.Misc.Dropshipzone.Synchronizer schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
        }
    }
}
