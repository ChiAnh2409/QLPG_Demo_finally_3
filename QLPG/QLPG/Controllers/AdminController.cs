using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLPG.Models;
using QLPG.ViewModel;

namespace QLPG.Controllers
{
    public class AdminController : Controller
    {
        private QLPG1Entities db = new QLPG1Entities();
        public ActionResult Index()
        {
            MultipleData data = new MultipleData();

            // Lấy toàn bộ dữ liệu
            data.hoiViens = db.HoiVien.Include("ThanhVien").ToList();
            data.vien = db.ThanhVien.ToList();
            data.chiTietDK_ = db.ChiTietDK_GoiTap.ToList();

            // Tính số lượng hội viên mới
            DateTime today = DateTime.Now.Date;
            int newMembersCount = data.hoiViens.Count(hv => hv.NgayGiaNhap.HasValue && hv.NgayGiaNhap.Value.Date == today);

            // Truyền số lượng hội viên mới cho view
            ViewBag.NewMembersCount = newMembersCount;

            return View(data);
        }

        //public ActionResult LoadBarChart(int? selectedYear)
        //{
        //    var mymodel = new MultipleData();

        //    // Filter ChiTietDK_GoiTap based on the selected year
        //    var chiTietDKs = db.ChiTietDK_GoiTap
        //        .Where(ct => !selectedYear.HasValue || (ct.NgayBatDau != null && ct.NgayBatDau.Value.Year == selectedYear))
        //        .ToList();

        //    // Group ChiTietDK_GoiTap by month and calculate the rounded sum of ThanhTien for each month
        //    var monthlyRevenue = chiTietDKs
        //        .GroupBy(ct => ct.NgayBatDau != null ? ct.NgayBatDau.Value.Month : (int?)null) // Use null-conditional operator here as well
        //        .Select(group => new
        //        {
        //            Month = group.Key,
        //            TotalRevenue = Math.Round((group.Sum(ct => ct.ThanhTien) ?? 0), 3) // Rounded to 3 decimal places
        //        })
        //        .OrderBy(entry => entry.Month) // Keep ascending order of months
        //        .ToList();

        //    return Json(monthlyRevenue, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult LoadBarChart(int? selectedYear)
        //{
        //    var mymodel = new MultipleData();

        //    // Filter ThanhToans based on the selected year
        //    var invoices = db.ChiTietDK_GoiTap
        //        .Where(t => selectedYear == null || t.NgayBatDau.Value.Year == selectedYear)
        //        .ToList();

        //    // Group invoices by month and calculate the rounded sum of ThanhTien for each month
        //    var monthlyRevenue = invoices
        //        .GroupBy(t => t.NgayBatDau?.Month)
        //        .Select(group => new
        //        {
        //            Month = group.Key,
        //            TotalRevenue = Math.Round((double)(group.Sum(t => t.ThanhTien) ?? 0), 3) // Rounded to 3 decimal places
        //        })
        //        .OrderBy(entry => entry.Month) // Keep ascending order of months
        //        .ToList();

        //    return Json(monthlyRevenue, JsonRequestBehavior.AllowGet);
        //}
    }
}
