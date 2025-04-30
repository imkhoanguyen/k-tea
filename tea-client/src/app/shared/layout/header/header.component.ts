import { Component, inject, ViewChild } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { UserService } from '../../../core/services/user.service';
import { CommonModule } from '@angular/common';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { CartService } from '../../../core/services/cart.service';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { ToastrService } from 'ngx-toastr';
import { CartComponent } from '../../../features/cart/cart.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    NzMenuModule,
    NzIconModule,
    RouterLink,
    CommonModule,
    NzAvatarModule,
    NzDropDownModule,
    NzBadgeModule,
    NzDrawerModule,
    CartComponent,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  currentPage = 1;
  userService = inject(UserService);
  cartService = inject(CartService);
  utilService = inject(UtilitiesService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);

  @ViewChild(CartComponent) cartComponent!: CartComponent;

  changePage(page: number) {
    this.currentPage = page;
  }

  // open drawer cart
  open(): void {
    this.cartComponent.visible = true;
  }

  goHistoryOrder() {
    var user = this.userService.currentUser();
    if (!user) {
      this.toastrService.info('Vui lòng đăng nhập');
      return;
    }
    this.router.navigate(['/lich-su-dat-hang', user?.userName]);
  }
}
