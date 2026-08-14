import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderDto } from '../Dtos/order-dto';
import { Order } from '../Models/order';
import { OrderItem } from '../Models/order-item';
import { OrderItemDto } from '../Dtos/order-item-dto';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  
  constructor(private httpClient:HttpClient){}
  
  CreateOrder(order:Order):Observable<OrderDto>{
    return this.httpClient.post<OrderDto>(`https://localhost:7273/api/orders`,order)
  }
  GetAllOrders():Observable<OrderDto[]>{
    return this.httpClient.get<OrderDto[]>(`https://localhost:7273/api/orders`)
  }
  AddOrderItemToOrder(orderItem:OrderItem,itemId:number,quantity:number):Observable<OrderItem>{
    return this.httpClient.post<OrderItem>(`https://localhost:7273/api/orders/orderItems/${itemId}?quantity=${quantity}`,orderItem)
  }
  DeleteOrderItemFromOrder(itemId:number):Observable<void>{
    return this.httpClient.delete<void>(`https://localhost:7273/api/orders/${itemId}`)
  }
  GetOrderItemsById(orderId:number):Observable<OrderItemDto[]>{
    return this.httpClient.get<OrderItemDto[]>(`https://localhost:7273/api/orders/orderItems/${orderId}`)
  }
  CancelOrder(order:Order):Observable<void>{
    return this.httpClient.put<void>(`https://localhost:7273/api/orders/cancel`,order)
  }

  
}
