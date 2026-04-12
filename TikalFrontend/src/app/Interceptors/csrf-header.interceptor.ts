import { HttpInterceptorFn } from '@angular/common/http';

export const csrfHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const modifiedRequest = req.clone({
    withCredentials: true,
    headers: req.headers.set('X-CSRF', '1')
  });
  return next(modifiedRequest);
};