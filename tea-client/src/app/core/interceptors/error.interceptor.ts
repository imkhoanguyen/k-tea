import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastrService = inject(ToastrService);

  return next(req).pipe(
    catchError((error) => {
      if (error) {
        console.log(error);
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const modalStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modalStateErrors.push(error.error.errors[key]);
                }
              }
              throw modalStateErrors.flat();
            } else {
              toastrService.error(
                error.error.statusCode + ' - ' + error.error.message
              );
            }
            break;
          case 401:
            toastrService.error(
              error.error.statusCode + ' - ' + error.error.message
            );
            break;
          case 403:
            toastrService.error(
              '403' + ' - ' + 'Bạn không có quyền thực hiện chức năng này'
            );
            break;
          case 404:
            if (error.url !== 'http://localhost:4200/') {
              toastrService.error(
                error.error.statusCode + ' - ' + error.error.message
              );
            }
            break;
          case 500:
            const navigationExtras: NavigationExtras = {
              state: { error: error.error },
            };
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toastrService.error('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    })
  );
};
