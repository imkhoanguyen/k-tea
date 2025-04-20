import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { LoginComponent } from './features/login/login.component';
import { PaymentReturnComponent } from './features/payment/payment-return/payment-return.component';
import { PaymentSuccessComponent } from './features/payment/payment-success/payment-success.component';
import { PaymentFailedComponent } from './features/payment/payment-failed/payment-failed.component';
import { OrderListComponent } from './features/order/order-list/order-list.component';
import { OrderDetailComponent } from './features/order/order-detail/order-detail.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'thuc-don', component: ShopComponent },
  { path: 'dang-nhap', component: LoginComponent },
  { path: 'xu-ly-thanh-toan', component: PaymentReturnComponent },
  { path: 'dat-hang-thanh-cong', component: PaymentSuccessComponent },
  { path: 'thanh-toan-that-bai', component: PaymentFailedComponent },
  { path: 'lich-su-dat-hang/:username', component: OrderListComponent },
  { path: 'chi-tiet-don-hang/:id', component: OrderDetailComponent },
];
