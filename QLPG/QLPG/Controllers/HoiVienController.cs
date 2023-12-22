using QLPG.Models;
using QLPG.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QLPG.Controllers
{
    public class HoiVienController : Controller
    {
        private QLPG1Entities db = new QLPG1Entities();
        //tạo biến database để lấy dữ liệu
        // GET: HoiVien
        public ActionResult HoiVien()
        {
            var list = new MultipleData();
            list.hoiViens = db.HoiVien.Include("ThanhVien").ToList();
            //tình trạng gói tập của hội viên
            foreach (var hoiVien in list.hoiViens)
            {
                var goiTap = db.ChiTietDK_GoiTap
                                .Where(ct => ct.id_HV == hoiVien.id_HV)
                                .OrderByDescending(ct => ct.NgayKetThuc)
                                .FirstOrDefault();

                if (goiTap != null && goiTap.NgayKetThuc >= DateTime.Now)
                {
                    hoiVien.TinhTrang = true;
                }
                else
                {
                    hoiVien.TinhTrang = false;
                }

                // Cập nhật trạng thái của hội viên trong cơ sở dữ liệu
                db.Entry(hoiVien).State = EntityState.Modified;
            }

            // Lưu các thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            list.vien = db.ThanhVien.ToList();
            return View(list);
        }
        public ActionResult ThemHV() 
        {
            // Lấy danh sách thành viên chưa là hội viên
            var chuaLaHoiVienIds = db.HoiVien.Select(hv => hv.id_TV).ToList();
            var chuaLaHoiVien = db.ThanhVien.Where(tv => !chuaLaHoiVienIds.Contains(tv.id_TV)).ToList();

            var list = new MultipleData
            {
                hoiViens = db.HoiVien.Include("ThanhVien"),
                vien = chuaLaHoiVien
            };
            return View(list);
        }
        [HttpPost]
        public ActionResult ThemHV(HoiVien hv)
        {
            String HinhAnh = "";

            HttpPostedFileBase file = Request.Files["HinhAnh"];
            if (file != null && file.FileName != "")
            {
                String serverPath = HttpContext.Server.MapPath("~/assets/img/team");
                String filePath = serverPath + "/" + file.FileName;
                file.SaveAs(filePath);
                HinhAnh = file.FileName;
            }
            hv.HinhAnh = HinhAnh;
            db.HoiVien.Add(hv);
            hv.TinhTrang = true;
            DateTime now = DateTime.Now;
            hv.NgayGiaNhap = now;
            db.SaveChanges();
            return RedirectToAction("HoiVien");
        }
        public ActionResult SuaHV(int id)
        {
            var viewmodel = new MultipleData();
            viewmodel.hoiViens = db.HoiVien.Where(hv => hv.id_HV == id).ToList();
            viewmodel.vien = db.ThanhVien.ToList();
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult SuaHV(HoiVien hv)
        {
            HoiVien existingHoiVien = db.HoiVien.Find(hv.id_HV);
            if (existingHoiVien != null)
            {
                existingHoiVien.id_TV = hv.id_TV;
                existingHoiVien.NgaySinh = hv.NgaySinh;
                existingHoiVien.CCCD = hv.CCCD;
                existingHoiVien.TinhTrang = hv.TinhTrang;

                // Kiểm tra và lưu hình ảnh nếu có
                HttpPostedFileBase file = Request.Files["HinhAnh"];
                if (file != null && file.FileName != "")
                {
                    String HinhAnh = file.FileName;
                    String serverPath = HttpContext.Server.MapPath("~/assets/img/team");
                    String filePath = serverPath + "/" + HinhAnh;
                    file.SaveAs(filePath);
                    existingHoiVien.HinhAnh = HinhAnh;
                }

                db.SaveChanges();
            }

            return RedirectToAction("HoiVien");
        }
        public ActionResult XoaHV(int id)
        {
            var HoiVien = db.HoiVien.Find(id);
            if (HoiVien != null)
            {
                db.HoiVien.Remove(HoiVien);
                db.SaveChanges();

            }
            return RedirectToAction("HoiVien");
        }
        [HttpPost] //Tìm kiếm bằng tên trong bảng hội viên nhưng tham chiếu bằng id_TV trong bảng thành viên
        public ActionResult TimKiemHV(string search)
        {
            var list = new MultipleData();
            list.hoiViens = db.HoiVien.Include("ThanhVien").Where(hv => hv.ThanhVien.TenTV.Contains(search)).ToList();
            list.vien = db.ThanhVien.ToList();

            return View("HoiVien", list);
        }

        // Điểm danh cho hội viên
        [HttpGet]
        public ActionResult DiemDanhHV(int id_HV)
        {
            var hoiVien = db.HoiVien.Include("ThanhVien").FirstOrDefault(hv => hv.id_HV == id_HV);

            if (hoiVien != null)
            {
                // Retrieve success message from TempData
                ViewBag.DiemDanhSuccess = TempData["DiemDanhSuccess"] as string;

                return View(hoiVien);
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DiemDanhHV(int id_HV, bool DaDiemDanh)
        {
            var hoiVien = db.HoiVien.Find(id_HV);

            if (hoiVien != null)
            {
                // Get the current date without the time component
                DateTime currentDate = DateTime.Now.Date;

                // Check if the member has already attended on the same day
                bool hasAttended = db.BuoiTap.Any(bt => bt.id_HV == id_HV && bt.NgayThamGia.HasValue && DbFunctions.TruncateTime(bt.NgayThamGia.Value) == currentDate);

                if (hasAttended)
                {
                    // Set error message in TempData
                    TempData["DiemDanhError"] = "Hội viên đã được điểm danh trong ngày hôm nay.";
                }
                else
                {
                    // If the member has not attended on the same day, add a new attendance record
                    var buoiTap = new BuoiTap
                    {
                        id_HV = id_HV,
                        DaDiemDanh = DaDiemDanh,
                        NgayThamGia = DateTime.Now // Sử dụng thời gian hiện tại
                    };

                    db.BuoiTap.Add(buoiTap);
                    db.SaveChanges();

                    // Set success message in TempData
                    TempData["DiemDanhSuccess"] = "Điểm danh thành công!";
                }

                // Redirect back to the "DiemDanhHV" action
                return RedirectToAction("DiemDanhHV", new { id_HV = id_HV });
            }

            return HttpNotFound();
        }

        // Xem chi tiết điểm danh
        public ActionResult CTDiemDanh(int id_HV, DateTime ngay)
        {
            var list = new MultipleData();
            list.hoiViens = db.HoiVien.Include("ThanhVien").ToList();
            list.vien = db.ThanhVien.ToList();
            list.buoiTap = db.BuoiTap.Where(bt => bt.id_HV == id_HV).ToList();

            // Set the ViewBag for id_HV
            ViewBag.id_HV = id_HV;

            return View(list);
        }

    }
}