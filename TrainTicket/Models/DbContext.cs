using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace TrainTicket.Models
{
    public class DbContext
    {
        public SqlSugarClient SugarDatabase { get { return GetInstance(); } }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = "Data Source=data.db3",//主数据库
                    DbType = SqlSugar.DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    IsShardSameThread = true /*Shard Same Thread*/,
                    InitKeyType = InitKeyType.Attribute
                    /*从数据库*/
                    //SlaveConnectionConfigs = new List<SlaveConnectionConfig>() {
                    //    new SlaveConnectionConfig()
                    //    {
                    //        HitRate=10, ConnectionString="server=.;database=zhundao_qiandao;uid=sa;pwd=xcl19920619"
                    //    },
                    //   /new SlaveConnectionConfig() { HitRate=30, ConnectionString=Config.ConnectionString3 }
                    //}
                });
            return db;
        }

    }
}
