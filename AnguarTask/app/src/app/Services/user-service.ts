import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { SigningResponse } from '../Models/signing-response';
import { RegisterRequest } from '../Models/register-request';
import { LoginDto } from '../Dtos/login-dto';
import { User } from '../Models/user';
import { RefreshToken } from '../Models/refresh-token';

@Injectable({
  providedIn: 'root',
})
export class UserService {

  constructor(private httpClient:HttpClient){}
  jwtToken=signal<string|null>(null)
  refreshToken=signal<RefreshToken|null>(null)
  SignUp(registerDto:RegisterRequest):Observable<SigningResponse>{
    return this.httpClient.post<SigningResponse>(`https://localhost:7273/api/auth/register`,registerDto)
  }

  SignIn(signInDto:LoginDto):Observable<SigningResponse>{
    return this.httpClient.post<SigningResponse>(`https://localhost:7273/api/auth/login`,signInDto).pipe(
      tap(res=>{
        this.jwtToken.set(res.jwtToken)
        this.refreshToken.set(res.refreshToken)
      })
    )
  }

  GetCurrentUser():Observable<User>{
    return this.httpClient.get<User>(`https://localhost:7273/api/auth/currentUser`)
  }

  SignOut(user:User):Observable<void>{
    return this.httpClient.put<void>(`https://localhost:7273/api/auth/logout`,user)
  }

   
}
