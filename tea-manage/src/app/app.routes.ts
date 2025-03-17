import { Routes } from '@angular/router';
import { CategoryListComponent } from './features/category/category-list/category-list.component';
import { ItemListComponent } from './features/item/item-list/item-list.component';
import { ItemAddComponent } from './features/item/item-add/item-add.component';
import { ItemUpdateComponent } from './features/item/item-update/item-update.component';

export const routes: Routes = [
  { path: 'danh-muc', component: CategoryListComponent },
  { path: 'san-pham', component: ItemListComponent },
  { path: 'them-san-pham', component: ItemAddComponent },
  { path: 'cap-nhat-san-pham/:id', component: ItemUpdateComponent },
];
