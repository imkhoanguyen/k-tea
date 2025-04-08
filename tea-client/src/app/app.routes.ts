import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'thuc-don', component: ShopComponent },
];
