using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace TrainTicket.Models
{
    public class DailyTicketRecord
    {
        public DailyTicketRecord()
        {
            AddTime = DateTime.Now;
        }
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 记录日期
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 20)]
        public string Date { get; set; }
        /// <summary>
        /// 对应窗口ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int WindowId { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int AreaId { get; set; }
        /// <summary>
        /// 进库
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Input { get; set; }
        /// <summary>
        /// 出库
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Output { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime AddTime { get; set; }
    }
}
