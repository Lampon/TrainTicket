using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace TrainTicket.Models
{
    /// <summary>
    /// 窗口
    /// </summary>
    public class Window
    {

        public Window()
        {
            AddTime = DateTime.Now;
        }
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 所属区域ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int AreaId { get; set; }
        /// <summary>
        /// 窗口编号
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 250)]
        public string Name { get; set; }
        /// <summary>
        /// 初始库存
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Stock { get; set; }
        /// <summary>
        /// 当前结存
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Remain { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime AddTime { get; set; }
    }
}
