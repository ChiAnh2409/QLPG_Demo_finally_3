using QLPG.Models;
using QLPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLPG.Controllers
{
    public class ThanhVienController: Controller
    {
        private QLPG1Entities db = new QLPG1Entities(); 
        public ActionResult ThanhVien() 
        {
            
            var list = new MultipleData();
            list.vien = db.ThanhVien.ToList();
            return View(list);
        }
        public ActionResult ThemDK()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemDK(ThanhVien tv)
        {
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                tv.NgayTao = now;
                db.ThanhVien.Add(tv);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công!";
            }

            // Trả lại cùng view ThemDK (tức là view hiện tại)
            return View("ThemDK");
        }

        public ActionResult ThemTV() 
        { 
            return View(); 
        }
        [HttpPost]
        public ActionResult ThemTV(ThanhVien tv)
        {
            DateTime now = DateTime.Now; 
            tv.NgayTao = now;
            db.ThanhVien.Add(tv);
            db.SaveChanges();
            return RedirectToAction("ThanhVien");
        }
        public ActionResult SuaTV(int id)
        {
            ThanhVien tv = db.ThanhVien.Find(id);
            return View(tv);
        }
        [HttpPost]
        public ActionResult SuaTV(ThanhVien tv)
        {
            //DateTime now = DateTime.Now;
            //tv.NgayTao = now;
            db.Entry(tv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ThanhVien");
        }
        [HttpPost]
        public ActionResult XoaTV(int id)
        {
            ThanhVien tv = db.ThanhVien.Find(id);
            db.ThanhVien.Remove(tv);
            db.SaveChanges();
            return RedirectToAction("ThanhVien");
        }
        [HttpPost]
        public ActionResult TimKiemTV(string search)
        {
            var list = new MultipleData();

            // Perform case-insensitive search based on TenTV or other properties as needed
            list.vien = db.ThanhVien
                           .Where(tv => tv.TenTV.ToLower().Contains(search.ToLower()))
                           .ToList();

            return View("ThanhVien", list);
        }

    }
}