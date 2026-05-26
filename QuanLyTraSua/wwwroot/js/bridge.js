// ===== BRIDGE.JS - Hỗ trợ chạy ứng dụng cả trên trình duyệt và WebView2 =====
if (!window.chrome || !window.chrome.webview) {
    console.log("⚠️ Phát hiện chạy trên trình duyệt web. Kích hoạt Mock Backend (localStorage).");
    window.chrome = window.chrome || {};
    window.chrome.webview = (function() {
        const listeners = [];
        
        function initMockDb() {
            if (!localStorage.getItem('mock_db_initialized')) {
                const users = [
                    { Id: 1, TenDangNhap: 'admin', MatKhau: 'admin123', HoTen: 'Quản Trị Viên', VaiTro: 'Admin' },
                    { Id: 2, TenDangNhap: 'nhanvien1', MatKhau: 'nv123456', HoTen: 'Nguyễn Văn A', VaiTro: 'NhanVien' },
                    { Id: 3, TenDangNhap: 'khachhang1', MatKhau: 'kh123456', HoTen: 'Trần Thị B', VaiTro: 'KhachHang' }
                ];
                const products = [
                    { Id: 1, TenSanPham: 'Trà Sữa Truyền Thống', GiaCoBan: 35000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 2, TenSanPham: 'Trà Sữa Matcha', GiaCoBan: 40000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 3, TenSanPham: 'Trà Sữa Khoai Môn', GiaCoBan: 38000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 4, TenSanPham: 'Trà Đào Cam Sả', GiaCoBan: 32000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 5, TenSanPham: 'Trà Sữa Oolong', GiaCoBan: 42000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 6, TenSanPham: 'Trà Sữa Socola', GiaCoBan: 38000, Loai: 'TraSua', HinhAnh: '', DangBan: true },
                    { Id: 7, TenSanPham: 'Thạch Dừa', GiaCoBan: 8000, Loai: 'Topping', HinhAnh: '', DangBan: true },
                    { Id: 8, TenSanPham: 'Trân Châu Đen', GiaCoBan: 8000, Loai: 'Topping', HinhAnh: '', DangBan: true },
                    { Id: 9, TenSanPham: 'Pudding Trứng', GiaCoBan: 10000, Loai: 'Topping', HinhAnh: '', DangBan: true },
                    { Id: 10, TenSanPham: 'Thạch Cà Phê', GiaCoBan: 8000, Loai: 'Topping', HinhAnh: '', DangBan: true }
                ];
                localStorage.setItem('mock_users', JSON.stringify(users));
                localStorage.setItem('mock_products', JSON.stringify(products));
                localStorage.setItem('mock_orders', JSON.stringify([]));
                localStorage.setItem('mock_db_initialized', 'true');
            }
        }
        initMockDb();

        function handleMockMessage(msg) {
            const users = JSON.parse(localStorage.getItem('mock_users') || '[]');
            const products = JSON.parse(localStorage.getItem('mock_products') || '[]');
            const orders = JSON.parse(localStorage.getItem('mock_orders') || '[]');
            
            let response = { ok: false, action: msg.action };
            
            switch (msg.action) {
                case 'dangNhap':
                    const u = users.find(x => x.TenDangNhap === msg.data.tenDangNhap && x.MatKhau === msg.data.matKhau);
                    if (u) {
                        response.ok = true;
                        response.nguoiDung = u;
                    } else {
                        response.loi = 'Tên đăng nhập hoặc mật khẩu sai!';
                    }
                    break;
                case 'dangXuat':
                    response.ok = true;
                    break;
                case 'laySanPham':
                    response.ok = true;
                    response.data = msg.data.loai 
                        ? products.filter(p => p.Loai === msg.data.loai)
                        : products;
                    break;
                case 'themSanPham':
                    const newSp = {
                        Id: products.length ? Math.max(...products.map(p => p.Id)) + 1 : 1,
                        TenSanPham: msg.data.tenSanPham,
                        GiaCoBan: msg.data.giaCoBan,
                        Loai: msg.data.loai,
                        HinhAnh: msg.data.hinhAnh || '',
                        DangBan: msg.data.dangBan
                    };
                    products.push(newSp);
                    localStorage.setItem('mock_products', JSON.stringify(products));
                    response.ok = true;
                    break;
                case 'suaSanPham':
                    const spIdx = products.findIndex(p => p.Id === msg.data.id);
                    if (spIdx !== -1) {
                        products[spIdx] = { 
                            Id: msg.data.id,
                            TenSanPham: msg.data.tenSanPham !== undefined ? msg.data.tenSanPham : products[spIdx].TenSanPham,
                            GiaCoBan: msg.data.giaCoBan !== undefined ? msg.data.giaCoBan : products[spIdx].GiaCoBan,
                            Loai: msg.data.loai !== undefined ? msg.data.loai : products[spIdx].Loai,
                            HinhAnh: msg.data.hinhAnh !== undefined ? msg.data.hinhAnh : products[spIdx].HinhAnh,
                            DangBan: msg.data.dangBan !== undefined ? msg.data.dangBan : products[spIdx].DangBan
                        };
                        localStorage.setItem('mock_products', JSON.stringify(products));
                        response.ok = true;
                    }
                    break;
                case 'xoaSanPham':
                    const newProducts = products.filter(p => p.Id !== msg.data.id);
                    localStorage.setItem('mock_products', JSON.stringify(newProducts));
                    response.ok = true;
                    break;
                case 'datMon':
                    const newOrder = {
                        Id: orders.length ? Math.max(...orders.map(o => o.Id)) + 1 : 1,
                        KhachHangId: JSON.parse(sessionStorage.getItem('nguoiDung') || '{}').Id || 3,
                        ThoiGianDat: new Date().toLocaleString('vi-VN'),
                        TongTien: msg.data.items.reduce((sum, item) => {
                            const p = products.find(prod => prod.Id === item.sanPhamId);
                            if (!p) return sum;
                            let itemPrice = p.GiaCoBan;
                            if (item.tuyChonThem && item.tuyChonThem.includes('Size L')) {
                                itemPrice += 5000;
                            }
                            return sum + (itemPrice * item.soLuong);
                        }, 0),
                        TrangThai: 'Chờ xác nhận',
                        GhiChu: msg.data.ghiChu || '',
                        TenKhachHang: JSON.parse(sessionStorage.getItem('nguoiDung') || '{}').HoTen || 'Khách hàng'
                    };
                    orders.unshift(newOrder);
                    localStorage.setItem('mock_orders', JSON.stringify(orders));
                    response.ok = true;
                    response.donHangId = newOrder.Id;
                    break;
                case 'layDonHang':
                    response.ok = true;
                    const currentUser = JSON.parse(sessionStorage.getItem('nguoiDung') || '{}');
                    if (msg.data.filter === 'cuaToi') {
                        response.data = orders.filter(o => o.KhachHangId === currentUser.Id);
                    } else if (msg.data.filter) {
                        response.data = orders.filter(o => o.TrangThai === msg.data.filter);
                    } else {
                        response.data = orders;
                    }
                    break;
                case 'capNhatTrangThai':
                    const oIdx = orders.findIndex(o => o.Id === msg.data.id);
                    if (oIdx !== -1) {
                        orders[oIdx].TrangThai = msg.data.trangThai;
                        localStorage.setItem('mock_orders', JSON.stringify(orders));
                        response.ok = true;
                    }
                    break;
                case 'layNguoiDung':
                    response.ok = true;
                    response.data = users;
                    break;
                case 'themNguoiDung':
                    const newU = {
                        Id: users.length ? Math.max(...users.map(u => u.Id)) + 1 : 1,
                        TenDangNhap: msg.data.tenDangNhap,
                        MatKhau: msg.data.matKhau,
                        HoTen: msg.data.hoTen,
                        VaiTro: msg.data.vaiTro
                    };
                    users.push(newU);
                    localStorage.setItem('mock_users', JSON.stringify(users));
                    response.ok = true;
                    break;
                case 'suaNguoiDung':
                    const uIdx = users.findIndex(u => u.Id === msg.data.id);
                    if (uIdx !== -1) {
                        users[uIdx] = { 
                            ...users[uIdx], 
                            ...msg.data, 
                            TenDangNhap: msg.data.tenDangNhap || users[uIdx].TenDangNhap, 
                            MatKhau: msg.data.matKhau || users[uIdx].MatKhau, 
                            HoTen: msg.data.hoTen || users[uIdx].HoTen 
                        };
                        localStorage.setItem('mock_users', JSON.stringify(users));
                        response.ok = true;
                    }
                    break;
                case 'xoaNguoiDung':
                    const newUsers = users.filter(u => u.Id !== msg.data.id);
                    localStorage.setItem('mock_users', JSON.stringify(newUsers));
                    response.ok = true;
                    break;
            }

            setTimeout(() => {
                listeners.forEach(cb => {
                    cb({ data: JSON.stringify(response) });
                });
            }, 50);
        }

        return {
            postMessage: function(msgStr) {
                try {
                    const msg = JSON.parse(msgStr);
                    handleMockMessage(msg);
                } catch(e) { console.error(e); }
            },
            addEventListener: function(type, cb) {
                if (type === 'message') {
                    listeners.push(cb);
                }
            },
            removeEventListener: function(type, cb) {
                if (type === 'message') {
                    const idx = listeners.indexOf(cb);
                    if (idx !== -1) listeners.splice(idx, 1);
                }
            }
        };
    })();
}
