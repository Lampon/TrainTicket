using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using TrainTicket.Common;
using TrainTicket.Models;

namespace TrainTicket.Controllers
{
    public class HomeController : Controller
    {
        DbContext db = new DbContext();
        /// <summary>
        /// 添加区域
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public IActionResult AddArea(Area area)
        {
            db.SugarDatabase.Insertable(area).ExecuteCommand();
            return Json(new BaseResult { Code = MsgCode.Success, Msg = "添加成功" }, new Newtonsoft.Json.JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <returns></returns>
        public IActionResult GetArea()
        {
            var list = db.SugarDatabase.Queryable<Area>().ToList();
            return Json(new BaseResult { Code = MsgCode.Success, Data = list }, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 添加窗口
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public IActionResult AddWindow(Window window)
        {
            db.SugarDatabase.Insertable(window).ExecuteCommand();
            return Json(new BaseResult { Code = MsgCode.Success, Msg = "添加成功" }, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 获取所有区域下的窗口包含名称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllWindow(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                date = DateTime.Now.ToString("yyy-MM-dd");
            }
            var Arealist = db.SugarDatabase.Queryable<Area>().ToList();
            List<AreaBody> list = new List<AreaBody>();
            foreach (var item in Arealist)
            {
                //var windowBody = db.SugarDatabase.Queryable<Window, DailyTicketRecord>((wd, dtr) => new object[]
                //{
                //    JoinType.Left, wd.Id == dtr.WindowId
                //}).Where((wd, dtr) => wd.AreaId == item.Id && dtr.Date == date).Select((wd, dtr) => new WindowBody
                //{
                //    Id = wd.Id,
                //    Name = wd.Name,
                //    AreaId = wd.AreaId,
                //    Input = dtr.Input,
                //    Output = dtr.Output,
                //    Remain = dtr.Remain
                //}).ToList();
                List<WindowBody> listModel = new List<WindowBody>();
                var listWindowBody = db.SugarDatabase.Queryable<Window>().Where(a => a.AreaId == item.Id).ToList();
                foreach (var window in listWindowBody)
                {
                    WindowBody wbBody=new WindowBody();
                    wbBody.Name = window.Name;
                    wbBody.Id = window.Id;
                    wbBody.AreaId = window.AreaId;
                    wbBody.Stock = window.Stock;
                    var record = db.SugarDatabase.Queryable<DailyTicketRecord>().First(a => a.WindowId == window.Id && a.Date == date);
                    if (record == null)
                    {
                        wbBody.Input = 0;
                        wbBody.Output =0;
                        wbBody.Remain = 0;
                    }
                    else
                    {
                        wbBody.Input = record.Input;
                        wbBody.Output = record.Output;
                        wbBody.Remain = record.Remain;
                    }
                    listModel.Add(wbBody);
                }
                //if (windowBody.Count == 0)//不存在记录
                //{
                //    windowBody = db.SugarDatabase.Queryable<Window>().Where(a => a.AreaId == item.Id).Select(a => new WindowBody
                //    {
                //        Id = a.Id,
                //        Name = a.Name,
                //        AreaId = a.AreaId,
                //        Input = 0,
                //        Output = 0,
                //        Remain = 0
                //    }).ToList();
                //}

                int areaRemain = 0;
                foreach (var wb in listModel)
                {
                    areaRemain += wb.Remain;
                }
                list.Add(new AreaBody()
                {
                    Id = item.Id,
                    Name = item.Name,
                    AreaRemain = areaRemain,
                    Windows = listModel
                });
            }
            return Json(new BaseResult { Code = MsgCode.Success, Data = list }, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 添加每日窗口进出库数量
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddDailyTicketRecord(string date, [FromBody]List<AreaBody> list)
        {
            foreach (var area in list)
            {
                foreach (var window in area.Windows)
                {
                    var model = db.SugarDatabase.Queryable<DailyTicketRecord>()
                        .First(a => a.Date == date && a.WindowId == window.Id);
                    if (model == null)
                    {
                        DailyTicketRecord dailyTicketRecord = new DailyTicketRecord();
                        dailyTicketRecord.AreaId = area.Id;
                        dailyTicketRecord.Date = date;
                        dailyTicketRecord.Input = window.Input;
                        dailyTicketRecord.Output = window.Output;
                        dailyTicketRecord.WindowId = window.Id;
                        var previousDayRemain = db.SugarDatabase.Queryable<DailyTicketRecord>().Where("Date<@Date and windowId=@WindowId", new { Date = date, WindowId = window.Id }).OrderBy(a => a.Date, OrderByType.Desc).First();
                        if (previousDayRemain == null)
                        {
                            var wModel = db.SugarDatabase.Queryable<Window>().Single(a => a.Id == window.Id);
                            dailyTicketRecord.Remain = wModel.Stock - window.Output + window.Input;
                        }
                        else
                        {
                            dailyTicketRecord.Remain = previousDayRemain.Remain - window.Output + window.Input;
                        }
                        db.SugarDatabase.Insertable(dailyTicketRecord).ExecuteCommand();
                    }
                    else
                    {
                        model.Input = window.Input;
                        model.Output = window.Output;
                        var previousDayRemain = db.SugarDatabase.Queryable<DailyTicketRecord>().Where("Date<@Date and windowId=@WindowId", new { Date = date, WindowId = window.Id }).OrderBy(a => a.Date, OrderByType.Desc).First();
                        if (previousDayRemain == null)
                        {
                            var wModel = db.SugarDatabase.Queryable<Window>().Single(a => a.Id == window.Id);
                            model.Remain = wModel.Stock - window.Output + window.Input;
                        }
                        else
                        {
                            model.Remain = previousDayRemain.Remain - window.Output + window.Input;
                        }
                        db.SugarDatabase.Updateable(model).ExecuteCommand();
                        ReCalculate(date, window.Id);

                    }
                }
            }
            return Json(new BaseResult { Code = MsgCode.Success, Msg = "添加成功" }, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 重新计算这个日子之后所有的记录
        /// </summary>
        /// <param name="date"></param>
        /// <param name="windowId"></param>
        public void ReCalculate(string date, int windowId)
        {
            var dailyTicketRecords = db.SugarDatabase.Queryable<DailyTicketRecord>().Where("Date>@Date and windowId=@WindowId", new { Date = date, WindowId = windowId }).OrderBy(a => a.Date).ToList();
            foreach (var item in dailyTicketRecords)
            {
                var previousDayRemain = db.SugarDatabase.Queryable<DailyTicketRecord>().Where("Date<@Date and windowId=@WindowId", new { Date = item.Date, WindowId = windowId }).OrderBy(a => a.Date, OrderByType.Desc).First();
                item.Remain = previousDayRemain.Remain - item.Output + item.Input;
                db.SugarDatabase.Updateable(item).ExecuteCommand();
            }
        }
        public IActionResult Index()
        {
            //DbContext db = new DbContext();
            //db.SugarDatabase.CodeFirst.InitTables(typeof(Area), typeof(Window), typeof(DailyTicketRecord));
            return View();
        }
    }

    public class AreaBody
    {
        public int Id { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 区域结存
        /// </summary>
        public int AreaRemain { get; set; }
        /// <summary>
        /// 区域下面所有窗口
        /// </summary>
        public List<WindowBody> Windows { get; set; }
    }
    public class WindowBody
    {
        public int Id { get; set; }
        /// <summary>
        /// 所属区域ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 窗口编号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 当前结存
        /// </summary>
        public int Remain { get; set; }
        /// <summary>
        /// 进库
        /// </summary>
        public int Input { get; set; }
        /// <summary>
        /// 出库
        /// </summary>
        public int Output { get; set; }
    }
}
