// ===== ADMIN.JS - Logic quản trị =====
const nd = JSON.parse(sessionStorage.getItem('nguoiDung') || '{}');
if (!nd.id && !nd.Id) window.location.href = 'login.html';

const vaiTro = nd.vaiTro || nd.VaiTro || '';
if (vaiTro === 'KhachHang') window.location.href = 'order.html';

document.getElementById('userNameDisplay').textContent = nd.hoTen || nd.HoTen || 'Admin';
document.getElementById('avatarChar').textContent = (nd.hoTen || nd.HoTen || 'A')[0].toUpperCase();
document.getElementById('userRoleDisplay').textContent = vaiTro;

// Ẩn menu users nếu không phải Admin
if (vaiTro !== 'Admin') document.getElementById('nav-users').style.display = 'none';

let dsSanPham = [], dsDonHang = [], dsNguoiDung = [], currentLoaiFilter = '', currentOrderFilter = '';

// ===== NAVIGATION =====
function showPage(page, el) {
  document.querySelectorAll('.page-section').forEach(p => p.style.display = 'none');
  document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
  document.getElementById('page-' + page).style.display = 'flex';
  document.getElementById('page-' + page).style.flexDirection = 'column';
  if (el) el.classList.add('active');
  const loaders = { dashboard: loadDashboard, orders: loadOrders, menu: loadMenu, users: loadUsers };
  if (loaders[page]) loaders[page]();
}

// ===== DASHBOARD =====
function loadDashboard() {
  sendMsg('layDonHang', { filter: '' });
  sendMsg('laySanPham', {});
}

function renderDashboard(orders, sanPham) {
  const choXacNhan = orders.filter(o => (o.trangThai || o.TrangThai) === 'Chờ xác nhận').length;
  const dangPhaChE = orders.filter(o => (o.trangThai || o.TrangThai) === 'Đang pha chế').length;
  const hoanthanh = orders.filter(o => (o.trangThai || o.TrangThai) === 'Hoàn thành').length;
  const tongThu = orders.filter(o => (o.trangThai || o.TrangThai) === 'Hoàn thành')
    .reduce((s, o) => s + Number(o.tongTien || o.TongTien || 0), 0);

  document.getElementById('statGrid').innerHTML = `
    <div class="stat-card"><div class="stat-icon" style="background:rgba(249,115,22,0.15);">📋</div>
      <div><div class="stat-value">${orders.length}</div><div class="stat-label">Tổng đơn hàng</div></div></div>
    <div class="stat-card"><div class="stat-icon" style="background:rgba(245,158,11,0.15);">⏳</div>
      <div><div class="stat-value">${choXacNhan}</div><div class="stat-label">Chờ xác nhận</div></div></div>
    <div class="stat-card"><div class="stat-icon" style="background:rgba(56,189,248,0.15);">🧪</div>
      <div><div class="stat-value">${dangPhaChE}</div><div class="stat-label">Đang pha chế</div></div></div>
    <div class="stat-card"><div class="stat-icon" style="background:rgba(34,197,94,0.15);">💰</div>
      <div><div class="stat-value">${fmt(tongThu)}</div><div class="stat-label">Doanh thu hoàn thành</div></div></div>`;

  // Bảng đơn hàng mới nhất
  const recent = orders.slice(0, 10);
  document.getElementById('recentOrdersBody').innerHTML = recent.length
    ? recent.map(dh => rowDonHang(dh, true)).join('')
    : '<tr><td colspan="6" style="text-align:center;color:var(--clr-text-muted);">Chưa có đơn hàng</td></tr>';
}

// ===== ĐƠN HÀNG =====
function loadOrders() { sendMsg('layDonHang', { filter: currentOrderFilter }); }

function filterOrders(filter, btn) {
  currentOrderFilter = filter;
  document.querySelectorAll('#page-orders .tab-btn').forEach(b => b.classList.remove('active'));
  if (btn) btn.classList.add('active');
  sendMsg('layDonHang', { filter });
}

function renderOrders(orders) {
  const tbody = document.getElementById('ordersTableBody');
  tbody.innerHTML = orders.length
    ? orders.map(dh => rowDonHang(dh, false)).join('')
    : '<tr><td colspan="7" style="text-align:center;color:var(--clr-text-muted);">Không có đơn hàng</td></tr>';
}

