using System;

namespace Phezu.Data {

    public interface IDataMigrator {

        public abstract Func<PlayerProfile, PlayerProfile> GetUpgrader(string profileVersion);

    }
}