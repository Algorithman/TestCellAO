using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AO.Database.Dao
{
    using System.Data;

    using AO.Database.Entities;

    using Dapper;

    using SmokeLounge.AOtomation.Messaging.GameData;

    public static class OrganizationDao
    {
        public static bool OrgExists(string name)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<int>(
                        "SELECT count(*) FROM organizations WHERE Name=@name", new { name }).Single().Equals(1);
            }
        }

        public static bool OrgExists(int orgId)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return
                    conn.Query<int>(
                        "SELECT count(*) FROM organizations WHERE ID=@orgId", new { orgId }).Single().Equals(1);
            }
        }




        public static bool CreateOrganization(string desiredOrgName, DateTime creationDate, int leaderId)
        {
            bool canBeCreated = !OrgExists(desiredOrgName);
            if (canBeCreated)
            {
                using (IDbConnection conn = Connector.GetConnection())
                {
                    conn.Execute("INSERT INTO organizations (creation, Name, LeaderID, GovernmentForm) VALUES (@creation, @name, @leaderid, 0)", new { creation = creationDate, name = desiredOrgName, leaderid = leaderId });
                }
            }
            return canBeCreated;
        }

        public static int GetOrganizationId(string orgName)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<int>("SELECT ID FROM organizations WHERE Name=@name", new { name = orgName }).Single();
            }
        }

        public static int GetGovernmentForm(int orgId)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<int>("SELECT GovernmentForm FROM organizations WHERE ID=@orgId", new { orgId }).Single();
            }
        }

        public static DBOrganization GetOrganizationData(int orgId)
        {
            if (!OrgExists(orgId))
            {
                return null;
            }
            using (IDbConnection conn = Connector.GetConnection())
            {
                return conn.Query<DBOrganization>("SELECT * FROM organizations WHERE ID=@orgId", new { orgId }).Single();
            }
        }

        public static void SetNewPrez(int orgId, int newLeaderId)
        {
            using (IDbConnection conn = Connector.GetConnection())
            {
                conn.Execute("UPDATE organizations SET LeaderID=@leaderId WHERE ID=@orgId", new { newLeaderId, orgId });
            }
        }
    }
}