function rowDonHang(dh, isShort) {
  const id = dh.id || dh.Id;
  const tt = dh.trangThai || dh.TrangThai;
  const cols = isShort ? `
    <td><strong>#${id}</strong></td>
    <td>${dh.tenKhachHang || dh.TenKhachHang || 'N/A'}</td>
    <td>${dh.thoiGianDat || dh.ThoiGianDat}</td>
    <td style="color:var(--clr-primary);font-weight:700;">${fmt(dh.tongTien || dh.TongTien)}</td>
    <td>${badgeTrangThai(tt)}</td>
    <td><button class="btn btn-secondary btn-sm" onclick="xemChiTiet(${id})">🔍 Xem</button></td>`
    : `<td><strong>#${id}</strong></td>
    <td>${dh.tenKhachHang || dh.TenKhachHang || 'N/A'}</td>
    <td>${dh.thoiGianDat || dh.ThoiGianDat}</td>
    <td style="color:var(--clr-primary);font-weight:700;">${fmt(dh.tongTien || dh.TongTien)}</td>
    <td>${badgeTrangThai(tt)}</td>
    <td style="color:var(--clr-text-muted);">${dh.ghiChu || dh.GhiChu || '—'}</td>
    <td style="display:flex;gap:6px;flex-wrap:wrap;">
      <button class="btn btn-secondary btn-sm" onclick="xemChiTiet(${id})">🔍 Xem</button>
      ${nextStatusBtn(id, tt)}
    </td>`;
  return `<tr>${cols}</tr>`;
}

function nextStatusBtn(id, tt) {
  const nextMap = { 'Chờ xác nhận': ['Đang pha chế', '🔵 Xác nhận'], 'Đang pha chế': ['Hoàn thành', '✅ Hoàn thành'] };
  if (nextMap[tt]) {
    const [next, label] = nextMap[tt];
    return `<button class="btn btn-success btn-sm" onclick="capNhatTrangThai(${id},'${next}')">${label}</button>`;
  }
  if (tt === 'Chờ xác nhận') return `<button class="btn btn-danger btn-sm" onclick="capNhatTrangThai(${id},'Đã hủy')">❌ Hủy</button>`;
  return '';
}

function capNhatTrangThai(id, trangThai) {
  sendMsg('capNhatTrangThai', { id, trangThai });
}

function xemChiTiet(donHangId) {
  const dh = dsDonHang.find(d => (d.id || d.Id) === donHangId);
  if (!dh) return;
  document.getElementById('chiTietTitle').textContent = `📋 Chi Tiết Đơn #${donHangId}`;
  // Hiển thị thông tin đơn hàng (Master)
  const tt = dh.trangThai || dh.TrangThai;
  document.getElementById('chiTietBody').innerHTML = `
    <div style="background:var(--clr-surface2);border-radius:var(--radius);padding:16px;margin-bottom:16px;">
      <div style="display:grid;grid-template-columns:1fr 1fr;gap:8px;font-size:0.88rem;">
        <div><span style="color:var(--clr-text-muted);">Khách hàng:</span> <strong>${dh.tenKhachHang||dh.TenKhachHang||'N/A'}</strong></div>
        <div><span style="color:var(--clr-text-muted);">Thời gian:</span> <strong>${dh.thoiGianDat||dh.ThoiGianDat}</strong></div>
        <div><span style="color:var(--clr-text-muted);">Trạng thái:</span> ${badgeTrangThai(tt)}</div>
        <div><span style="color:var(--clr-text-muted);">Ghi chú:</span> ${dh.ghiChu||dh.GhiChu||'—'}</div>
      </div>
    </div>
    <h4 style="margin-bottom:12px;font-size:0.9rem;color:var(--clr-text-muted);">CHI TIẾT MÓN ĂN</h4>
    <div style="background:var(--clr-surface2);border-radius:var(--radius);padding:12px;font-size:0.85rem;color:var(--clr-text-muted);text-align:center;">
      <em>Đang tải chi tiết...</em>
    </div>
    <div style="display:flex;justify-content:space-between;align-items:center;margin-top:16px;padding-top:12px;border-top:1px solid var(--clr-border);">
      <span style="color:var(--clr-text-muted);">Tổng cộng:</span>
      <span style="font-size:1.2rem;font-weight:800;color:var(--clr-primary);">${fmt(dh.tongTien||dh.TongTien)}</span>
    </div>`;

  const footerBtns = [];
  const nextMap = { 'Chờ xác nhận': ['Đang pha chế', '🔵 Xác nhận đơn'], 'Đang pha chế': ['Hoàn thành', '✅ Hoàn thành'] };
  if (nextMap[tt]) {
    const [next, label] = nextMap[tt];
    footerBtns.push(`<button class="btn btn-primary" onclick="capNhatTrangThai(${donHangId},'${next}');closeModal('chiTietModal');loadOrders();">${label}</button>`);
  }
  if (tt === 'Chờ xác nhận') {
    footerBtns.push(`<button class="btn btn-danger" onclick="capNhatTrangThai(${donHangId},'Đã hủy');closeModal('chiTietModal');loadOrders();">❌ Hủy đơn</button>`);
  }
  document.getElementById('chiTietFooter').innerHTML = `<button class="btn btn-secondary" onclick="closeModal('chiTietModal')">Đóng</button>${footerBtns.join('')}`;
  openModal('chiTietModal');
}

