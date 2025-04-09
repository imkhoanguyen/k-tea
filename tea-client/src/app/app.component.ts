import { Component, inject, OnInit } from '@angular/core';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { HeaderComponent } from './shared/layout/header/header.component';
import { RouterOutlet } from '@angular/router';
import { UserService } from './core/services/user.service';
import { CartService } from './core/services/cart.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NzLayoutModule, HeaderComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'tea-client';
  userService = inject(UserService);
  cartService = inject(CartService);
  private toastrService = inject(ToastrService);

  ngOnInit(): void {
    this.setCurrentUser();
    if (this.userService.currentUser()) {
      this.cartService
        .getCart(this.userService.currentUser()?.userName || '')
        .subscribe({
          next: (cart) => {
            // Cart đã được load và lưu trong cartService.cart()
            console.log('Cart loaded', cart);
          },
          error: (err) => {
            console.error('Failed to load cart', err);
            this.toastrService.error('Không thể tải giỏ hàng');
          },
        });
    }
  }
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.userService.setCurrentUser(user);
  }
}
