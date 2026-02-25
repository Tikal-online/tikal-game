using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Identity.IntegrationTests.Extensions;

public static class DatabaseFacadeExtensions
{
    extension(DatabaseFacade databaseFacade)
    {
        public void DropTables()
        {
            const string sql =
                """
                DO $$
                DECLARE r RECORD;
                BEGIN
                    FOR r IN (
                        SELECT nspname 
                        FROM pg_namespace 
                        WHERE nspname NOT IN ('pg_catalog', 'information_schema', 'public')
                          AND nspname NOT LIKE 'pg_%'
                    ) LOOP
                        EXECUTE 'DROP SCHEMA IF EXISTS ' || quote_ident(r.nspname) || ' CASCADE';
                    END LOOP;
                END $$;
                """;

            databaseFacade.ExecuteSqlRaw(sql);
        }
    }
}