// ===== MENU SẢN PHẨM =====
function loadMenu() { sendMsg('laySanPham', {}); }

function renderMenu(items) {
  dsSanPham = items;
  filterSanPham();
}

function filterLoai(loai, btn) {
  currentLoaiFilter = loai;
  document.querySelectorAll('#page-menu .tab-btn').forEach(b => b.classList.remove('active'));
  if (btn) btn.classList.add('active');
  filterSanPham();
}

function filterSanPham() {
  const query = (document.getElementById('searchSP')?.value || '').toLowerCase();
  const filtered = dsSanPham.filter(sp => {
    const ten = (sp.tenSanPham || sp.TenSanPham || '').toLowerCase();
    const loai = sp.loai || sp.Loai || '';
    return (!currentLoaiFilter || loai === currentLoaiFilter) && (!query || ten.includes(query));
  });
  const tbody = document.getElementById('menuTableBody');
  tbody.innerHTML = filtered.length ? filtered.map(sp => {
    const id = sp.id || sp.Id;
    const loai = sp.loai || sp.Loai;
    const dangBan = sp.dangBan !== undefined ? sp.dangBan : sp.DangBan;
    return `<tr>
      <td>${id}</td>
      <td><strong>${sp.tenSanPham || sp.TenSanPham}</strong></td>
      <td style="color:var(--clr-primary);font-weight:700;">${fmt(sp.giaCoBan || sp.GiaCoBan)}</td>
      <td><span class="badge badge-${loai.toLowerCase()}">${loai}</span></td>
      <td><span class="badge badge-${dangBan ? 'success' : 'danger'}">${dangBan ? 'Còn hàng' : 'Hết hàng'}</span></td>
      <td style="display:flex;gap:6px;">
        <button class="btn btn-secondary btn-sm" onclick="editSanPham(${id})">✏️ Sửa</button>
        <button class="btn btn-danger btn-sm" onclick="xoaSanPham(${id})">🗑️ Xóa</button>
      </td></tr>`;
  }).join('') : '<tr><td colspan="6" style="text-align:center;color:var(--clr-text-muted);">Không có sản phẩm</td></tr>';
}

function openSanPhamModal(sp) {
  document.getElementById('spModalTitle').textContent = sp ? '✏️ Sửa Sản Phẩm' : '➕ Thêm Sản Phẩm';
  document.getElementById('spId').value = sp ? (sp.id || sp.Id) : '';
  document.getElementById('spTen').value = sp ? (sp.tenSanPham || sp.TenSanPham) : '';
  document.getElementById('spGia').value = sp ? (sp.giaCoBan || sp.GiaCoBan) : '';
  document.getElementById('spLoai').value = sp ? (sp.loai || sp.Loai) : 'TraSua';
  document.getElementById('spDangBan').value = sp ? String(sp.dangBan !== undefined ? sp.dangBan : sp.DangBan) : 'true';
  openModal('sanPhamModal');
}

function editSanPham(id) {
  const sp = dsSanPham.find(s => (s.id || s.Id) === id);
  if (sp) openSanPhamModal(sp);
}

function luuSanPham() {
  const id = document.getElementById('spId').value;
  const data = {
    id: id ? parseInt(id) : 0,
    tenSanPham: document.getElementById('spTen').value.trim(),
    giaCoBan: parseFloat(document.getElementById('spGia').value),
    loai: document.getElementById('spLoai').value,
    hinhAnh: '',
    dangBan: document.getElementById('spDangBan').value === 'true'
  };
  if (!data.tenSanPham || isNaN(data.giaCoBan)) { toast('error', '❌ Vui lòng nhập đầy đủ thông tin!'); return; }
  sendMsg(id ? 'suaSanPham' : 'themSanPham', data);
  closeModal('sanPhamModal');
}

function xoaSanPham(id) {
  if (!confirm('Bạn có chắc muốn xóa sản phẩm này?')) return;
  sendMsg('xoaSanPham', { id });
}

// ===== NGƯỜI DÙNG =====
function loadUsers() { sendMsg('layNguoiDung', {}); }

function renderUsers(items) {
  dsNguoiDung = items;
  const tbody = document.getElementById('usersTableBody');
  const vaiTroMap = { Admin: 'danger', NhanVien: 'info', KhachHang: 'success' };
  tbody.innerHTML = items.map(u => {
    const id = u.id || u.Id;
    const vt = u.vaiTro || u.VaiTro;
    return `<tr>
      <td>${id}</td>
      <td><code style="color:var(--clr-info);">${u.tenDangNhap || u.TenDangNhap}</code></td>
      <td>${u.hoTen || u.HoTen}</td>
      <td><span class="badge badge-${vaiTroMap[vt]||'info'}">${vt}</span></td>
      <td style="display:flex;gap:6px;">
        <button class="btn btn-secondary btn-sm" onclick="editUser(${id})">✏️ Sửa</button>
        <button class="btn btn-danger btn-sm" onclick="xoaUser(${id})">🗑️ Xóa</button>
      </td></tr>`;
  }).join('');
}

