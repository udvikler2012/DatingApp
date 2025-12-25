// import { HttpInterceptorFn } from '@angular/common/http';
// import { BusyService } from '../_services/busy.service';
// import { finalize } from 'rxjs';
// import { inject } from '@angular/core';

// export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
//   const busyService = inject(BusyService);

//   busyService.busy();

//   return next(req).pipe(
//     finalize(() => {
//       busyService.idle()
//     })
//   )
// };

