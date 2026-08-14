import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { UserService } from '../Services/user-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const userService=inject(UserService)
  const token=userService.jwtToken();
  if(!token){
    return next(req)
  }
  const authReq=req.clone({
    setHeaders:{
      Authorization:`Bearer ${token}`
    }
  })
  return next(authReq);
}; 
