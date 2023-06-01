using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TelegramBotTest
{
    class DBWorker
    {
        public string Host { get; set; }
        public string DBUser { get; set; }
        public string DBname { get; set; }
        public string DBpass { get; set; }
        public string DBport { get; set; }

        private static DBWorker s_instance;
        private readonly ILogger<DBWorker> _logger;

        private DBWorker(String Host, String DBUser, String DBname,
            String DBport, String DBpass, ILogger<DBWorker> logger)
        {
            this.Host = Host;
            this.DBUser = DBUser;
            this.DBname = DBname;
            this.DBport = DBport;
            this.DBpass = DBpass;
            this._logger = logger;
        }
        public static DBWorker getInstance(String Host, String DBUser, String DBname,
            String DBport, String DBpass, ILogger<DBWorker> logger)
        {
            return s_instance ??= new DBWorker(Host, DBUser, DBname, DBport, DBpass, logger);
        }

        private string getConnString()
        {
            return String.Format(
                "Server={0};UserName={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                Host,
                DBUser,
                DBname,
                DBport,
                DBpass
            );
        }
        public bool canConnect()
        {

            if (Host != null && DBUser != null && DBname != null && DBport != null
                && DBpass != null)
            {
                return true;
            }

            return false;
        }
        private NpgsqlConnection connect()
        {

            if (!this.canConnect()) return null;
            NpgsqlConnection conn = null;

            try
            {
                conn = new NpgsqlConnection(this.getConnString());
                _logger.LogDebug("Opening Connection ...");
                conn.Open();
                _logger.LogDebug("Connection has been opened");
            }
            catch (NpgsqlException e)
            {
                _logger.LogWarning("Connection Open Faild : {e}", e.Message);
                conn = null;
            }

            return conn;
        }

        public bool insertToTable(string tableName, string[] attributes, string[] values = null,
            string strPool = null)
        {
            bool result = false;
            var conn = this.connect();

            if (conn == null) return false;
            if (values == null && strPool == null) return false;

            try
            {
                string str = $"INSERT INTO {tableName}(";

                for (int i = 0; i < attributes.Length; i++)
                {

                    if (i != attributes.Length - 1)
                    {
                        str += attributes[i] + ", ";
                    }
                    else
                    {
                        str += attributes[i] + ")";
                    }
                }

                if (strPool == null)
                {
                    str += " VALUES(";

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i != values.Length - 1)
                        {
                            str += values[i] + ", ";
                        }
                        else
                        {
                            str += values[i] + ");";
                        }
                    }
                }
                else
                {
                    str = $"{str} VALUES{strPool}";
                }

                _logger.LogInformation("Execute Insert command: {str}", str);

                var command = new NpgsqlCommand(str, conn);
                command.ExecuteNonQuery();

                _logger.LogInformation("Insert command has been Executed");
                result = true;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error Insert to Table[{tableName}] : {e}", tableName, e);
                result = false;
            }

            return result;
        }
        /**
         * <summary>
         *  Read data from table.
         * <example>
         *  For example:
         *  <code>
         *    SELECT <paramref name="attributes"/> FROM <paramref name="tableName"/>
         *    WHERE <paramref name="where"/>
         *  </code>
         * </example>
         * <returns>
         *  Return <typeparamref name="NpgsqlDataReader"></typeparamref>
         *  with result of query if good;
         *  <para>otherwise <typeparamref name="null"></typeparamref> if error</para>
         * </returns>
         * </summary>
         */
        public NpgsqlDataReader readFromTable(string tableName, string[] attributes = null, string[] where = null)
        {
            var conn = this.connect();
            if (conn == null) return null;

            NpgsqlDataReader reader = null;

            try
            {
                string str = "SELECT ";

                if (attributes != null)
                {
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (i < attributes.Length - 1)
                        {
                            str += attributes[i] + ", ";
                        }
                        else
                        {
                            str += attributes[i];
                        }
                    }
                }
                else
                {
                    str += "* ";
                }
                str += $" FROM {tableName}";

                if (where != null)
                {
                    str += " WHERE ";
                    for (int i = 0; i < where.Length; i++)
                    {
                        if (i != where.Length - 1)
                        {
                            str += where[i] + " AND ";
                        }
                        else
                        {
                            str += where[i];
                        }
                    }
                }
                str += ";";

                _logger.Log(LogLevel.Information, "Execute Read command: {str}", str);
                reader = (new NpgsqlCommand(str, conn)).ExecuteReader();
                _logger.Log(LogLevel.Information, "Read command has been Executed");
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error Read from Table[{tableName}] : {e}", tableName, e.Message);
                reader = null;
            }

            return reader;
        }
        /**
         * <summary> 
         * Update data in table
         * <example>
         * For example:
         * <code>
         *    UPDATE <paramref name="tableName"/> SET <paramref name="attributes"/> = <paramref name="values"/>, ...
         *    WHERE <paramref name="where"/>;
         * </code>
         * </example>
         * </summary>
         * <returns>
         *  return number of updated rows in table <paramref name="tableName"/> if good;
         *  otherwise <c>-1</c> if error connect to DB or error query;
         *  <para><c>-2</c> if attributes is <typeparamref name="null"></typeparamref></para> 
         *  <para><c>-3</c> if <paramref name="values"/> is <typeparamref name="null"></typeparamref> 
         *  or his length less than <c>length</c> of <paramref name="attributes"/></para>
         * </returns>
         **/
        public int updateTable(string tableName, string[] attributes, string[] values, string[] where = null)
        {
            int result = -1;
            var conn = this.connect();

            if (conn == null) return -1;

            try
            {
                string str = $"UPDATE {tableName}";

                if (attributes == null) return -2;
                if (values == null) return -3;
                if (values.Length != attributes.Length) return -3;

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (i == 0)
                    {
                        str = $" {str} SET {attributes[i]} = {values[i]}";
                    } else 
                    {
                        str = $"{str}, {attributes[i]} = {values[i]}"; 
                    } 
                }

                if (where != null)
                {
                    str = $"{str} WHERE";
                    for (int i = 0; i < where.Length; i++)
                    {
                        str = $"{str} {where[i]}";

                        if (i < where.Length - 1)
                        {
                            str = $"{str} AND ";
                        }
                    }
                }
                str = $"{str};";

                _logger.LogInformation("Eexecute Update command: {str}", str);

                var command = new NpgsqlCommand(str, conn);
                result = command.ExecuteNonQuery();

                _logger.LogInformation("Result Update Command = {result}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error Update Table[{tableName}] : {e}", tableName, e.Message);
                result = -1;
            }
            return result;
        }
        /**
         * <returns>
         *  Return count deleted rows in table <paramref name="tableName"/> if good;
         *  <para>otherwise <c>-1</c> if error connect to DB</para>
         *  <c>-2</c> if error query
         * </returns>
         */
        public int deleteTable(string tableName, string[] where = null)
        {
            int result = -1;
            var conn = this.connect();

            if (conn == null) return -1;

            try
            {
                string str = $"DELETE FROM {tableName}";

                if (where != null)
                {
                    str = $"{str} WHERE";
                    for (int i = 0; i < where.Length; i++)
                    {
                        str = $"{str} {where[i]}";

                        if (i < where.Length - 1) str = $"{str} AND";
                    }
                }
                str = $"{str};";

                _logger.LogInformation("Execute Delete command: {str}", str);

                var command = new NpgsqlCommand(str, conn);
                result = command.ExecuteNonQuery();

                _logger.LogInformation("Result Delete Command = {result}", result);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error Delete Table[{tableName}] : {e}", tableName, e.Message);
                result = -2;
            }
            return result;
        }

        #region USER methods
        public bool insertUser(long userId, long grpId = Group.nullGrpId)
        {
            string[] attr = (grpId == Group.nullGrpId) ? new string[] { "ID" }
                : new string[] { "ID", "GRP_ID" };
            string[] values = (grpId == Group.nullGrpId) ? new string[] { $"{userId}" }
                : new string[] { $"{userId}", $"{grpId}" };
            return insertToTable("STUDENTS", attr, values);
        }
        public bool insertUser(User user)
        {
            return insertUser(user.Id, user.GrpID);
        }

        public User readUser(long userId)
        {
            NpgsqlDataReader reader = this.readFromTable("STUDENTS", null, new string[] { $"ID = {userId}" });

            if (reader == null) return null;

            if (!reader.Read()) return null;

            if (reader.IsDBNull(0)) return null;

            long grpId = (reader.IsDBNull(1)) ? Group.nullGrpId : reader.GetInt64(1);
            User user = new User(reader.GetInt64(0), grpId);
            reader.Close();

            return user;
        }

        public int updateUser(long userId, long grpId = Group.nullGrpId)
        {
            string[] attr = { "GRP_ID" };
            string[] values = { $"{grpId}" };
            string[] where = { $"ID = {userId}" };
            return updateTable("STUDENTS", attr, values, where);
        }
        public int updateUser(User user) { return updateUser(user.Id, user.GrpID); }

        public int deleteUser(long userId)
        {
            string[] where = { $"ID = {userId}" };
            return deleteTable("STUDENTS", where);
        }
        public int deleteUser(User user) { return (user == null) ? 0 : deleteUser(user.Id); }
        #endregion

        #region GROUP methods
        public bool insertGroup(string grpName)
        {
            string[] attr = { "NAME" };
            string[] val = { $"\'{grpName.ToUpper()}\'" };
            return insertToTable("GROUPS", attr, val);
        }
        public bool insertGroup(string id, string grpName)
        {
            string[] attr = { "ID", "NAME" };
            string[] val = { id, $"\'{grpName.ToUpper()}\'" };
            return insertToTable("GROUPS", attr, val);
        }
        public bool insertGroup(long id, string grpName) { return insertGroup(id.ToString(), grpName); }
        public bool insertGroup(Group grp) { return insertGroup(grp.Id, grp.GrpName); }
        public bool insertPoolGroups(Group[] groups)
        {
            if (groups == null) return false;

            string[] attr = { "ID", "NAME" };
            string strPool = getPoolValuesGroups(groups);
            return insertToTable("GROUPS", attr, null, strPool);
        }
        public bool insertPoolGroups(List<Group> list)
        {
            if (list == null) return false;

            return insertPoolGroups(list.ToArray());
        }

        /**
         * <summary>
         *  For get number of Groups from DB.
         *  For example: 
         *  <code> SELECT count(*) FROM GROUPS WHERE <paramref name="where"/></code>
         * </summary>
         * <returns>
         *  Return Number of Group if good; 
         *  <para>otherwise <c>-1</c> if error connection to DB</para>
         * </returns> 
         */
        public int readCountGroups(string[] where = null)
        {
            string[] attr = { "count(*)" };
            NpgsqlDataReader reader = readFromTable("GROUPS", attr, where);

            if (reader == null) return -1;
            reader.Read();
            int result = reader.GetInt32(0);
            reader.Close();
            return result;
        }
        /**
         * <summary>
         *  Read data from table <c>GROUPS</c>
         * </summary>
         * <returns>
         *  return all groups from table <c>GROUPS</c> if good; 
         *  <para>otherwise <c>null</c> if error or table don't have rows</para>
         * </returns>
         */
        public Group[] readAllGroups(string[] where = null)
        {
            int countGrps = readCountGroups(where);

            if (countGrps <= 0) return null;

            string[] attr = null;
            NpgsqlDataReader reader = readFromTable("GROUPS", attr, where);

            if (reader == null) return null;
            Group[] grps = new Group[countGrps];

            for (int i = 0; reader.Read(); i++)
            {
                grps[i] = new Group(reader.GetInt64(0), reader.GetString(1));
            }
            reader.Close();
            return grps;
        }
        public Group[] readGroups(long id)
        {
            string[] where = { $"ID = {id}" };
            return readAllGroups(where);
        }
        public Group readGroup(long grpId) { return readGroups(grpId)?[0]; }
        public Group[] readGroups(string nameGrp)
        {
            string[] where = { $"GRP_NAME = \'{nameGrp}\'" };
            return readAllGroups(where);
        }
        public Group[] getGroupsByShortName(string shortNameGrp)
        {
            string[] where = { $"NAME LIKE '{shortNameGrp}%'" };
            Group[] grps = readAllGroups(where);
            if (grps == null)
            {
                where = new string[] { $"NAME LIKE '{shortNameGrp.ToUpper()}%'" };
            }
            grps = readAllGroups(where);

            if (grps == null)
            {
                where = new string[] { $"NAME LIKE '{shortNameGrp.ToLower()}%'" };
            }
            return readAllGroups(where);
        }

        public string getPoolValuesGroups(Group[] grps)
        {
            if (grps == null) return null;

            StringBuilder strSb = new StringBuilder();
            for (int i = 0; i < grps.Length; i++)
            {
                if (i < grps.Length - 1)
                {
                    strSb.Append($"({grps[i].Id}, \'{grps[i].GrpName}\'),");
                }
                else
                {
                    strSb.Append($"({grps[i].Id}, \'{grps[i].GrpName}\');");
                }
            }
            return strSb.ToString();
        }
        public string getPoolValuesGroups(List<Group> list)
        {
            if (list == null) return null;
            return getPoolValuesGroups(list.ToArray());
        }

        public int updateGroup(long grpId, string grpName)
        {
            string[] attr = { "NAME" };
            string[] value = { $"\'{grpName}\'" };
            string[] where = { $"ID = {grpId}" };
            return updateTable("GROUPS", attr, value, where);
        }
        public int updateGroup(Group grp) { return grp == null ? -3 : updateGroup(grp.Id, grp.GrpName); }

        public int deleteGroup(long groupId)
        {
            string[] where = { $"ID = {groupId}" };
            return deleteTable("GROUPS");
        }
        public int deleteGroup(Group grp) { return (grp == null) ? 0 : deleteGroup(grp.Id); }
        #endregion

        #region STAROSTA methods
        public bool insertStarosta(User user)
        {
            if (user == null || user.GrpID == Group.nullGrpId) return false;
            string[] attr = { "STD_ID", "GRP_ID" };
            string[] val = { $"{user.Id}", $"{user.GrpID}" };
            return insertToTable("STAROSTA", attr, val);
        }
        public bool isStarosta(long userId) 
        {
            string[] attr = { "COUNT(*)" };
            string[] where = { $"STD_ID = {userId}" };
            NpgsqlDataReader reader = readFromTable("STAROSTA", attr, where);

            if (reader == null) return false;
            reader.Read();

            if (reader.GetInt32(0) == 1) return true;
            return false;
        }
        public bool isStarosta(User user) 
        {
            if (user == null) return false;
            return isStarosta(user.Id);
        }
        public int readCountStarosta(long grpId)
        {
            if (grpId == Group.nullGrpId) return -1;

            string[] attr = { "count(*)" };
            string[] where = { $"GRP_ID = {grpId}" };
            NpgsqlDataReader reader = readFromTable("STAROSTA", attr, where);

            if (reader == null) return -1;
            reader.Read();
            int result = reader.GetInt32(0);
            return result;
        }
        #endregion

        #region PAIRLINKS methods
        public bool InsertPairLink(PairLink pairLink) 
        {
            if (pairLink == null) return false;

            string[] attr = {"DSC_ID", "GRP_ID", "DSC_TYPE", "LINK", "LINK_ID", "PASS"};
            string[] val = { 
                $"{pairLink.DSC_ID}", $"{pairLink.GRP_ID}", $"{(int)pairLink.DSC_TYPE}",
                $"\'{pairLink.Link}\'", $"\'{pairLink.LinkId}\'", $"\'{pairLink.Pass}\'"
            };
            return insertToTable("PAIRLINKS", attr, val);
        }
        public bool InsertOrUpdatePairLink(PairLink pairLink) 
        {
            if (pairLink == null) return false;

            if (readCountPairLink(pairLink.DSC_ID, pairLink.GRP_ID, pairLink.DSC_TYPE) <= 0) 
            {
                return InsertPairLink(pairLink);
            } 
            else 
            {
                return updatePairLink(pairLink);
            }
        }
        public bool updatePairLink(PairLink pairLink) 
        {
            if (pairLink == null) return false;
            string[] attr = { "LINK", "LINK_ID", "PASS"};
            string[] val = { $"\'{pairLink.Link}\'", $"\'{pairLink.LinkId}\'", $"\'{pairLink.Pass}\'"};
            string[] where = { $"DSC_ID = {pairLink.DSC_ID}", $"GRP_ID = {pairLink.GRP_ID}", $"DSC_TYPE = {(int)pairLink.DSC_TYPE}" };
            return updateTable("PAIRLINKS", attr, val, where) > 0;
        }
        public int readCountPairLink(long dscId, long grpId, PairLink.DSC_TYPES dscType) 
        {
            string[] attr = { "COUNT(*)" };
            string[] where = { $"DSC_ID = {dscId}", $"GRP_ID = {grpId}", $"DSC_TYPE = {(int)dscType}" };

            NpgsqlDataReader reader = readFromTable("PAIRLINKS", attr, where);

            if (reader == null) return -1;
            reader.Read();
            int count = reader.GetInt32(0);
            reader.Close();
            return count;
        }
        public PairLink readParLink(long dscId, long grpId, PairLink.DSC_TYPES dscType) 
        {
            if (readCountPairLink(dscId, grpId, dscType) <= 0) return null;

            string[] attr = { "LINK", "LINK_ID", "PASS"};
            string[] where = { $"DSC_ID = {dscId}", $"GRP_ID = {grpId}", $"DSC_TYPE = {(int)dscType}" };

            NpgsqlDataReader reader = readFromTable("PAIRLINKS", attr, where);

            if (reader == null) return null;

            reader.Read();
            PairLink pairLink = new PairLink(dscId, grpId, dscType,
                reader.GetString(0), reader.GetString(1), reader.GetString(2)
            );
            reader.Close();
            return pairLink;
        }
        #endregion

        #region GROUPSCHATS methods
        public bool InsertGroupChat(long chatId, long grpId) 
        {
            string[] attr = { "ID", "GRP_ID" };
            string[] val = { $"{chatId}", $"{grpId}" };
            return insertToTable("GROUPCHATS", attr, val);
        }
        public bool isExistGroupChat(long chatId) 
        {
            string[] attr = { "COUNT(*)" };
            string[] where = { $"ID = {chatId}"};

            NpgsqlDataReader reader = readFromTable("GROUPCHATS", attr, where);

            if (reader == null) return false;
            reader.Read();
            bool result = reader.GetInt32(0) == 1;
            reader.Close();
            return result;
        }
        
        public long getGrpIdOfGroupChat(long chatId) 
        {
            if (!isExistGroupChat(chatId)) return -1;
            string[] attr = { "GRP_ID" };
            string[] where = { $"ID = {chatId}" };
            NpgsqlDataReader reader = readFromTable("GROUPCHATS", attr, where);

            if (reader == null) return -1;
            reader.Read();
            long grpId = reader.GetInt64(0);
            reader.Close();
            return grpId;
        }
        #endregion

        #region ALTPAIR methods
        public bool InsertAltPair(AltPair altPair)
        {
            if (altPair == null) return false;
            string[] attr = {"DATE_REG", "NUM_PAIR", "GRP_ID", "INFO"};
            string[] val = { 
                $"\'{altPair.DATE_REG}\'", $"{AltPair.NumberPairToIntBasedOne(altPair.NUM_PAIR)}", 
                $"{altPair.GRP_ID}", $"\'{altPair.INFO}\'" 
            };
            return insertToTable("ALTPAIR", attr, val);
        }
        public bool InsertOrUpdateAltPair(AltPair altPair) 
        {
            if (altPair == null) return false;

            if (readCountAltPair(altPair.DATE_REG, altPair.NUM_PAIR, altPair.GRP_ID) <= 0) 
            {
                return InsertAltPair(altPair);
            } 
            else 
            {
                return updateAltPair(altPair);
            }
        }
        public int readCountAltPair(string dateReg, AltPair.NUMBER_PAIR numP, long grpId) 
        {
            string[] attr = { "COUNT(*)" };
            string[] where = {
                $"DATE_REG = \'{dateReg}\'", $"NUM_PAIR = {AltPair.NumberPairToIntBasedOne(numP)}",
                $"GRP_ID = {grpId}"
            };

            NpgsqlDataReader reader = readFromTable("ALTPAIR", attr, where);

            if (reader == null) return -1;
            reader.Read();
            int count = reader.GetInt32(0);
            reader.Close();
            return count;
        }
        public AltPair readAltPair(string dateReg, AltPair.NUMBER_PAIR num, long grpId) 
        {
            if (readCountAltPair(dateReg, num, grpId) < 1) return null;
            AltPair altPair = new AltPair(dateReg, num, grpId);
            string[] attr = { "INFO" };
            string[] where = {
                $"DATE_REG = \'{dateReg}\'",
                $"NUM_PAIR = {AltPair.NumberPairToIntBasedOne(num)}",
                $"GRP_ID = {grpId}"
            };

            NpgsqlDataReader reader = readFromTable("ALTPAIR", attr, where);

            if (reader == null) return null;
            reader.Read();
            altPair.INFO = reader.GetString(0);
            reader.Close();
            return altPair;
        }
        public AltPair readAltPair(Pair pair) 
        {
            if (pair == null
                || readCountAltPair(Pair.getNormalDate(pair.DateReg), AltPair.ToNumberPair(pair.NumPair), pair.WorkLink.GRP_ID) < 1
                )
            {
                return null; 
            }
            AltPair altPair = new AltPair(pair.DateReg, AltPair.ToNumberPair(pair.NumPair), pair.WorkLink.GRP_ID);
            string[] attr = { "INFO" };
            string[] where = { 
                $"DATE_REG = \'{pair.DateReg}\'", 
                $"NUM_PAIR = {AltPair.NumberPairToIntBasedOne(AltPair.ToNumberPair(pair.NumPair))}", 
                $"GRP_ID = {pair.WorkLink.GRP_ID}"
            };

            NpgsqlDataReader reader = readFromTable("ALTPAIR", attr, where);

            if (reader == null) return null;
            reader.Read();
            altPair.INFO = reader.GetString(0);
            reader.Close();
            return altPair;
        }
        public List<AltPair> readAltPairs(string dateReg, long grpId) 
        {
            string[] attr = { "NUM_PAIR", "INFO" };
            string[] where = { $"DATE_REG = \'{dateReg}\'", $"GRP_ID = {grpId}"};

            NpgsqlDataReader reader = readFromTable("ALTPAIR", attr, where);

            if (reader == null) return null;
            List<AltPair> list = new List<AltPair>();
            for (; reader.Read();)
            {
                list.Add(new AltPair(dateReg, AltPair.ToNumberPair(reader.GetInt32(0)), grpId, reader.GetString(1)));
            }
            reader.Close();
            return list;
        }
        public bool updateAltPair(AltPair altPair) 
        {
            if (altPair == null) return false;
            string[] attr = { "DATE_REG", "NUM_PAIR", "GRP_ID", "INFO" };
            string[] val = { 
                $"\'{altPair.DATE_REG}\'", $"{AltPair.NumberPairToIntBasedOne(altPair.NUM_PAIR)}", 
                $"{altPair.GRP_ID}", $"\'{altPair.INFO}\'"
            };
            string[] where = { 
                $"DATE_REG = \'{altPair.DATE_REG}\'", $"NUM_PAIR = {AltPair.NumberPairToIntBasedOne(altPair.NUM_PAIR)}", 
                $"GRP_ID = {altPair.GRP_ID}"
            };
            return updateTable("ALTPAIR", attr, val, where) > 0;
        }
        #endregion
    }
}
