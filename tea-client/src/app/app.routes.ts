import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { LoginComponent } from './features/login/login.component';
import { PaymentReturnComponent } from './features/payment/payment-return/payment-return.component';
import { PaymentSuccessComponent } from './features/payment/payment-success/payment-success.component';
import { PaymentFailedComponent } from './features/payment/payment-failed/payment-failed.component';
import { OrderListComponent } from './features/order/order-list/order-list.component';
import { OrderDetailComponent } from './features/order/order-detail/order-detail.component';
import { RecommendDrinkComponent } from './features/recommend-drink/recommend-drink.component';
import { RegisterComponent } from './features/register/register.component';
import { ForgotPasswordComponent } from './features/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/reset-password/reset-password.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'thuc-don', component: ShopComponent },
  { path: 'dang-nhap', component: LoginComponent },
  { path: 'xu-ly-thanh-toan', component: PaymentReturnComponent },
  { path: 'dat-hang-thanh-cong', component: PaymentSuccessComponent },
  { path: 'thanh-toan-that-bai', component: PaymentFailedComponent },
  { path: 'lich-su-dat-hang/:username', component: OrderListComponent },
  { path: 'chi-tiet-don-hang/:id', component: OrderDetailComponent },
  { path: 'goi-y-mon-ai', component: RecommendDrinkComponent },
  { path: 'dang-ky', component: RegisterComponent },
  { path: 'quen-mat-khau', component: ForgotPasswordComponent },
  { path: 'dat-lai-mat-khau', component: ResetPasswordComponent },
];
