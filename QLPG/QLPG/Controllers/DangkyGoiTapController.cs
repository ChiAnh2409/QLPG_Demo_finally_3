﻿using QLPG.Models;
using QLPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLPG.Controllers
{
    public class DangkyGoiTapController : Controller
    {
        private QLPG1Entities db = new QLPG1Entities();
        //tạo biến database để lấy dữ liệu
        // GET: DangkyGoiTap
        public ActionResult DKGT()
        {
            var list = new MultipleData();
            list.chiTietDK_= db.ChiTietDK_GoiTap.Include("GoiTap");  //tham chiếu khóa ngoại 2 bảng gói tập và hội viên
            list.chiTietDK_ = db.ChiTietDK_GoiTap.Include("HoiVien");
            list.goiTap = db.GoiTap.ToList();
            list.vien = db.ThanhVien.ToList();  //hiển thị thông báo
            list.hoiViens = db.HoiVien.ToList();
            return View(list);
        }
        public ActionResult LichTap(string search)
        {
            var list = new MultipleData();

            // Retrieve the initial data
            list.chiTietDK_ = db.ChiTietDK_GoiTap.Include("GoiTap").Include("HoiVien").ToList();
            list.goiTap = db.GoiTap.ToList();
            list.vien = db.ThanhVien.ToList();
            list.hoiViens = db.HoiVien.ToList();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower().Trim();
                list.chiTietDK_ = list.chiTietDK_.Where(item =>
                    item.GoiTap.TenGoiTap.ToLower().Contains(search) ||
                    item.HoiVien.ThanhVien.TenTV.ToLower().Contains(search)
                ).ToList();
            }

            return View(list);
        }

        public ActionResult ThemDKGT()
        {
            var list = new MultipleData();
            list.chiTietDK_ = db.ChiTietDK_GoiTap.Include("GoiTap");
            list.chiTietDK_ = db.ChiTietDK_GoiTap.Include("HoiVien");
            list.goiTap = db.GoiTap.ToList();
            list.hoiViens = db.HoiVien.ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult ThemDKGT(ChiTietDK_GoiTap dkgt)
        {
            db.ChiTietDK_GoiTap.Add(dkgt);
            db.SaveChanges();
            return RedirectToAction("DKGT");
        }
        public ActionResult SuaDKGT(int id)
        {
            var viewmodel = new MultipleData();
            viewmodel.chiTietDK_ = db.ChiTietDK_GoiTap.Where(dkgt => dkgt.id_CTDKGoiTap == id).ToList();
            viewmodel.goiTap = db.GoiTap.ToList();
            viewmodel.hoiViens = db.HoiVien.ToList();
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult SuaDKGT(ChiTietDK_GoiTap dkgt)
        {
            var existingDangkyGoiTap = db.ChiTietDK_GoiTap.FirstOrDefault(item => item.id_CTDKGoiTap == dkgt.id_CTDKGoiTap);
            if (existingDangkyGoiTap != null)
            {
                existingDangkyGoiTap.id_GT = dkgt.id_GT;
                existingDangkyGoiTap.id_HV = dkgt.id_HV;
                existingDangkyGoiTap.NgayBatDau = dkgt.NgayBatDau;
                existingDangkyGoiTap.NgayKetThuc= dkgt.NgayKetThuc;
                existingDangkyGoiTap.ThanhTien = dkgt.ThanhTien;

                db.SaveChanges();
            }

            return RedirectToAction("DKGT");
        }
        public ActionResult XoaDKGT(int id)
        {
            var DangkyGoiTap = db.ChiTietDK_GoiTap.Find(id);
            if (DangkyGoiTap != null)
            {
                db.ChiTietDK_GoiTap.Remove(DangkyGoiTap);
                db.SaveChanges();

            }
            return RedirectToAction("DKGT");
        }
        [HttpPost]
        public ActionResult TimKiemDKGT(string search)
        {
            var list = new MultipleData();

            // Tìm kiếm theo tên thành viên trong bảng hội viên
            var hoiViensResults = db.HoiVien.Where(hv => hv.ThanhVien.TenTV.Contains(search)).ToList();
            var hoiVienIds = hoiViensResults.Select(hv => hv.id_HV).ToList();

            // Lấy danh sách ChiTietDK_GoiTap dựa trên các kết quả tìm kiếm
            list.chiTietDK_ = db.ChiTietDK_GoiTap
                .Include("HoiVien")
                .Include("GoiTap")
                .Where(dkgt => hoiVienIds.Contains(dkgt.HoiVien.id_HV))
                .ToList();

            list.goiTap = db.GoiTap.ToList();
            list.hoiViens = db.HoiVien.ToList();
            list.vien = db.ThanhVien.ToList();  //hiển thị thông báo
            ViewBag.Search = search; // Đặt tên cần tìm kiếm vào ViewBag để hiển thị trong view
            return View("DKGT", list);
        }
        //gia hạn gói tập cho hội viên
        [HttpGet]
        public ActionResult GiaHanGoiTap(int id_HV)
        {
            var hoiVien = db.HoiVien.Find(id_HV);

            if (hoiVien != null && hoiVien.TinhTrang == false)
            {
                var expiredSubscription = db.ChiTietDK_GoiTap
                    .Where(ct => ct.id_HV == id_HV && ct.NgayKetThuc < DateTime.Now)
                    .OrderByDescending(ct => ct.NgayKetThuc)
                    .FirstOrDefault();

                if (expiredSubscription != null)
                {
                    var list = new MultipleData
                    {
                        hoiViens = new List<HoiVien> { hoiVien },
                        chiTietDK_ = new List<ChiTietDK_GoiTap> { expiredSubscription }
                    };

                    return View(list);
                }
            }

            return RedirectToAction("DKGT", "DangkyGoiTap");
        }

        [HttpPost]
        public ActionResult GiaHanGoiTap(int id_HV, int id_GT, DateTime NgayBatDau, DateTime NgayKetThuc, decimal ThanhTien)
        {
            var hoiVien = db.HoiVien.Find(id_HV);

            if (hoiVien != null && hoiVien.TinhTrang == false)
            {
                var expiredSubscription = db.ChiTietDK_GoiTap
                    .Where(ct => ct.id_HV == id_HV && ct.NgayKetThuc < DateTime.Now)
                    .OrderByDescending(ct => ct.NgayKetThuc)
                    .FirstOrDefault();

                if (expiredSubscription != null)
                {
                    var newSubscription = new ChiTietDK_GoiTap
                    {
                        id_GT = id_GT,
                        id_HV = id_HV,
                        NgayBatDau = NgayBatDau,
                        NgayKetThuc = NgayKetThuc,
                        ThanhTien = ThanhTien
                    };

                    db.ChiTietDK_GoiTap.Add(newSubscription);
                    db.SaveChanges();

                    hoiVien.TinhTrang = true;
                    db.Entry(hoiVien).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("DKGT", "DangkyGoiTap"); // Đổi thành trang mà bạn muốn chuyển hướng sau khi gia hạn thành công.
                }
            }

            return RedirectToAction("DKGT", "DangkyGoiTap");
        }
    }
}