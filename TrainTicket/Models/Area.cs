using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace TrainTicket.Models
{
    public class Area 
    {
        public Area()
        {
            AddTime = DateTime.Now;
        }
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [SugarColumn(Length = 250, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime AddTime { get; set; }

    }
}
