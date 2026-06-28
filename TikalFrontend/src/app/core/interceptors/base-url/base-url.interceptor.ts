import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export const baseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  const excludedPaths = ['/i18n/'];

  const isExcluded = excludedPaths.some((path) => req.url.includes(path));

  if (isExcluded) {
    return next(req);
  }

  const modifiedRequest = req.clone({
    url: `${environment.backend_url}${req.url}`,
  });

  return next(modifiedRequest);
};
