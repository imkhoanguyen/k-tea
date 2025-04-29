import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const toastrService = inject(ToastrService);

  const currentUser = userService.currentUser();
  const haveAccessAdminPage = userService.hasClaim('Access_Admin');

  if (!haveAccessAdminPage) {
    toastrService.warning('Bạn không có quyền truy cập vào trang này');
    userService.logout();
    router.navigate(['/dang-nhap']);
    return false;
  }

  return true;
};
