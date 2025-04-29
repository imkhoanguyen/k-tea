import { Routes } from '@angular/router';
import { CategoryListComponent } from './features/category/category-list/category-list.component';
import { ItemListComponent } from './features/item/item-list/item-list.component';
import { ItemAddComponent } from './features/item/item-add/item-add.component';
import { ItemUpdateComponent } from './features/item/item-update/item-update.component';
import { LoginComponent } from './features/login/login.component';
import { RoleListComponent } from './features/role/role-list/role-list.component';
import { RolePermissionComponent } from './features/role/role-permission/role-permission.component';
import { UserListComponent } from './features/user/user-list/user-list.component';
import { UserAddComponent } from './features/user/user-add/user-add.component';
import { UserUpdateComponent } from './features/user/user-update/user-update.component';
import { SaleListComponent } from './features/sale/sale-list/sale-list.component';
import { DiscountListComponent } from './features/discount/discount-list/discount-list.component';
import { ReportComponent } from './features/report/report.component';
import { NotfoundComponent } from './shared/errors/notfound/notfound.component';
import { ServererrorComponent } from './shared/errors/servererror/servererror.component';
import { OrderDetailComponent } from './features/order-detail/order-detail.component';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', component: ReportComponent, canActivate: [adminGuard] },
  {
    path: 'danh-muc',
    component: CategoryListComponent,
    canActivate: [adminGuard],
  },
  { path: 'san-pham', component: ItemListComponent, canActivate: [adminGuard] },
  {
    path: 'them-san-pham',
    component: ItemAddComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'cap-nhat-san-pham/:id',
    component: ItemUpdateComponent,
    canActivate: [adminGuard],
  },
  { path: 'dang-nhap', component: LoginComponent },
  { path: 'quyen', component: RoleListComponent, canActivate: [adminGuard] },
  {
    path: 'cap-nhat-chuc-nang-cua-quyen/:id',
    component: RolePermissionComponent,
    canActivate: [adminGuard],
  },
  { path: 'nguoi-dung', component: UserListComponent },
  {
    path: 'them-nguoi-dung',
    component: UserAddComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'cap-nhat-nguoi-dung/:username',
    component: UserUpdateComponent,
    canActivate: [adminGuard],
  },
  { path: 'ban-hang', component: SaleListComponent, canActivate: [adminGuard] },
  {
    path: 'ma-giam-gia',
    component: DiscountListComponent,
    canActivate: [adminGuard],
  },
  { path: 'thong-ke', component: ReportComponent, canActivate: [adminGuard] },
  {
    path: 'chi-tiet-don-hang/:id',
    component: OrderDetailComponent,
    canActivate: [adminGuard],
  },

  { path: 'not-found', component: NotfoundComponent },
  { path: 'server-error', component: ServererrorComponent },
  { path: '**', component: ReportComponent, pathMatch: 'full' },
];
