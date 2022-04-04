using DoAn_ShopGiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_ShopGiay.Controllers
{
    public class GiohangController : Controller
    {
        dbQLBanGiayDataContext data = new dbQLBanGiayDataContext();
        // GET: Giohang
        // lay gio hang
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                // Nếu gio hàng chưa tồn tại thì khởi tạo listGiohang
                lstGiohang = new List<Giohang>();

                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        //Cập nhập phương thức ThemGiohang
        public ActionResult ThemGiohang(int iMaGIAY, string strURL, int maSize)
        {
            // lấy ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            // kiểm tra sách này tồn tại trong Session["Giohang"] chưa ?
            Giohang sanpham = lstGiohang.Find(n => n.iMaGIAY == iMaGIAY);

            if (sanpham == null)
            {
                sanpham = new Giohang(iMaGIAY,maSize);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //Phương thức tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstgiohangs = Session["GioHang"] as List<Giohang>;
            if (lstgiohangs != null)
            {
                iTongSoLuong = lstgiohangs.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        //Phương thức Tính Tổng tiền 
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();

            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "ShopGiay");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            // Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach da co trong Session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaGIAY == iMaSP);
            //Neu ton tai thi cho sua so luong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaGIAY == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "ShopGiay");
            }
            return RedirectToAction("GioHang");
        }
        // cap nhap gio hang
        public ActionResult CapnhatGiohang(int iMaSP, int iMaS, FormCollection f)
        {
            int slnhap = 0;
            // Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach da co trong Session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaGIAY == iMaSP);
            Giohang sanphamsize = lstGiohang.SingleOrDefault(n => n.iMaSize == iMaS);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
                slnhap = sanpham.iSoluong;
            }
     
            var querysoluongton = from ctgiay in data.CHITIETGIAYs
                                  join sizegiay in data.SIZEGIAYs on ctgiay.MaSize equals sizegiay.MaSize
                                  where ctgiay.MaSize == iMaS
                                  && ctgiay.MaGIAY == iMaSP
                                  select ctgiay;

            List<CHITIETGIAY> listChiTietGiay = querysoluongton.ToList<CHITIETGIAY>();

            int slton = 0;
            int txtsl = 0;
            if (listChiTietGiay.Count > 0)
            {
                slton = listChiTietGiay[0].SoLuongTon;
            }

            if (slton < slnhap)
            {
                TempData["Tag"] = "Số lượng giày không đủ !!!";
                sanpham.iSoluong = slton;
            }
            
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "ShopGiay");
        }
        //Hien thi View Dat hang de cap nhat cac thong tin cho don hang
        [HttpGet]
        public ActionResult Dathang()
        {
            //kiem tra dang nhap
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "ShopGiay");
            }

            // Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult Dathang(CHITIETGIAY ctgiay, FormCollection collection)
        {
            //Them Don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them Chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaGIAY = item.iMaGIAY;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                ctdh.Tongtien = item.dThanhtien;
                ctdh.MaSize = item.iMaSize;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
            }
            int sl = 0;
            foreach (var item in gh)
            {
                var queryctgiay = from ctgiay1 in data.CHITIETGIAYs
                                  where ctgiay1.MaGIAY == item.iMaGIAY
                                  && ctgiay1.MaSize == item.iMaSize
                                  select ctgiay1;

                List<CHITIETGIAY> listctgiay = queryctgiay.ToList<CHITIETGIAY>();
                if (listctgiay.Count > 0)
                {
                    if (listctgiay[0].MaGIAY == item.iMaGIAY && listctgiay[0].MaSize == item.iMaSize)
                    {
                        listctgiay[0].SoLuongTon = listctgiay[0].SoLuongTon - item.iSoluong;
                        ctgiay.SoLuongTon = listctgiay[0].SoLuongTon;
                        if (ctgiay.MaGIAY == item.iMaGIAY && ctgiay.MaGIAY == item.iMaSize)
                        {
                            UpdateModel(ctgiay);
                        }    
                    }    
                }    
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}