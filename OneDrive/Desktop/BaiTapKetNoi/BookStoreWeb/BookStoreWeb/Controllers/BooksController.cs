using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStoreWeb.Models;

namespace BookStoreWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Gọi điều khiển trung tâm DbContext vào đây để làm việc với SQL Server
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. CHỨC NĂNG XEM: Lấy danh sách sách từ SQL Server ra hiển thị trên Web
        public async Task<IActionResult> Index()
        {
            // Include(b => b.Category) giúp lấy luôn tên danh mục của cuốn sách đó
            var books = await _context.Books.Include(b => b.Category).ToListAsync();
            return View(books);
        }

        // 2. CHỨC NĂNG MỞ FORM: Hiển thị giao diện để người dùng nhập liệu
        public IActionResult Create()
        {
            // Lấy danh sách Categories để đổ vào ô Dropdown (thẻ chọn) ngoài giao diện
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // 3. CHỨC NĂNG LƯU: Nhận dữ liệu khi người dùng ấn nút "Lưu" và TỰ ĐỘNG BẮN SANG SQL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Price,Stock,Description,CategoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Bước A: Thêm sách vào bộ nhớ theo dõi của .NET
                _context.Add(book);

                // Bước B: LỆNH THẦN THÁNH - Ép hệ thống dịch thành lệnh INSERT INTO và lưu thẳng vào SQL Server
                await _context.SaveChangesAsync();

                // Lưu thành công thì tự động chuyển hướng về trang danh sách sách (Index)
                return RedirectToAction(nameof(Index));
            }

            // Nếu nhập liệu lỗi, load lại form và giữ nguyên dữ liệu đã nhập để người dùng sửa
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // 4. CHỨC NĂNG XÓA: Nhận ID của cuốn sách và xóa thẳng dưới SQL Server
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm cuốn sách trong SQL dựa theo ID truyền lên từ giao diện web
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                // Ra lệnh xóa cuốn sách khỏi bộ theo dõi dữ liệu
                _context.Books.Remove(book);

                // Ép hệ thống dịch thành lệnh DELETE FROM và thực thi xuống SQL Server
                await _context.SaveChangesAsync();
            }

            // Xóa xong tự động load lại trang kho sách (Index) để cập nhật danh sách mới
            return RedirectToAction(nameof(Index));
        }
    }
}