function openUserModal(u) {
  document.getElementById('userModalTitle').textContent = u ? '✏️ Sửa Tài Khoản' : '➕ Thêm Tài Khoản';
  document.getElementById('userId').value = u ? (u.id || u.Id) : '';
  document.getElementById('userHoTen').value = u ? (u.hoTen || u.HoTen) : '';
  document.getElementById('userTenDN').value = u ? (u.tenDangNhap || u.TenDangNhap) : '';
  document.getElementById('userMatKhau').value = '';
  document.getElementById('userVaiTro').value = u ? (u.vaiTro || u.VaiTro) : 'KhachHang';
  openModal('userModal');
}

function editUser(id) {
  const u = dsNguoiDung.find(u => (u.id || u.Id) === id);
  if (u) openUserModal(u);
}

function luuNguoiDung() {
  const id = document.getElementById('userId').value;
  const data = {
    id: id ? parseInt(id) : 0,
    hoTen: document.getElementById('userHoTen').value.trim(),
    tenDangNhap: document.getElementById('userTenDN').value.trim(),
    matKhau: document.getElementById('userMatKhau').value,
    vaiTro: document.getElementById('userVaiTro').value
  };
  if (!data.hoTen || !data.tenDangNhap || !data.matKhau) { toast('error', '❌ Vui lòng nhập đầy đủ!'); return; }
  sendMsg(id ? 'suaNguoiDung' : 'themNguoiDung', data);
  closeModal('userModal');
}

function xoaUser(id) {
  if (!confirm('Xóa tài khoản này?')) return;
  sendMsg('xoaNguoiDung', { id });
}

// ===== UTILS =====
function fmt(n) { return Number(n).toLocaleString('vi-VN') + 'đ'; }
function badgeTrangThai(t) {
  const m = { 'Chờ xác nhận': 'warning', 'Đang pha chế': 'info', 'Hoàn thành': 'success', 'Đã hủy': 'danger' };
  return `<span class="badge badge-${m[t]||'info'}">${t}</span>`;
}
function openModal(id) { document.getElementById(id).classList.add('show'); }
function closeModal(id) { document.getElementById(id).classList.remove('show'); }
function sendMsg(action, data) { window.chrome.webview.postMessage(JSON.stringify({ action, data: data || {} })); }
function toast(type, msg) {
  const div = document.createElement('div');
  div.className = `toast ${type}`;
  div.innerHTML = msg;
  document.getElementById('toastContainer').appendChild(div);
  setTimeout(() => div.remove(), 3500);
}
function dangXuat() { sendMsg('dangXuat', {}); sessionStorage.clear(); window.location.href = 'login.html'; }

// ===== WEBVIEW HANDLER =====
window.chrome.webview.addEventListener('message', function(e) {
  try {
    const res = JSON.parse(e.data);
    if (!res.ok && res.loi) { toast('error', '❌ ' + res.loi); return; }
    switch (res.action) {
      case 'laySanPham':
        if (res.ok) {
          if (document.getElementById('page-dashboard').style.display !== 'none') renderDashboard(dsDonHang, res.data);
          renderMenu(res.data);
        }
        break;
      case 'layDonHang':
        if (res.ok) {
          dsDonHang = res.data;
          if (document.getElementById('page-dashboard').style.display !== 'none') { loadDashboard(); }
          else renderOrders(res.data);
        }
        break;
      case 'layNguoiDung': if (res.ok) renderUsers(res.data); break;
      case 'themSanPham': case 'suaSanPham': case 'xoaSanPham':
        toast(res.ok ? 'success' : 'error', res.ok ? '✅ Thao tác thành công!' : '❌ Thao tác thất bại!');
        if (res.ok) loadMenu();
        break;
      case 'themNguoiDung': case 'suaNguoiDung': case 'xoaNguoiDung':
        toast(res.ok ? 'success' : 'error', res.ok ? '✅ Thao tác thành công!' : '❌ Thao tác thất bại!');
        if (res.ok) loadUsers();
        break;
      case 'capNhatTrangThai':
        toast(res.ok ? 'success' : 'error', res.ok ? '✅ Cập nhật trạng thái thành công!' : '❌ Thất bại!');
        if (res.ok) loadOrders();
        break;
    }
  } catch(err) { console.error(err); }
});

// ===== INIT =====
// Dashboard mặc định: load cả đơn hàng và sản phẩm
sendMsg('layDonHang', { filter: '' });
sendMsg('laySanPham', {});
