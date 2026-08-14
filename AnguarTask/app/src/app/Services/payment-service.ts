import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Wallet } from '../Models/wallet';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {

  constructor(private httpClient:HttpClient){}

  ApplyPayment(wallet:Wallet):Observable<Wallet>{
    return this.httpClient.post<Wallet>(`https://localhost:7273/api/Payment`,wallet)
  }
  
}
