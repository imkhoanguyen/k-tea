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

export const routes: Routes = [
  { path: 'danh-muc', component: CategoryListComponent },
  { path: 'san-pham', component: ItemListComponent },
  { path: 'them-san-pham', component: ItemAddComponent },
  { path: 'cap-nhat-san-pham/:id', component: ItemUpdateComponent },
  { path: 'dang-nhap', component: LoginComponent },
  { path: 'quyen', component: RoleListComponent },
  {
    path: 'cap-nhat-chuc-nang-cua-quyen/:id',
    component: RolePermissionComponent,
  },
  { path: 'nguoi-dung', component: UserListComponent },
  { path: 'them-nguoi-dung', component: UserAddComponent },
  { path: 'cap-nhat-nguoi-dung/:username', component: UserUpdateComponent },
  { path: 'ban-hang', component: SaleListComponent },

];
