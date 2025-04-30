import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UtilitiesService {
  generateSlug(name: string): string {
    if (!name) return '';

    // Loại bỏ dấu và thay thế khoảng trắng bằng dấu gạch ngang
    return name
      .toLowerCase() // Chuyển đổi thành chữ thường
      .normalize('NFD') // Tách ký tự có dấu thành ký tự không dấu và dấu
      .replace(/[\u0300-\u036f]/g, '') // Loại bỏ các ký tự dấu
      .replace(/[^\w\s-]/g, '') // Loại bỏ các ký tự đặc biệt
      .replace(/\s+/g, '-') // Thay thế khoảng trắng bằng dấu gạch ngang
      .replace(/-+/g, '-'); // Loại bỏ nhiều dấu gạch ngang liên tiếp
  }

  /**
   * Định dạng số tiền theo chuẩn VNĐ
   * @param amount Số tiền cần định dạng
   * @returns Chuỗi đã được định dạng (ví dụ: 69,000 VNĐ)
   */
  formatVND(amount: number | string): string {
    // Chuyển đổi thành number nếu đầu vào là string
    const num = typeof amount === 'string' ? parseFloat(amount) : amount;

    // Kiểm tra nếu không phải là số hợp lệ
    if (isNaN(num)) {
      return '0 VNĐ';
    }

    // Làm tròn số nếu là float
    const roundedNum = Math.round(num);

    // Định dạng số với dấu phân cách hàng nghìn
    const formatted = roundedNum
      .toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    return `${formatted} VNĐ`;
  }

  downloadFile(blob: Blob, filename: string) {
    //  Native JavaScript
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    a.remove();
  }
}
