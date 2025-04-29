import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { UserService } from '../services/user.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { User } from '../../shared/models/user';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const userService = inject(UserService);
  const toastrService = inject(ToastrService);
  const router = inject(Router);
  let currentUser = userService.currentUser();
  if (currentUser) {
    const accessToken = currentUser.accessToken;
    const refreshToken = currentUser.refreshToken;
    let authorization = `Bearer ${accessToken}`;

    return next(req.clone({ setHeaders: { authorization } })).pipe(
      catchError((error) => {
        console.log(error);
        if (error instanceof HttpErrorResponse && error.status === 401) {
          if (refreshToken) {
            return userService.callRefreshToken(refreshToken).pipe(
              switchMap((res: User) => {
                userService.setCurrentUser(res);

                authorization = `Bearer ${res.accessToken}`;
                return next(req.clone({ setHeaders: { authorization } }));
              }),
              catchError((er) => {
                userService.logout();
                toastrService.info(
                  'Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại'
                );
                router.navigate(['/dang-nhap']);
                return throwError(
                  () => new Error('Phiên đăng nhập đã hết hạn 1 ')
                );
              })
            );
          } else {
            userService.logout();
            toastrService.info(
              'Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại'
            );
            userService.logout();
            router.navigate(['/dang-nhap']);
            return throwError(() => new Error('Phiên đăng nhập đã hết hạn 2'));
          }
        } else {
          // Xử lý lỗi khác nếu có
          return throwError(() => error);
        }
      })
    );
  } else {
    return next(req);
  }
};
