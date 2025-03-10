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
}
