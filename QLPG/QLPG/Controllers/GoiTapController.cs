using QLPG.Models;
using QLPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLPG.Controllers
{
    public class GoiTapController : Controller
    {
        private QLPG1Entities db = new QLPG1Entities();
        //tạo biến database để lấy dữ liệu
        // GET: GoiTap
        
        public ActionResult GoiTap()
        {
            var list = new MultipleData();
            list.vien= db.ThanhVien.ToList(); //hiển thị thông báo
            list.goiTap = db.GoiTap.ToList();
            return View(list);
        }
        public ActionResult ThemGT() 
        {
                return View();
        }
        [HttpPost]
        public ActionResult ThemGT(GoiTap gt) 
        {
            db.GoiTap.Add(gt); 
            db.SaveChanges();
            return RedirectToAction("GoiTap");
        }
        public ActionResult SuaGT(int id)
        {
            GoiTap gt = db.GoiTap.Find(id);
            return RedirectToAction("GoiTap"); 
        }
        [HttpPost]
        public ActionResult SuaGT(GoiTap gt)
        {
            db.Entry(gt).State = System.Data.Entity.EntityState.Modified; 
            db.SaveChanges();
            return RedirectToAction("GoiTap");
        }

        [HttpPost]
        public ActionResult XoaGT(int id)
        {
                 GoiTap gt = db.GoiTap.Find(id);
                 db.GoiTap.Remove(gt);
                 db.SaveChanges();
                 return RedirectToAction("GoiTap");
        }

    }
}